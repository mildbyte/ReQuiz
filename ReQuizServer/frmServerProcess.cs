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
        int clientsCompletedTest;

        public frmServerProcess(int portNumber, int noQuestions, int noHints, int minRegexLen, int maxRegexLen)
        {
            InitializeComponent();

            //Generate the question list
            List<IQuizQuestion> questions = RegexQuestions.GenerateQuestions(noQuestions, minRegexLen, maxRegexLen);

            clientsCompletedTest = 0;

            //Set up the server and start it
            serverHandler = new ServerNetworkHandler(questions, noHints, portNumber);
            serverHandler.ServerLog += LogEventHandler;
            serverHandler.ClientCompletedTest += ClientCompletedTestEventHandler;
            serverHandler.StartServer();
        }

        private void LogEventHandler(object sender, ServerLogEventArgs e)
        {
            //Append the time and write the log text to the listview
            lvLog.Items.Add(new ListViewItem(new string[]{DateTime.Now.ToLongTimeString(), e.Message}));
        }

        /// <summary>
        /// A client has completed the test, report this on the form
        /// </summary>
        private void ClientCompletedTestEventHandler(object sender, ClientCompletedTestEventArgs e)
        {
            clientsCompletedTest++;
            lblNoCompletedTest.Text = "Completed the test: " + clientsCompletedTest.ToString();
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            //Same action as closing the form
            this.Close();
        }

        private void frmServerProcess_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Confirm the user's intentions
            if (MessageBox.Show("The test has been completed by " + clientsCompletedTest.ToString()
                + " people. " + Environment.NewLine +
                "If you decide to finish the test now, nobody else will be able to submit their answers."
                + Environment.NewLine + "Are you sure you want to finish the test?", "ReQuiz",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
                return;
            }

            //Get the results from the server
            ConcurrentDictionary<string, ReQuizResult> results = serverHandler.StopServer();

            //Create the form for exporting quiz data
            frmFinish finishForm = new frmFinish(results);
            finishForm.Show();
        }

        private void frmServerProcess_Load(object sender, EventArgs e)
        {
            ttfrmServerProcess.SetToolTip(btnFinish, "Stops the testing and marks the answers");
        }
    }
}
