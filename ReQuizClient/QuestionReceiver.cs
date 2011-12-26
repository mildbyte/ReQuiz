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
    class QuestionReceiver
    {
        private BackgroundWorker clientThread;
        private IPAddress serverAddress;
        private string username;
        private int serverPort;

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
        public event QuestionsFetchedEventHandler QuestionsFetched;
        public event ConnectFailedEventHandler ConnectFailed;

        public QuestionReceiver()
        {
            clientThread = new BackgroundWorker();
            clientThread.WorkerReportsProgress = true;
            clientThread.WorkerSupportsCancellation = true;
            clientThread.DoWork += DoConnectAndFetchQuestions;
            clientThread.RunWorkerCompleted += FinishConnectAndFetchQuestions;
            clientThread.ProgressChanged += LogEvent;
        }

        public void ReceiveQuestions()
        {
            if (!clientThread.IsBusy) clientThread.RunWorkerAsync();
        }

        public void CancelReceiveQuestions()
        {
            clientThread.CancelAsync();
        }

        private void LogEvent(object sender, ProgressChangedEventArgs e)
        {
            ClientLog(this, new ClientLogEventArgs((string)e.UserState));
        }

        private void DoConnectAndFetchQuestions(object sender, DoWorkEventArgs e)
        {
            clientThread.ReportProgress(0, "Establishing connection to " + serverAddress.ToString());
            TcpClient serverSocket = new TcpClient();
            serverSocket.Connect(new IPEndPoint(serverAddress, serverPort));

            StreamWriter serverWriter = new StreamWriter(serverSocket.GetStream());
            StreamReader serverReader = new StreamReader(serverSocket.GetStream());

            clientThread.ReportProgress(0, "Fetching the questions...");

            serverWriter.WriteLine("QSTN " + username);
            serverWriter.Flush();

            int questionAmount = int.Parse(serverReader.ReadLine());

            QuizParameters result;
            result.hintsAllowed = int.Parse(serverReader.ReadLine());
            result.questions = new List<QuizQuestionRaw>();

            for (int i = 0; i < questionAmount; i++)
            {
                QuizQuestionRaw newQuestion;
                newQuestion.type = serverReader.ReadLine();
                newQuestion.parameters = serverReader.ReadLine();
                result.questions.Add(newQuestion);
            }
            e.Result = result;

            serverSocket.Close();
        }

        private void FinishConnectAndFetchQuestions(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                try
                {
                    throw e.Error;
                }
                catch (SocketException ex)
                {
                    if (ConnectFailed != null)
                    {
                        ConnectFailed(this, new ConnectFailedEventArgs(ex.Message));
                    }
                    return;
                }
            }
            if (!e.Cancelled)
            {
                QuestionsFetched(this, new QuestionsFetchedEventArgs((QuizParameters)e.Result));
                ClientLog(this, new ClientLogEventArgs("Questions fetched."));
            }
        }
    }
}
