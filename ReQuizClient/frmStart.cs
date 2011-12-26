using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Security.Principal;

namespace ReQuizClient
{
    public partial class frmStart : Form
    {
        private bool isConnecting = false;
        private IPAddress serverAddr;
        private int serverPort;

        public frmStart()
        {
            InitializeComponent();
        }

        private void OnConnectFailed(object sender, ConnectFailedEventArgs e)
        {
            if (!isConnecting) return;
            MessageBox.Show("Failed to connect to the server. Check that the server IP is entered correctly and that your connection is not being blocked by a firewall." + Environment.NewLine + "(" + e.Message + ")"
    , "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            isConnecting = false;
            btnConnect.Enabled = true;
            lblConnectionProgress.Visible = false;
            txtAddress.Visible = true;
            lblIPAddress.Visible = true;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (!isConnecting)
            {
                try
                {
                    serverAddr = IPAddress.Parse(txtAddress.Text.Trim());
                    serverPort = int.Parse(txtPort.Text.Trim());
                } catch (FormatException ex) {
                    MessageBox.Show("You have entered an invalid IP address and/or port.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtAddress.SelectAll();
                    txtAddress.Select();
                    return;
                }

                isConnecting = true;
                btnConnect.Enabled = false;
                lblConnectionProgress.Visible = true;
                txtAddress.Visible = false;
                lblIPAddress.Visible = false;

                QuestionReceiver questionReceiver = new QuestionReceiver();
                questionReceiver.Server = serverAddr;
                questionReceiver.Port = serverPort;
                questionReceiver.Username = System.Environment.UserName;
                questionReceiver.ClientLog += LogEvent;
                questionReceiver.QuestionsFetched += OnConnectComplete;
                questionReceiver.ConnectFailed += OnConnectFailed;

                questionReceiver.ReceiveQuestions();
            }
        }
        private void LogEvent(object sender, ClientLogEventArgs e)
        {
            lblConnectionProgress.Text = e.Message;
        }
        private void OnConnectComplete(object sender, QuestionsFetchedEventArgs e)
        {
            this.Hide();
            isConnecting = false;
            btnConnect.Enabled = true;
            lblConnectionProgress.Visible = false;
            txtAddress.Visible = true;
            lblIPAddress.Visible = true;
            frmQuestions questForm = new frmQuestions(e.Parameters);
            questForm.QuizCompleted += OnQuestionsClose;
            questForm.FormClosed += OnQuestionFormClose;
        }

        private void OnQuestionsClose(object sender, QuizCompletedEventArgs e)
        {
            QuizAnswers qAns = e.Answers;
            frmSubmit submitForm = new frmSubmit(qAns, serverAddr, serverPort);
            submitForm.FormClosed += OnSubmitFormClose;
        }

        private void OnQuestionFormClose(object sender, FormClosedEventArgs e)
        {
            ((frmQuestions)sender).Dispose();
            this.Show();
        }

        private void OnSubmitFormClose(object sender, FormClosedEventArgs e)
        {
            this.Close();
        }

        private void frmStart_Load(object sender, EventArgs e)
        {
            txtAddress.Select();
        }
    }
}
