using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace ReQuizClient
{
    /// <summary>
    /// Defines a ReQuiz parser for regular expression questions
    /// </summary>
    class RegexQuestions
    {
        /// <summary>
        /// Detects the type of the raw ReQuiz question and creates a corresponding parsed question object
        /// </summary>
        /// <param name="question">Raw quiz question to parse</param>
        /// <returns>Parsed ReQuiz question</returns>
        public static IQuizQuestion CreateQuestion(QuizQuestionRaw question)
        {
            if (question.type == "WMATCH")
            {
                return new MatchStringQuestion(question.parameters);
            }
            else
            {
                return new ChooseMatchQuestion(question.parameters);
            }
        }
    }


    /// <summary>
    /// A "write a string that matches the given expression" question
    /// </summary>
    class MatchStringQuestion : IQuizQuestion
    {
        //Random number generator
        Random randGen = new Random();

        //Regular expression in question
        private string questionRegex;

        //Has the user used the hint?
        private bool hintAvailable;

        //The textbox in which the hint is stored
        private TextBox txtHint;

        //The textbox with the answer specified by the user
        private TextBox txtAnswer;

        /// <summary>
        /// Specifies whether a hint to this question is available
        /// </summary>
        public bool HintAvailable
        {
            get
            {
                return hintAvailable;
            }
        }

        /// <summary>
        /// Renders the question and its options to the given Panel control
        /// </summary>
        /// <param name="toRender">Panel control to render the question to</param>
        /// <param name="questionFont">Font used to render the actual question text</param>
        /// <param name="answerFont">Font used to render the answer text</param>
        public void Render(Panel toRender, Font questionFont, Font answerFont) {
            //Create the label that will host the question text
            Label lblQuestion = new Label();
            lblQuestion.Parent = toRender;
            lblQuestion.Text = "Enter a string that matches the following regular expression: ";
            lblQuestion.Top = 10;
            lblQuestion.Width = toRender.Width;
            lblQuestion.Height = 20;
            lblQuestion.TextAlign = ContentAlignment.MiddleCenter;
            lblQuestion.Font = questionFont;
            lblQuestion.AutoSize = true;
            lblQuestion.Left = (toRender.Width - lblQuestion.Width) / 2;
            
            //Textbox that hosts the regular expression
            TextBox txtQuestion = new TextBox();
            txtQuestion.Parent = toRender;
            txtQuestion.Left = 10;
            txtQuestion.Font = questionFont;
            txtQuestion.TextAlign = HorizontalAlignment.Center;
            txtQuestion.Height = (int)questionFont.Size;
            txtQuestion.ReadOnly = true;
            txtQuestion.Text = questionRegex;
            txtQuestion.Top = lblQuestion.Bottom + 5;
            txtQuestion.Width = toRender.Width - 20;

            //Textbox into which the answer would be entered
            TextBox txtAnswer = new TextBox();
            txtAnswer.Top = txtQuestion.Bottom + 5;
            txtAnswer.Left = 10;
            txtAnswer.Width = toRender.Width - 20;
            txtAnswer.Parent = toRender;
            txtAnswer.Font = answerFont;
            txtAnswer.Height = (int)answerFont.Size;

            //Textbox that contains the hint to the question
            txtHint = new TextBox();
            txtHint.Parent = toRender;
            txtHint.Left = 10;
            txtHint.Width = toRender.Width - 20;
            txtHint.ReadOnly = true;
            txtHint.Top = txtAnswer.Bottom + 5;
            txtHint.TextAlign = HorizontalAlignment.Center;
            txtHint.Font = answerFont;
            txtHint.Height = (int)answerFont.Size;
            txtHint.Visible = false;

            //Add the controls to the question panel
            toRender.Controls.Add(lblQuestion);
            toRender.Controls.Add(txtQuestion);
            toRender.Controls.Add(txtAnswer);
            toRender.Controls.Add(txtHint);
        }

        /// <summary>
        /// Formats the answer received from the user in a format markable by the server
        /// </summary>
        /// <returns>The formatted answer</returns>
        public string GetAnswer() {
            return txtAnswer.Text.Trim();
        }

        /// <summary>
        /// Initialises the question
        /// </summary>
        /// <param name="regExp">Regular expression to base the question on</param>
        public MatchStringQuestion(string regExp)
        {
            this.questionRegex = regExp;
            hintAvailable = true;
        }

        /// <summary>
        /// Shows a hint to this question to the user
        /// </summary>
        public void DisplayHint()
        {
            //Bail out if the hint has already been displayed for this question
            if (!hintAvailable) return;

            //Hint no longer available
            hintAvailable = false;

            //Get a random answer that matches this regex
            RegExp parsedExp = new RegExp(questionRegex);
            char[] randomAnswer = parsedExp.RandomString().ToCharArray();

            //Replace some characters in it with underscores
            for (int i = 0; i < randomAnswer.Length; i++)
            {
                if (randGen.Next() % 3 == 0) randomAnswer[i] = '_';
            }

            //Display the hint
            txtHint.Text = "Hint: " + new string(randomAnswer);
            txtHint.Show();
        }
    }


    /// <summary>
    /// A "choose a string that matches the given regex" question
    /// </summary>
    class ChooseMatchQuestion : IQuizQuestion
    {
        //Random number generator
        private static Random randGen = new Random();

        //Available options
        private string[] options;

        //The regular expression used in the question
        private string questionRegex;

        //Is the hint available?
        private bool hintAvailable;

        //Radiobuttons that represent the possible answers
        private RadioButton[] displayedOptions;

        /// <summary>
        /// Specifies whether a hint to this question is available
        /// </summary>
        public bool HintAvailable {
            get {
                return hintAvailable;
            }
        }

        /// <summary>
        /// Renders the question and its options to the given Panel control
        /// </summary>
        /// <param name="toRender">Panel control to render the question to</param>
        /// <param name="questionFont">Font used to render the actual question text</param>
        /// <param name="answerFont">Font used to render the answer text</param>
        public void Render(Panel toRender, Font questionFont, Font answerFont)
        {
            //Label with the question text
            Label lblQuestion = new Label();
            lblQuestion.Parent = toRender;
            lblQuestion.Text = "Choose a string that matches the following regular expression: ";
            lblQuestion.Top = 10;
            lblQuestion.Width = toRender.Width;
            lblQuestion.Height = 20;
            lblQuestion.TextAlign = ContentAlignment.MiddleCenter;
            lblQuestion.Font = questionFont;
            lblQuestion.AutoSize = true;
            lblQuestion.Left = (toRender.Width - lblQuestion.Width) / 2;

            //Label with the regex
            TextBox txtQuestion = new TextBox();
            txtQuestion.Parent = toRender;
            txtQuestion.Left = 10;
            txtQuestion.Font = questionFont;
            txtQuestion.TextAlign = HorizontalAlignment.Center;
            txtQuestion.Height = (int)questionFont.Size;
            txtQuestion.ReadOnly = true;
            txtQuestion.Text = questionRegex;
            txtQuestion.Top = lblQuestion.Bottom + 5;
            txtQuestion.Width = toRender.Width - 20;

            //Array of the displayed possible answers
            displayedOptions = new RadioButton[4];
            for (int i = 0; i < 4; i++)
            {
                //Create the button
                RadioButton newOptionButton = new RadioButton();
                newOptionButton.Parent = toRender;
                newOptionButton.Text = options[i];
                newOptionButton.Font = answerFont;
                newOptionButton.Top = txtQuestion.Bottom + 5 + i * (int)answerFont.Size * 2;
                newOptionButton.Left = 10;

                //Store the created button and place it onto the panel
                displayedOptions[i] = newOptionButton;
                toRender.Controls.Add(newOptionButton);
            }

            //Place the question text on the panel
            toRender.Controls.Add(lblQuestion);
            toRender.Controls.Add(txtQuestion);
        }

        /// <summary>
        /// Formats the answer received from the user in a format markable by the server
        /// </summary>
        /// <returns>The formatted answer</returns>
        public string GetAnswer()
        {
            //Find the radiobutton that is selected
            for (int chosenID = 0; chosenID < 4; chosenID++)
            {
                if (displayedOptions[chosenID].Checked) return chosenID.ToString();
            }

            //If no buttons selected, return a deliberately incorrect value
            return "-1";
        }

        /// <summary>
        /// Shows a hint to this question to the user
        /// </summary>
        public void DisplayHint()
        {
            //Exit if the hint has been used
            if (!hintAvailable) return;

            //Mark the hint as used
            hintAvailable = false;

            //Find the correct option
            RegExp parsedQuestionRegex = new RegExp(questionRegex);
            int correctOption = 0;
            while (!parsedQuestionRegex.Match(options[correctOption]))
            {
                correctOption++;
            }

            //Generate two different IDs so that none of them is the correct ID
            int toRemove1, toRemove2;
            do {
                toRemove1 = randGen.Next(4);
            } while (toRemove1 == correctOption);
            do { 
                toRemove2 = randGen.Next(4);
            } while (toRemove2 == correctOption || toRemove2 == toRemove1);

            //Remove the two incorrect options
            displayedOptions[toRemove1].Hide();
            displayedOptions[toRemove2].Hide();

            displayedOptions[toRemove1].Checked = false;
            displayedOptions[toRemove2].Checked = false;
        }

        /// <summary>
        /// Initializes the question
        /// </summary>
        /// <param name="rawOptions">The unparsed question options line</param>
        public ChooseMatchQuestion(string rawOptions)
        {
            hintAvailable = true;
            options = new string[4];

            //Split the line into 
            string[] splitOptions = rawOptions.Split(';');
            
            //The first element in the resultant array is the regular expression
            questionRegex = splitOptions[0];

            //The other four are the actual options
            for (int i = 0; i < 4; i++)
            {
                options[i] = splitOptions[i + 1];
            }
        }
    }
}
