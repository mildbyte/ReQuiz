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
    class MarkReceiver
    {
        private BackgroundWorker clientThread;
        private IPAddress serverAddress;
        private int serverPort;
        private string username;
        private QuizAnswers answers;

        public IPAddress Server
        {
            get
            {
                return serverAddress;
            }
            set
            {
                serverAddress = value;
            }
        }

        public string Username
        {
            get
            {
                return username;
            }
            set
            {
                username = value;
            }
        }

        public QuizAnswers Answers
        {
            get
            {
                return answers;
            }
            set
            {
                answers = value;
            }
        }

        public int Port
        {
            get
            {
                return serverPort;
            }
            set
            {
                serverPort = value;
            }
        }

        public event ClientLogEventHandler ClientLog;
        public event MarkReceivedEventHandler MarkReceived;
        public event ConnectFailedEventHandler ConnectFailed;

        public MarkReceiver()
        {
            clientThread = new BackgroundWorker();
            clientThread.WorkerReportsProgress = true;
            clientThread.WorkerSupportsCancellation = true;
            clientThread.DoWork += DoSubmitAnswers;
            clientThread.RunWorkerCompleted += FinishSubmitAnswers;
            clientThread.ProgressChanged += LogEvent;
        }

        public void SendAnswersReceiveMark()
        {
            if (!clientThread.IsBusy) clientThread.RunWorkerAsync();
        }

        public void CancelSendAnswersReceiveMark()
        {
            clientThread.CancelAsync();
        }

        private void LogEvent(object sender, ProgressChangedEventArgs e)
        {
            ClientLog(this, new ClientLogEventArgs((string)e.UserState));
        }
        private void DoSubmitAnswers(object sender, DoWorkEventArgs e)
        {
            clientThread.ReportProgress(0, "Establishing connection to " + serverAddress.ToString());
            TcpClient serverSocket = new TcpClient();
            serverSocket.Connect(new IPEndPoint(serverAddress, 1234));

            StreamWriter serverWriter = new StreamWriter(serverSocket.GetStream());
            StreamReader serverReader = new StreamReader(serverSocket.GetStream());

            clientThread.ReportProgress(0, "Requesting permission to submit the answers...");
            serverWriter.WriteLine("ANSW " + username);
            serverWriter.Flush();

            string serverReply = serverReader.ReadLine();
            if (serverReply == "NOPE")
            {
                QuizResult result;
                result.answersAccepted = false;
                result.correctAnswers = 0;
                result.mark = 0;
                e.Result = result;
            }
            else
            {
                clientThread.ReportProgress(0, "Submitting the answers...");

                serverWriter.WriteLine(answers.hintsUsed);
                foreach (string currAnswer in answers.answers)
                {
                    serverWriter.WriteLine(currAnswer);
                }
                serverWriter.Flush();

                clientThread.ReportProgress(0, "Receiving the mark...");
                QuizResult result;
                result.correctAnswers = int.Parse(serverReader.ReadLine());
                result.mark = int.Parse(serverReader.ReadLine());
                result.answersAccepted = true;
                e.Result = result;
            }
            serverSocket.Close();
        }

        private void FinishSubmitAnswers(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                try
                {
                    throw e.Error;
                }
                catch (Exception ex)
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
                    MarkReceived(this, new MarkReceivedEventArgs((QuizResult)e.Result));
                }
            }
        }
    }
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

    public class QuestionsFetchedEventArgs : EventArgs
    {
        private QuizParameters quizParameters;
        public QuestionsFetchedEventArgs(QuizParameters quizParameters)
        {
            this.quizParameters = quizParameters;
        }
        public QuizParameters Parameters
        {
            get
            {
                return quizParameters;
            }
        }
    }
    public delegate void QuestionsFetchedEventHandler(object sender, QuestionsFetchedEventArgs e);

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
