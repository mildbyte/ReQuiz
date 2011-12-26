using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ReQuizServer
{
    public partial class frmServerProcess : Form
    {
        ServerNetworkHandler serverHandler;

        public frmServerProcess(int portNumber, int noQuestions, int noHints)
        {
            InitializeComponent();

            //Generate the question list
            List<IQuizQuestion> questions = RegexQuestions.GenerateQuestions(noQuestions);

            //Set up the server and start it
            serverHandler = new ServerNetworkHandler(questions, noHints, portNumber);
            serverHandler.ServerLog += LogEventHandler;
            serverHandler.StartServer();
        }

        private void LogEventHandler(object sender, ServerLogEventArgs e)
        {
            //Pass the event to the actual logging procedure
            LogEvent(e.Message);
        }

        private void LogEvent(string toLog)
        {
            //Append the time and write the log string to the output
            txtLog.Text += System.DateTime.Now.ToString() + " " + toLog + Environment.NewLine;
            txtLog.ScrollToCaret();
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            //Same action as closing the form
            this.Close();
        }

        private void frmServerProcess_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Get the results from the server
            ConcurrentDictionary<string, ReQuizResult> results = serverHandler.StopServer();

            //Create the form for exporting quiz data
            frmFinish finishForm = new frmFinish(results);
            finishForm.Show();
        }
    }
}
