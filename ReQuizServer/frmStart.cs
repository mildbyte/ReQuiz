using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ReQuizServer
{
    public partial class frmStart : Form
    {
        public frmStart()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            //Create the main form that displays the server status and show it
            frmServerProcess serverForm = new frmServerProcess((int)nudPortNumber.Value,
                (int)nudNoQuestions.Value, (int)nudNoHints.Value,
                (int)nudReLenMin.Value, (int)nudReLenMax.Value);
            this.Hide();
            serverForm.Show();
        }

        private void nudNoQuestions_ValueChanged(object sender, EventArgs e)
        {
            //No point in selecting more hints than there are questions,
            //as every question has only one hint

            nudNoHints.Maximum = nudNoQuestions.Value;
        }

        private void nudReLenMax_ValueChanged(object sender, EventArgs e)
        {
            //Adjust the bounds on the minimum regular expression length
            nudReLenMin.Maximum = nudReLenMax.Value;
        }

        private void nudReLenMin_ValueChanged(object sender, EventArgs e)
        {
            //Adjust the bounds on the maximim regular expression length
            nudReLenMax.Minimum = nudReLenMin.Value;
        }

        private void frmStart_Load(object sender, EventArgs e)
        {
            ttfrmStart.SetToolTip(btnStart, "Starts the testing server");
        }
    }
}
