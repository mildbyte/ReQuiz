using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ReQuizServer
{
    public partial class frmFinish : Form
    {
        private ConcurrentDictionary<string, ReQuizResult> results;
        private bool resultsExported;

        public frmFinish(ConcurrentDictionary<string, ReQuizResult> results)
        {
            InitializeComponent();
            this.results = results;
            resultsExported = false;

            foreach(KeyValuePair<string, ReQuizResult> quizScore in results)
            {
                txtResults.Text += quizScore.Key + " - used " 
                    + quizScore.Value.hintsUsed + " hints, total score: "
                    + quizScore.Value.mark + Environment.NewLine;
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            sfdExportMarks.ShowDialog();
            if (sfdExportMarks.FileName == "") return;

            try {
                Stream outFile = sfdExportMarks.OpenFile();
                StreamWriter fileWriter = new StreamWriter(outFile);

                fileWriter.WriteLine("\"Username\";\"Hints used\";\"Total score\"");

                foreach(KeyValuePair<string, ReQuizResult> quizScore in results) {
                    fileWriter.WriteLine("\"" + quizScore.Key + "\";\""
                        + quizScore.Value.hintsUsed + "\";\""
                        + quizScore.Value.mark + "\"");
                }
                fileWriter.Close();
                resultsExported = true;
            } catch(IOException ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmFinish_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (resultsExported) return;

            e.Cancel = (MessageBox.Show("You have not exported the results. Are you sure you want to exit?",
                        "ReQuiz", MessageBoxButtons.YesNo, MessageBoxIcon.Question) 
                        == System.Windows.Forms.DialogResult.No);
        }

        private void frmFinish_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
