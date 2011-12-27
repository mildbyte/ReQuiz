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
        //User's answers to the quiz
        private QuizAnswers answers;

        /// <summary>
        /// Creates a submission form that will automatically submit the answers and
        /// display the score to the user
        /// </summary>
        /// <param name="answers">User's quiz answers</param>
        /// <param name="server">IP address of the server</param>
        /// <param name="port">Port number on the server</param>
        public frmSubmit(QuizAnswers answers, IPAddress server, int port)
        {
            //Set up the needed components
            InitializeComponent();
            this.answers = answers;

            //Initialise the answer-sending and mark-receiving component
            MarkReceiver answerSender = new MarkReceiver();
            answerSender.Server = server;
            answerSender.Port = port;
            answerSender.Username = System.Environment.UserName;
            answerSender.Answers = answers;

            //Set up the events to log connection progress and get back the 
            //quiz result when the operation is complete
            answerSender.ClientLog += LogEvent;
            answerSender.MarkReceived += DisplayMark;

            //Start the submission process
            answerSender.SendAnswersReceiveMark();

            Show();
        }

        private void LogEvent(object sender, ClientLogEventArgs e)
        {
            //Display the text the component wants to log
            lblConnectionProgress.Text = e.Message;
        }

        private void DisplayMark(object sender, MarkReceivedEventArgs e)
        {
            //Show the label with the result and the close button
            lblResult.Show();
            lblConnectionProgress.Hide();
            btnClose.Show();

            if (e.Result.answersAccepted)
            {
                //If the server accepted our answers, display the result and the percentage
                lblResult.Text = "Correct answers: " + e.Result.correctAnswers + "/" + answers.answers.Count +
                    string.Format(" ({0:#0}%)", e.Result.correctAnswers * 100 / answers.answers.Count) + Environment.NewLine +
                    "Hints used: " + answers.hintsUsed + Environment.NewLine +
                    "Total mark: " + e.Result.mark;
            }
            else
            {
                //The user tried to take the quiz twice, report that on the label
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
