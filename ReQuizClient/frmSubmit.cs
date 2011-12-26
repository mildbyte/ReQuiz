using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace ReQuizClient
{
    public partial class frmSubmit : Form
    {
        private QuizAnswers answers;

        public frmSubmit(QuizAnswers answers, IPAddress server, int port)
        {
            InitializeComponent();
            this.answers = answers;
            MarkReceiver answerSender = new MarkReceiver();
            answerSender.Server = server;
            answerSender.Port = port;
            answerSender.Username = System.Environment.UserName;
            answerSender.Answers = answers;

            answerSender.ClientLog += LogEvent;
            answerSender.MarkReceived += DisplayMark;

            answerSender.SendAnswersReceiveMark();

            Show();
        }

        private void LogEvent(object sender, ClientLogEventArgs e)
        {
            lblConnectionProgress.Text = e.Message;
        }

        private void DisplayMark(object sender, MarkReceivedEventArgs e)
        {
            lblResult.Show();
            lblConnectionProgress.Hide();
            btnClose.Show();

            if (e.Result.answersAccepted)
            {
                lblResult.Text = "Correct answers: " + e.Result.correctAnswers + "/" + answers.answers.Count +
                    string.Format(" ({0:00}%)", e.Result.correctAnswers * 100 / answers.answers.Count) + Environment.NewLine +
                    "Hints used: " + answers.hintsUsed + Environment.NewLine +
                    "Total mark: " + e.Result.mark;
            }
            else
            {
                lblResult.Text = "The server has reported that you have already sent the answers once." + Environment.NewLine +
                    "You cannot submit the answers twice in one session." + Environment.NewLine +
                    "Only your first result will be stored on the server.";
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
