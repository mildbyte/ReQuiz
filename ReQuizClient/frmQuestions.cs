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
    /// <summary>
    /// Parses the questions, displays them to the user and receives his answers
    /// </summary>
    public partial class frmQuestions : Form
    {
        //The questions and rules for this quiz
        private QuizParameters quizParameters;

        //Parsed quiz questions
        private List<IQuizQuestion> quizQuestions;

        //Panel controls that contain the questions
        private List<Panel> renderedQuestions;

        //The ID of the question currently displayed
        private int currQuestionID;

        //Number of hints already used up
        private int hintsUsed;

        //Gets set to True after all questions have been parsed and displayed
        private bool startupSuccessful = false;
        
        /// <summary>
        /// Gets thrown when the user completes the quiz and asks to submit the questions
        /// </summary>
        public event QuizCompletedEventHandler QuizCompleted;

        public frmQuestions(QuizParameters quizParameters)
        {
            //Initialise the needed structures
            InitializeComponent();
            this.quizParameters = quizParameters;
            quizQuestions = new List<IQuizQuestion>();
            renderedQuestions = new List<Panel>();
            hintsUsed = 0;
            currQuestionID = 0;
        }

        /// <summary>
        /// Enables and disables the Previous/Next/Hint buttons and redisplays
        /// the number of hints available and the progress through the quiz
        /// </summary>
        private void UpdateStatusBar()
        {
            //Bring the hint button to the front in case it's hidden behind a panel
            btnHint.BringToFront();

            //Show the hint button if the user hasn't used a hint for this question
            //and hasn't used all available hints
            btnHint.Visible = (hintsUsed < quizParameters.hintsAllowed) 
                && (quizQuestions[currQuestionID].HintAvailable);

            //Enable the Next button if we haven't reached the last question
            btnNextQuestion.Enabled = currQuestionID < quizQuestions.Count - 1;

            //Enable the Prev button if we are not on the first question
            btnPrevQuestion.Enabled = currQuestionID > 0;

            //Show the number of hints available and progress through the quiz
            lblHints.Text = "Hints: " + (quizParameters.hintsAllowed - hintsUsed).ToString();
            lblQuestionNumber.Text = currQuestionID + 1 + "/" + quizQuestions.Count();

            //Focus on the control that would input the user's answer
            quizQuestions[currQuestionID].FocusOnAnswerField();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            //Confirm the user's intentions
            if (MessageBox.Show("You can submit your answers only once. Are you sure you want to submit your answers for marking?", "ReQuiz Client",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            //Get the user's answer to every question
            QuizAnswers quizAnswers;
            quizAnswers.answers = new List<string>();

            foreach (IQuizQuestion question in quizQuestions)
            {
                quizAnswers.answers.Add(question.GetAnswer());
            }

            quizAnswers.hintsUsed = hintsUsed;

            //Call the QuizCompleted event handler
            if (QuizCompleted != null)
            {
                QuizCompleted(this, new QuizCompletedEventArgs(quizAnswers));
            }
        }

        private void btnPrevQuestion_Click(object sender, EventArgs e)
        {
            //Hide the current question panel and show the previous question panel
            renderedQuestions[currQuestionID].Hide();
            if (currQuestionID != 0) currQuestionID--;
            renderedQuestions[currQuestionID].Show();

            //Update the interface
            UpdateStatusBar();
        }

        private void btnNextQuestion_Click(object sender, EventArgs e)
        {
            //Hide the current question panel and show the next one
            renderedQuestions[currQuestionID].Hide();
            if (currQuestionID != quizQuestions.Count - 1) currQuestionID++;
            renderedQuestions[currQuestionID].Show();

            //Update the interface
            UpdateStatusBar();
        }

        private void btnHint_Click(object sender, EventArgs e)
        {
            //Confirm the user's intentions
            DialogResult choice = MessageBox.Show("Using a hint will penalise you by 1 mark. Are you sure you want to use a hint?",
               "ReQuiz Client", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (choice == DialogResult.No) return;

            //Request the question to display a hint and update the interface
            quizQuestions[currQuestionID].DisplayHint();
            hintsUsed++;
            UpdateStatusBar();
        }

        private void frmQuestions_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Form failed to start normally, no need to ask the user
            if (!startupSuccessful) return;

            //Confirm the user's intentions
            DialogResult choice = MessageBox.Show("Are you sure you want to exit this quiz without submitting your answers?",
                "ReQuiz Client", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            e.Cancel = (choice == DialogResult.No);
        }

        private void frmQuestions_Load(object sender, EventArgs e)
        {
            //Fonts used to display the question and the answer are set up here to make
            //it easier to change the application style
            Font questionFont = new Font("Tahoma", 10F);
            Font answerFont = new Font("Tahoma", 10F);

            //Parse the raw questions and render them to the panels
            foreach (QuizQuestionRaw question in quizParameters.questions)
            {
                //Parse the question
                try
                {
                    IQuizQuestion parsedQuestion = RegexQuestions.CreateQuestion(question);

                    //Create a panel for it
                    Panel thisQuestionPanel = new Panel();
                    thisQuestionPanel.Parent = this;
                    thisQuestionPanel.Left = 0;
                    thisQuestionPanel.Width = this.ClientRectangle.Width;
                    thisQuestionPanel.Top = 0;
                    thisQuestionPanel.Height = this.Height - 20;
                    this.Controls.Add(thisQuestionPanel);

                    //Each question's panel is hidden in the beginning
                    thisQuestionPanel.Hide();

                    //Render the question to this panel and store it in the memory
                    parsedQuestion.Render(thisQuestionPanel, questionFont, answerFont);
                    quizQuestions.Add(parsedQuestion);
                    renderedQuestions.Add(thisQuestionPanel);
                }
                catch (UnknownQuestionTypeException)
                {
                    MessageBox.Show("Unsupported question detected among questions fetched from the server.",
                        "ReQuiz Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                    return;
                }
            }

            //Show the information about the quiz
            MessageBox.Show("This test has " + quizParameters.questions.Count + " questions." + Environment.NewLine +
                "You are allowed " + quizParameters.hintsAllowed + " hints." + Environment.NewLine +
                "Each question is worth 2 marks." + Environment.NewLine + 
                "Each hint deducts 1 mark from your total score.");


            //Show the form, the first question and the needed buttons
            this.Show();
            renderedQuestions[0].Show();
            UpdateStatusBar();

            //Focus on the first question's answer field
            quizQuestions[0].FocusOnAnswerField();

            //All the startup procedures were successful, set the flag
            startupSuccessful = true;

            ttfrmQuestions.SetToolTip(btnNextQuestion, "Advances to the next question of the quiz");
            ttfrmQuestions.SetToolTip(btnPrevQuestion, "Goes back to the previous question of the quiz");
            ttfrmQuestions.SetToolTip(btnHint, "Shows a hint to the question. The usage of a hint penalises you for 1 mark");
            ttfrmQuestions.SetToolTip(btnSubmit, "Submits the answers to the server. You can only submit the answers once.");
        }
    }


    /// <summary>
    /// Holds the parameters for the QuizCompleted event
    /// </summary>
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
