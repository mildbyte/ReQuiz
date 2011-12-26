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

    public struct ReQuizResult
    {
        public int hintsUsed;
        public int mark;
    }

    class ServerNetworkHandler
    {
        private List<IQuizQuestion> questions;
        private string questionsBuf;
        private int hintsAllowed;
        private int serverPort;

        private ConcurrentDictionary<string, ReQuizResult> results;

        private BackgroundWorker serverThread;
        private TcpListener tcpListener;

        public event ServerLogEventHandler ServerLog;

        private string GetCurrentIP() 
        {
            IPAddress[] addresses = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress addr in addresses) {
                if (addr.AddressFamily == AddressFamily.InterNetwork) return addr.ToString();
            }
            return "127.0.0.1";
        }

        private void HandleClient(object client)
        {
            string clientAddress = ((TcpClient)client).Client.RemoteEndPoint.ToString();
            try
            {
                NetworkStream clientStream = ((TcpClient)client).GetStream();
                ASCIIEncoding encoder = new ASCIIEncoding();
                StreamReader clientReader = new StreamReader(clientStream);
                StreamWriter clientWriter = new StreamWriter(clientStream);

                serverThread.ReportProgress(0, "Inbound connection from " + clientAddress);

                string[] commands = clientReader.ReadLine().Split();
                string username = commands[1];

                serverThread.ReportProgress(0, clientAddress + " identified as " + username);

                if (commands[0] == "QSTN")
                {
                    serverThread.ReportProgress(0, username + " requests the questions");
                    clientWriter.WriteLine(questions.Count);
                    clientWriter.WriteLine(hintsAllowed);
                    clientWriter.Write(questionsBuf);

                    clientWriter.Flush();

                    serverThread.ReportProgress(0, "Sent the questions to " + username);
                }
                else if (commands[0] == "ANSW")
                {
                    if (results.ContainsKey(username))
                    {
                        clientWriter.WriteLine("NOPE");
                        clientWriter.Flush();
                        serverThread.ReportProgress(0, username + " tried to submit the answers again");
                    }
                    else
                    {
                        //TODO: do we penalize the hint usage even if the answer was wrong?
                        serverThread.ReportProgress(0, "Receiving the answers from " + username);
                        clientWriter.WriteLine("OKAY");
                        clientWriter.Flush();
                        int correctAnswers = 0;
                        int hintsUsed = int.Parse(clientReader.ReadLine());
                        foreach (IQuizQuestion currQuestion in questions)
                        {
                            string currAnswer = clientReader.ReadLine();
                            if (currQuestion.MarkAnswer(currAnswer)) correctAnswers++;
                        }

                        int clientMark = correctAnswers * 2 - hintsUsed;
                        if (clientMark < 0) clientMark = 0;

                        clientWriter.WriteLine(correctAnswers);
                        clientWriter.WriteLine(clientMark);
                        clientWriter.Flush();
                        serverThread.ReportProgress(0, "Marked " + username + "\'s answers: " +
                            correctAnswers + " correct, used " + hintsUsed + " hints, total score: " + clientMark);

                        ReQuizResult clientResult;
                        clientResult.hintsUsed = hintsUsed;
                        clientResult.mark = clientMark;

                        results[username] = clientResult;
                    }
                }
                
            } catch (Exception ex) {
                serverThread.ReportProgress(0, "Error while communicating with " + clientAddress + " (either network problem or faulty client version, exception reported: \"" + ex.Message + "\")");
            }

            serverThread.ReportProgress(0, "Closing connection to " + clientAddress);
            ((TcpClient)client).Close();
        }

        private void ServerLoop(object sender, DoWorkEventArgs e)
        {
            tcpListener = new TcpListener(IPAddress.Any, serverPort);
            tcpListener.Start();

            serverThread.ReportProgress(0, "Listening on " + GetCurrentIP() + ":" + serverPort);

            while (!serverThread.CancellationPending)
            {
                if (tcpListener.Pending()) {
                    TcpClient newClient = tcpListener.AcceptTcpClient();
                    Thread clientHandler = new Thread(new ParameterizedThreadStart(HandleClient));
                    clientHandler.Start(newClient);
                }
                Thread.Sleep(500);
            }
            serverThread.ReportProgress(0, "Shutting the server down...");
            tcpListener.Stop();
        }

        private void LogEvent(object sender, ProgressChangedEventArgs e)
        {
            if (ServerLog!=null)
                ServerLog(this, new ServerLogEventArgs((string)e.UserState));
        }

        public ServerNetworkHandler(List<IQuizQuestion> questions, int hintsAllowed, int port)
        {
            this.questions = questions;
            this.hintsAllowed = hintsAllowed;
            this.serverPort = port;

            foreach (IQuizQuestion currQuestion in questions) {
                questionsBuf += currQuestion.ToRawFormat() + Environment.NewLine;
            }

            this.results = new ConcurrentDictionary<string,ReQuizResult>();
        }
        
        public void StartServer()
        {
            if (ServerLog != null)
                ServerLog(this, new ServerLogEventArgs("Starting the server..."));
            serverThread = new BackgroundWorker();
            serverThread.DoWork += ServerLoop;
            serverThread.ProgressChanged += LogEvent;
            serverThread.WorkerSupportsCancellation = true;
            serverThread.WorkerReportsProgress = true;
            serverThread.RunWorkerAsync();
        }

        public ConcurrentDictionary<string, ReQuizResult> StopServer()
        {
            serverThread.CancelAsync();
            return results;
        }
    }
}
