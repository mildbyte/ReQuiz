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
                (int)nudNoQuestions.Value, (int)nudNoHints.Value);
            this.Hide();
            serverForm.Show();
        }
    }
}
