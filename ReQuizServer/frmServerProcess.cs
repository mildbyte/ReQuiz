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
            List<IQuizQuestion> questions = RegexQuestions.GenerateQuestions(noQuestions);

            serverHandler = new ServerNetworkHandler(questions, noHints, portNumber);
            serverHandler.ServerLog += LogEventHandler;
            serverHandler.StartServer();
        }

        private void LogEventHandler(object sender, ServerLogEventArgs e)
        {
            LogEvent(e.Message);
        }

        private void LogEvent(string toLog)
        {
            txtLog.Text += System.DateTime.Now.ToString() + " " + toLog + Environment.NewLine;
            txtLog.ScrollToCaret();
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmServerProcess_FormClosing(object sender, FormClosingEventArgs e)
        {
            ConcurrentDictionary<string, ReQuizResult> results = serverHandler.StopServer();
            frmFinish finishForm = new frmFinish(results);
            finishForm.Show();
        }
    }
}
