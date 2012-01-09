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
        //Quiz results
        private ConcurrentDictionary<string, ReQuizResult> results;

        //Flag that shows whether the results have already been exported
        private bool resultsExported;

        public frmFinish(ConcurrentDictionary<string, ReQuizResult> results)
        {
            InitializeComponent();
            this.results = results;
            resultsExported = false;

            //Copy the results to the output window
            foreach(KeyValuePair<string, ReQuizResult> quizScore in results)
            {
                lvResults.Items.Add(new ListViewItem(new string[]{quizScore.Key, 
                    quizScore.Value.hintsUsed.ToString(), quizScore.Value.mark.ToString()}));
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            //Get the filename to export to
            sfdExportMarks.ShowDialog();
            
            //Exit if the user cancelled
            if (sfdExportMarks.FileName == "") return;

            try {
                //Open the file for output
                Stream outFile = sfdExportMarks.OpenFile();
                StreamWriter fileWriter = new StreamWriter(outFile);

                //Output the header
                fileWriter.WriteLine("\"Username\",\"Hints used\",\"Total score\"");

                //Output every user's result, enclosed by double quotes and separated by semicolons
                foreach(KeyValuePair<string, ReQuizResult> quizScore in results) {
                    fileWriter.WriteLine("\"" + quizScore.Key + "\",\""
                        + quizScore.Value.hintsUsed + "\",\""
                        + quizScore.Value.mark + "\"");
                }

                //Close the file and remember that we have exported the results
                fileWriter.Close();
                resultsExported = true;
            } catch(IOException ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            //Same action as closing the window
            this.Close();
        }

        private void frmFinish_FormClosing(object sender, FormClosingEventArgs e)
        {
            //If the results have been saved, keep closing
            if (resultsExported) return;

            //Else, ask the user
            e.Cancel = (MessageBox.Show("You have not exported the results. Are you sure you want to exit?",
                        "ReQuiz", MessageBoxButtons.YesNo, MessageBoxIcon.Question) 
                        == System.Windows.Forms.DialogResult.No);
        }

        private void frmFinish_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Close the remaining forms
            Application.Exit();
        }
    }
}
