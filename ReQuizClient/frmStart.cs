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

        //Address and port of the server to connect to
        private IPAddress serverAddr;
        private int serverPort;

        public frmStart()
        {
            InitializeComponent();
        }

        private void OnConnectFailed(object sender, ConnectFailedEventArgs e)
        {
            //Don't display the message if the connection is not relevant anymore
            if (!isConnecting) return;

            //Show the error message and revert the form to the "Enter IP address" state
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
                //Validate the IP address and the port
                try
                {
                    serverAddr = IPAddress.Parse(txtAddress.Text.Trim());
                    serverPort = int.Parse(txtPort.Text.Trim());
                } catch (FormatException ex) {
                    MessageBox.Show("You have entered an invalid IP address and/or port.", 
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtAddress.SelectAll();
                    txtAddress.Select();
                    return;
                }

                //Hide the address fields and show the connection progress
                isConnecting = true;
                btnConnect.Enabled = false;
                lblConnectionProgress.Visible = true;
                txtAddress.Visible = false;
                lblIPAddress.Visible = false;

                //Set up the component that will receive the questions
                QuestionReceiver questionReceiver = new QuestionReceiver();
                questionReceiver.Server = serverAddr;
                questionReceiver.Port = serverPort;

                //Windows username is used as the username
                questionReceiver.Username = System.Environment.UserName;
                questionReceiver.ClientLog += LogEvent;
                questionReceiver.QuestionsFetched += OnConnectComplete;
                questionReceiver.ConnectFailed += OnConnectFailed;

                //Start the operation
                questionReceiver.ReceiveQuestions();
            }
        }

        private void LogEvent(object sender, ClientLogEventArgs e)
        {
            //Feed the log of the server to the connection progress label
            lblConnectionProgress.Text = e.Message;
        }

        private void OnConnectComplete(object sender, QuestionsFetchedEventArgs e)
        {
            //Hide the form and remove the connection progress label
            this.Hide();
            isConnecting = false;
            btnConnect.Enabled = true;
            lblConnectionProgress.Visible = false;
            txtAddress.Visible = true;
            lblIPAddress.Visible = true;

            //Create the form that would display the question to the user
            frmQuestions questForm = new frmQuestions(e.Parameters);

            //When the question form will be closed, request it to pass the user's answers back
            questForm.QuizCompleted += OnQuestionsClose;
            questForm.FormClosed += OnQuestionFormClose;
        }

        private void OnQuestionsClose(object sender, QuizCompletedEventArgs e)
        {
            //The user has entered the answers on the question form
            //Create a form that would submit the answers to the server
            frmSubmit submitForm = new frmSubmit(e.Answers, serverAddr, serverPort);

            //Notify this form when the submission form is closed
            submitForm.FormClosed += OnSubmitFormClose;
        }

        private void OnQuestionFormClose(object sender, FormClosedEventArgs e)
        {
            //The question form has closed
            //Destroy the questions form and show this form
            ((frmQuestions)sender).Dispose();
            this.Show();
        }

        private void OnSubmitFormClose(object sender, FormClosedEventArgs e)
        {
            //The user has viewed his results and wants to exit the application
            //Close the main form
            this.Close();
        }

        private void frmStart_Load(object sender, EventArgs e)
        {
            //Focus on the address field at startup
            txtAddress.Select();
        }
    }
}
