using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace ReQuizServer
{
    /// <summary>
    /// Gets thrown when the client sends an invalid command to the server
    /// </summary>
    public class ClientInvalidCommandException : ApplicationException
    {
        public ClientInvalidCommandException() { }
        public ClientInvalidCommandException(string message) : base(message) { }
    }

    /// <summary>
    /// Event arguments for the ServerLog event
    /// </summary>
    public class ServerLogEventArgs : EventArgs
    {
        private string message;
        public ServerLogEventArgs(string message)
        {
            this.message = message;
        }
        public string Message
        {
            get
            {
                return message;
            }
        }
    }
    public delegate void ServerLogEventHandler(object sender, ServerLogEventArgs e);

    public class ClientCompletedTestEventArgs : EventArgs
    {
    }
    public delegate void ClientCompletedTestEventHandler(object sender, ClientCompletedTestEventArgs e);


    /// <summary>
    /// Holds a result (mark and the amount of used hints) of one user
    /// </summary>
    public struct ReQuizResult
    {
        public int hintsUsed;
        public int mark;
    }


    /// <summary>
    /// The networking part of ReQuiz that listens to incoming connections,
    /// sends questions to users, marks their answers and keeps their scores
    /// </summary>
    class ServerNetworkHandler
    {
        //List of questions sent by the server
        private List<IQuizQuestion> questions;

        //Questions converted to the raw format suitable for sending
        private string questionsBuf;

        //Number of allowed hints
        private int hintsAllowed;

        //Port to listen for connections on
        private int serverPort;

        //A dictionary that stores each user's name and result
        private ConcurrentDictionary<string, ReQuizResult> results;

        //The listening thread that runs in the background
        private BackgroundWorker serverThread;
        
        //A component that listens for incoming connections
        private TcpListener tcpListener;

        /// <summary>
        /// Gets raised when the server needs to log an event
        /// </summary>
        public event ServerLogEventHandler ServerLog;

        /// <summary>
        /// Gets raised when a client completes the test
        /// </summary>
        public event ClientCompletedTestEventHandler ClientCompletedTest;
        
        /// <summary>
        /// Gets the local IPv4 address of the current computer
        /// </summary>
        /// <returns>The IPv4 address of the current computer</returns>
        private string GetCurrentIP() 
        {
            //Get all addresses assigned to this computer
            IPAddress[] addresses = Dns.GetHostAddresses(Dns.GetHostName());

            //Filter the first IPv4 address
            foreach (IPAddress addr in addresses) {
                if (addr.AddressFamily == AddressFamily.InterNetwork) return addr.ToString();
            }

            //If no addresses found, return the local (loopback) address
            return "127.0.0.1";
        }

        /// <summary>
        /// Handles a connection to a single client
        /// Shouldn't be called explicitly, supposed to be run in a separate thread
        /// </summary>
        private void HandleClient(object client)
        {
            string clientAddress = ((TcpClient)client).Client.RemoteEndPoint.ToString();
            try
            {
                //Establish the needed I/O streams
                NetworkStream clientStream = ((TcpClient)client).GetStream();
                StreamReader clientReader = new StreamReader(clientStream);
                StreamWriter clientWriter = new StreamWriter(clientStream);

                serverThread.ReportProgress(0, "Inbound connection from " + clientAddress);

                //The first line sent from the client is the command, followed by username.
                string[] commands = clientReader.ReadLine().Split();

                //If the first line is any different, stop the communication
                if (commands.Length != 2) throw new ClientInvalidCommandException();

                string username = commands[1];

                //There are no identification mechanisms here because the username
                //sent by the client software is the current Windows username.
                //One could possibly reverse engineer the protocol and directly send the 
                //relevant strings to the server, changing the username and taking the quiz
                //twice , but this would quickly be detected when the server would report
                //more users than intended taking the quiz

                serverThread.ReportProgress(0, clientAddress + " identified as " + username);

                if (commands[0] == "QSTN")
                {
                    //"QSTN" command 
                    serverThread.ReportProgress(0, username + " requests the questions");

                    //Send all question data back to the user
                    clientWriter.WriteLine(questions.Count);
                    clientWriter.WriteLine(hintsAllowed);
                    clientWriter.Write(questionsBuf);

                    clientWriter.Flush();

                    serverThread.ReportProgress(0, "Sent the questions to " + username);
                }
                else if (commands[0] == "ANSW")
                {
                    //"ANSW" command - user requests to send back the answers
                    if (results.ContainsKey(username))
                    {
                        //If the user has already sent the answers once, give him a slap
                        //on the wrist and terminate the connection
                        clientWriter.WriteLine("NOPE");
                        clientWriter.Flush();
                        serverThread.ReportProgress(0, username + " tried to submit the answers again");
                    }
                    else
                    {
                        //TODO: do we penalize the hint usage even if the answer was wrong?
                        //For that we would need to send whether the hint was used for every question

                        serverThread.ReportProgress(0, "Receiving the answers from " + username);

                        //Give the client permission to send
                        clientWriter.WriteLine("OKAY");
                        clientWriter.Flush();
                        int correctAnswers = 0;
                        int hintsUsed = 0;

                        //Read the amount of hints used by the client
                        try
                        {
                            hintsUsed = int.Parse(clientReader.ReadLine());
                        }
                        catch (FormatException)
                        {
                            //The client send a text string instead, stop the connection
                            throw new ClientInvalidCommandException();
                        }

                        //The answers are sent in the same order, so we iterate
                        //through the original quiz question objects which
                        //support marking answers
                        foreach (IQuizQuestion currQuestion in questions)
                        {
                            //Read the answer
                            string currAnswer = clientReader.ReadLine();

                            //Mark it
                            if (currQuestion.MarkAnswer(currAnswer)) correctAnswers++;
                        }

                        //Deduct 1 mark for every hint used
                        int clientMark = correctAnswers * 2 - hintsUsed;

                        //Don't allow negative marks
                        if (clientMark < 0) clientMark = 0;

                        //Report the mark to the client and to the log.
                        clientWriter.WriteLine(correctAnswers);
                        clientWriter.WriteLine(clientMark);
                        clientWriter.Flush();
                        serverThread.ReportProgress(0, "Marked " + username + "\'s answers: " +
                            correctAnswers + " correct, used " + hintsUsed + " hints, total score: " + clientMark);
                        serverThread.ReportProgress(1);

                        //Store the results in the dictionary
                        //TODO: check if executes correctly with simultaneous users
                        ReQuizResult clientResult;
                        clientResult.hintsUsed = hintsUsed;
                        clientResult.mark = clientMark;
                        results[username] = clientResult;
                    }
                }
                else
                {
                    //Unsupported command from the client
                    throw new ClientInvalidCommandException();
                }

            }
            catch (SocketException ex)
            {
                //Report any connection problems
                serverThread.ReportProgress(0, 
                    "Network problem while communicating with " + clientAddress +
                    " (exception reported: \"" + ex.Message + "\")");
            }
            catch (ClientInvalidCommandException)
            {
                serverThread.ReportProgress(0,
                    "Invalid data received from " + clientAddress + " probably due to faulty client version ");
            }

            //Close the connection to the client
            serverThread.ReportProgress(0, "Closing connection to " + clientAddress);
            ((TcpClient)client).Close();
        }

        /// <summary>
        /// The background listening loop of the server, executed by
        /// the BackgroundWorker
        /// </summary>
        private void ServerLoop(object sender, DoWorkEventArgs e)
        {
            //Set up the listener
            tcpListener = new TcpListener(IPAddress.Any, serverPort);
            tcpListener.Start();

            serverThread.ReportProgress(0, "Listening on " + GetCurrentIP() + ":" + serverPort);

            //Loop while there are no cancellation requests from the user
            while (!serverThread.CancellationPending)
            {
                //Clear the backlog of pending connections
                while (tcpListener.Pending()) {
                    //Accept the client
                    TcpClient newClient = tcpListener.AcceptTcpClient();

                    //Start a new thread to handle communication
                    Thread clientHandler = new Thread(new ParameterizedThreadStart(HandleClient));
                    clientHandler.Start(newClient);
                }

                //The listener will be polled every half a second to allow other processes
                //to take place and prevent high CPU usage
                Thread.Sleep(500);
            }

            //Shut down the server
            tcpListener.Stop();
            serverThread.ReportProgress(0, "Server shut down successfully.");
        }

        /// <summary>
        /// Propagates the log event from the listening thread to the UI
        /// </summary>
        private void LogEvent(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 0)
            {
                if (ServerLog != null)
                    ServerLog(this, new ServerLogEventArgs((string)e.UserState));
            }
            else
            {
                if (ClientCompletedTest != null)
                    ClientCompletedTest(this, new ClientCompletedTestEventArgs());
            }
        }

        /// <summary>
        /// Creates a ReQuiz server instance and initializes the needed variables
        /// </summary>
        /// <param name="questions">List of ReQuiz questions to test users on</param>
        /// <param name="hintsAllowed">Number of hints allowed</param>
        /// <param name="port">Port to listen for incoming connections on</param>
        public ServerNetworkHandler(List<IQuizQuestion> questions, int hintsAllowed, int port)
        {
            this.questions = questions;
            this.hintsAllowed = hintsAllowed;
            this.serverPort = port;

            //Convert all questions in the buffer to raw format to increase sending speed
            foreach (IQuizQuestion currQuestion in questions) {
                questionsBuf += currQuestion.ToRawFormat() + Environment.NewLine;
            }

            this.results = new ConcurrentDictionary<string,ReQuizResult>();
        }
        
        /// <summary>
        /// Starts the server
        /// </summary>
        public void StartServer()
        {
            //Return if the server's already running
            if (serverThread != null) return;
            if (ServerLog != null)
                ServerLog(this, new ServerLogEventArgs("Starting the server..."));

            //Set up the background worker
            serverThread = new BackgroundWorker();
            serverThread.DoWork += ServerLoop;
            serverThread.WorkerSupportsCancellation = true;

            //Normal use for the BackgroundWorker is to report progress in form of percentage.
            //In this case, percentage denotes the type of log event (0 - log to the form, 1 - client completed the test)
            //and the additional UserState parameter - the actual log event
            serverThread.ProgressChanged += LogEvent;
            serverThread.WorkerReportsProgress = true;

            //Start the listening thread
            serverThread.RunWorkerAsync();
        }

        /// <summary>
        /// Stops the server
        /// </summary>
        /// <returns>Quiz marks for every user</returns>
        public ConcurrentDictionary<string, ReQuizResult> StopServer()
        {
            serverThread.CancelAsync();
            return results;
        }
    }
}
