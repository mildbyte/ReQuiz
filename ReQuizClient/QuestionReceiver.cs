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
    /// Fetches the questions from a quiz server
    /// </summary>
    class QuestionReceiver
    {
        //Background thread used to execute the operation
        private BackgroundWorker clientThread;

        //Data about the connection
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

        /// <summary>
        /// Occurs when the component needs to report an event
        /// </summary>
        public event ClientLogEventHandler ClientLog;

        /// <summary>
        /// Occurs when the component has finished receiving questions
        /// </summary>
        public event QuestionsFetchedEventHandler QuestionsFetched;

        /// <summary>
        /// Occurs in case of connection failure
        /// </summary>
        public event ConnectFailedEventHandler ConnectFailed;

        public QuestionReceiver()
        {
            //Set up the background worker thread
            clientThread = new BackgroundWorker();
            clientThread.WorkerReportsProgress = true;
            clientThread.WorkerSupportsCancellation = true;

            //Specify the event handlers for the worker
            clientThread.DoWork += DoConnectAndFetchQuestions;
            clientThread.RunWorkerCompleted += FinishConnectAndFetchQuestions;
            clientThread.ProgressChanged += LogEvent;
        }

        /// <summary>
        /// Starts the process of fetching the questions from the server
        /// </summary>
        public void ReceiveQuestions()
        {
            //Start the background thread
            if (!clientThread.IsBusy) clientThread.RunWorkerAsync();
        }

        /// <summary>
        /// Requests the current background operation to terminate
        /// </summary>
        public void CancelReceiveQuestions()
        {
            clientThread.CancelAsync();
        }

        //Propagates the log request to the event handler specified by the user
        private void LogEvent(object sender, ProgressChangedEventArgs e)
        {
            ClientLog(this, new ClientLogEventArgs((string)e.UserState));
        }

        //Performs the actual fetch operation
        private void DoConnectAndFetchQuestions(object sender, DoWorkEventArgs e)
        {
            //Establish the connection to the server
            clientThread.ReportProgress(0, "Establishing connection to " + serverAddress.ToString());
            TcpClient serverSocket = new TcpClient();
            serverSocket.Connect(new IPEndPoint(serverAddress, serverPort));

            //Get the needed IO streams
            StreamWriter serverWriter = new StreamWriter(serverSocket.GetStream());
            StreamReader serverReader = new StreamReader(serverSocket.GetStream());

            //Send the request for questions to the server
            clientThread.ReportProgress(0, "Fetching the questions...");
            serverWriter.WriteLine("QSTN " + username);
            serverWriter.Flush();

            //Receive the amount of questions in the quiz
            int questionAmount = int.Parse(serverReader.ReadLine());

            //Set up the structure to store the unparsed quiz questions
            QuizParameters result;
            result.hintsAllowed = int.Parse(serverReader.ReadLine());
            result.questions = new List<QuizQuestionRaw>();

            for (int i = 0; i < questionAmount; i++)
            {
                //For every question, fetch its type and parameters
                QuizQuestionRaw newQuestion;
                newQuestion.type = serverReader.ReadLine();
                newQuestion.parameters = serverReader.ReadLine();
                result.questions.Add(newQuestion);
            }

            //Return the fetched questions and close the connection to the server
            e.Result = result;
            serverSocket.Close();
        }

        //Finalises the operation performed by the background thread
        private void FinishConnectAndFetchQuestions(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                //In case of connection error during the operation
                //raise the ConnectFailed event and send back the error text
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
                //Send the fetched questions to the user by raising the QuestionsFetched event
                ClientLog(this, new ClientLogEventArgs("Questions fetched."));
                QuestionsFetched(this, new QuestionsFetchedEventArgs((QuizParameters)e.Result));
            }
        }
    }


    /// <summary>
    /// Holds the QuizParameters structure for the QuestionsFetched event
    /// </summary>
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
}
