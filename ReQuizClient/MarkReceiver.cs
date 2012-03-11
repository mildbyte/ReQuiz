using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace ReQuizClient
{
    /// <summary>
    /// Submits the answer to the ReQuiz server and receives the marks from it
    /// </summary>
    public class MarkReceiver
    {
        //Thread used to perform the operation
        private BackgroundWorker clientThread;

        //Information about the connection
        private IPAddress serverAddress;
        private int serverPort;
        private string username;
        
        //User's answers to the quiz
        private QuizAnswers answers;

        /// <summary>
        /// Gets raised when a log message needs to be reported
        /// </summary>
        public event ClientLogEventHandler ClientLog;   //

        /// <summary>
        /// Gets raised when the process has finished fetching marks
        /// </summary>
        public event MarkReceivedEventHandler MarkReceived;

        /// <summary>
        /// Gers raised on connection failure
        /// </summary>
        public event ConnectFailedEventHandler ConnectFailed;

        public MarkReceiver(IPAddress serverAddress, int serverPort, string username, QuizAnswers quizAnswers)
        {
            //Store the connection parameters
            this.serverAddress = serverAddress;
            this.serverPort = serverPort;
            this.username = username;
            this.answers = quizAnswers;

            //Set up the background thread
            clientThread = new BackgroundWorker();
            clientThread.WorkerReportsProgress = true;
            clientThread.WorkerSupportsCancellation = true;
            clientThread.DoWork += DoSubmitAnswers;
            clientThread.RunWorkerCompleted += FinishSubmitAnswers;
            clientThread.ProgressChanged += LogEvent;
        }

        /// <summary>
        /// Starts the background operation. MarkReceived event is raised when finished.
        /// </summary>
        public void SendAnswersReceiveMark()
        {
            if (!clientThread.IsBusy) clientThread.RunWorkerAsync();
        }

        /// <summary>
        /// Sends the background thread a request to cancel the operation.
        /// </summary>
        public void CancelSendAnswersReceiveMark()
        {
            clientThread.CancelAsync();
        }

        //Propagates the log event further to the event handler
        private void LogEvent(object sender, ProgressChangedEventArgs e)
        {
            if (ClientLog != null)
            {
                ClientLog(this, new ClientLogEventArgs((string)e.UserState));
            }
        }

        //Performs the actual communication to the server in the background thread
        private void DoSubmitAnswers(object sender, DoWorkEventArgs e)
        {
            //Establish the connection to the server
            clientThread.ReportProgress(0, "Establishing connection to " + serverAddress.ToString());
            TcpClient serverSocket = new TcpClient();
            serverSocket.Connect(new IPEndPoint(serverAddress, serverPort));

            //Get the needed IO streams to the server
            StreamWriter serverWriter = new StreamWriter(serverSocket.GetStream());
            StreamReader serverReader = new StreamReader(serverSocket.GetStream());

            //Send the server a request to submit the answers
            clientThread.ReportProgress(0, "Requesting permission to submit the answers...");
            serverWriter.WriteLine("ANSW " + username);
            serverWriter.Flush();

            string serverReply = serverReader.ReadLine();

            //"NOPE" from the server means that we have already submitted the answers beforehand
            if (serverReply == "NOPE")
            {
                //Report the failure
                QuizResult result;
                result.answersAccepted = false;
                result.correctAnswers = 0;
                result.mark = 0;
                e.Result = result;
            }
            else
            {
                clientThread.ReportProgress(0, "Submitting the answers...");

                //Send the answers to the server
                serverWriter.WriteLine(answers.hintsUsed);
                foreach (string currAnswer in answers.answers)
                {
                    serverWriter.WriteLine(currAnswer);
                }
                serverWriter.Flush();

                //Receive the mark and the amount of accepted answers from the server
                clientThread.ReportProgress(0, "Receiving the mark...");
                QuizResult result;
                result.correctAnswers = int.Parse(serverReader.ReadLine());
                result.mark = int.Parse(serverReader.ReadLine());
                result.answersAccepted = true;
                e.Result = result;
            }
            
            //Finally, close the connection to the server
            serverSocket.Close();
        }

        //Gets run by the background worker after the operation is completed
        private void FinishSubmitAnswers(object sender, RunWorkerCompletedEventArgs e)
        {
            //Check for errors during the execution of the background worker
            if (e.Error != null)
            {
                //If there was an error, report the error to the user
                try
                {
                    throw e.Error;
                }
                catch (SocketException ex)
                {
                    MessageBox.Show("Failed to connect to the server. Check that the server IP is entered correctly and that your connection is not being blocked by a firewall." + Environment.NewLine + "(" + ex.Message + ")"
                       , "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                if (MarkReceived != null)
                {
                    //Call the event handlers and send them the quiz result
                    MarkReceived(this, new MarkReceivedEventArgs((QuizResult)e.Result));
                }
            }
        }
    }

    /// <summary>
    /// Stores the quiz result for the MarkReceived event
    /// </summary>
    public class MarkReceivedEventArgs : EventArgs
    {
        private QuizResult quizResult;
        public MarkReceivedEventArgs(QuizResult quizResult)
        {
            this.quizResult = quizResult;
        }
        public QuizResult Result
        {
            get
            {
                return quizResult;
            }
        }
    }
    public delegate void MarkReceivedEventHandler(object sender, MarkReceivedEventArgs e);


    /// <summary>
    /// Stores the log message for the ClientLog event
    /// </summary>
    public class ClientLogEventArgs : EventArgs
    {
        private string message;
        public ClientLogEventArgs(string message)
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
    public delegate void ClientLogEventHandler(object sender, ClientLogEventArgs e);


    /// <summary>
    /// Stores the data for the ConnectFailed event
    /// </summary>
    public class ConnectFailedEventArgs : EventArgs
    {
        private string message;
        public ConnectFailedEventArgs(string message)
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
    public delegate void ConnectFailedEventHandler(object sender, ConnectFailedEventArgs e);
}
