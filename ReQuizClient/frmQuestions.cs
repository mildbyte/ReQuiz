using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ReQuizClient
{
    public partial class frmQuestions : Form
    {
        public event QuizCompletedEventHandler QuizCompleted;

        private QuizParameters quizParameters;
        private QuizAnswers quizAnswers;
        private List<IQuizQuestion> quizQuestions;
        private List<Panel> renderedQuestions;
        private int currQuestionID;

        private int hintsUsed;

        public frmQuestions(QuizParameters quizParameters)
        {
            InitializeComponent();
            this.quizParameters = quizParameters;
            quizAnswers.answers = new List<string>();

            quizQuestions = new List<IQuizQuestion>();
            renderedQuestions = new List<Panel>();

            hintsUsed = 0;

            Font questionFont = new Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            Font answerFont = questionFont = new Font("Microsoft Sans Serif", 10F);

            MessageBox.Show("This test has " + quizParameters.questions.Count + " questions." + Environment.NewLine +
            "You are allowed " + quizParameters.hintsAllowed + " hints.");


            foreach (QuizQuestionRaw question in quizParameters.questions) {
                IQuizQuestion parsedQuestion = RegexQuestions.CreateQuestion(question);
                Panel thisQuestionPanel = new Panel();
                thisQuestionPanel.Parent = this;
                thisQuestionPanel.Left = 0;
                thisQuestionPanel.Width = this.Width;
                thisQuestionPanel.Top = 0;
                thisQuestionPanel.Height = this.Height - 20;
                this.Controls.Add(thisQuestionPanel);
                thisQuestionPanel.Hide();

                parsedQuestion.Render(thisQuestionPanel, questionFont, answerFont);

                quizQuestions.Add(parsedQuestion);
                renderedQuestions.Add(thisQuestionPanel);
            }

            currQuestionID = 0;

            this.Show();
            renderedQuestions[0].Show();
            UpdateStatusBar();
        }

        private void UpdateStatusBar()
        {
            btnHint.BringToFront();
            btnHint.Visible = (hintsUsed < quizParameters.hintsAllowed) && (quizQuestions[currQuestionID].HintAvailable());
            btnNextQuestion.Enabled = currQuestionID < quizQuestions.Count - 1;
            btnPrevQuestion.Enabled = currQuestionID > 0;
            lblHints.Text = "Hints: " + (quizParameters.hintsAllowed - hintsUsed).ToString();
            lblQuestionNumber.Text = currQuestionID + 1 + "/" + quizQuestions.Count();
        }

        private void frmQuestions_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            DialogResult choice = MessageBox.Show("Are you sure you want to submit your answers for marking?", "Answers",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (choice == DialogResult.No) return;
            
            foreach (IQuizQuestion question in quizQuestions)
            {
                quizAnswers.answers.Add(question.GetAnswer());
            }

            quizAnswers.hintsUsed = hintsUsed;

            if (QuizCompleted != null)
            {
                QuizCompleted(this, new QuizCompletedEventArgs(quizAnswers));
            }
        }

        private void btnPrevQuestion_Click(object sender, EventArgs e)
        {
            renderedQuestions[currQuestionID].Hide();
            if (currQuestionID != 0) currQuestionID--;
            renderedQuestions[currQuestionID].Show();

            UpdateStatusBar();
        }

        private void btnNextQuestion_Click(object sender, EventArgs e)
        {
            renderedQuestions[currQuestionID].Hide();
            if (currQuestionID != quizQuestions.Count - 1) currQuestionID++;
            renderedQuestions[currQuestionID].Show();

            UpdateStatusBar();
        }

        private void btnHint_Click(object sender, EventArgs e)
        {   
            quizQuestions[currQuestionID].DisplayHint();
            hintsUsed++;
            UpdateStatusBar();
        }

        private void frmQuestions_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult choice = MessageBox.Show("Are you sure you want to exit this quiz without submitting your answers?", "ReQuiz", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            e.Cancel = (choice == DialogResult.No);
        }
    }


    public class QuizCompletedEventArgs : EventArgs
    {
        private QuizAnswers quizAnswers;
        public QuizCompletedEventArgs(QuizAnswers quizAnswers)
        {
            this.quizAnswers = quizAnswers;
        }
        public QuizAnswers Answers
        {
            get
            {
                return quizAnswers;
            }
        }
    }
    public delegate void QuizCompletedEventHandler(object sender, QuizCompletedEventArgs e);
}
