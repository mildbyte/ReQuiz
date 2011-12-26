using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace ReQuizClient
{
    class RegexQuestions
    {
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

    class MatchStringQuestion : IQuizQuestion
    {
        private string answer;
        private string regExp;
        private bool hintAvailable;

        private TextBox hintTextBox;

        private void OnAnswerTextChanged(object sender, EventArgs e)
        {
            answer = ((TextBox)sender).Text;
        }

        public void Render(Panel toRender, Font questionFont, Font answerFont) {
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
            
            TextBox txtQuestion = new TextBox();
            txtQuestion.Parent = toRender;
            txtQuestion.Left = 10;
            txtQuestion.Font = questionFont;
            txtQuestion.TextAlign = HorizontalAlignment.Center;
            txtQuestion.Height = (int)questionFont.Size;
            txtQuestion.ReadOnly = true;
            txtQuestion.Text = regExp;
            txtQuestion.Top = lblQuestion.Bottom + 5;
            txtQuestion.Width = toRender.Width - 20;

            TextBox txtAnswer = new TextBox();
            txtAnswer.Top = txtQuestion.Bottom + 5;
            txtAnswer.Left = 10;
            txtAnswer.Width = toRender.Width - 20;
            txtAnswer.Parent = toRender;
            txtAnswer.TextChanged += OnAnswerTextChanged;
            txtAnswer.Font = answerFont;
            txtAnswer.Height = (int)answerFont.Size;

            TextBox txtHint = new TextBox();
            txtHint.Parent = toRender;
            txtHint.Left = 10;
            txtHint.Width = toRender.Width - 20;
            txtHint.ReadOnly = true;
            txtHint.Top = txtAnswer.Bottom + 5;
            txtHint.TextAlign = HorizontalAlignment.Center;
            txtHint.Font = answerFont;
            txtHint.Height = (int)answerFont.Size;
            txtHint.Visible = false;

            hintTextBox = txtHint;

            toRender.Controls.Add(lblQuestion);
            toRender.Controls.Add(txtQuestion);
            toRender.Controls.Add(txtAnswer);
            toRender.Controls.Add(txtHint);
        }

        public string GetAnswer() {
            return answer;
        }

        public MatchStringQuestion(string regExp)
        {
            this.regExp = regExp;
            hintAvailable = true;
        }

        public bool HintAvailable()
        {
            return hintAvailable;
        }

        public void DisplayHint()
        {
            if (!hintAvailable) return;
            hintAvailable = false;

            Random randGen = new Random();

            RegExp parsedExp = new RegExp(regExp);
            char[] randomAnswer = parsedExp.RandomString().ToCharArray();

            for (int i = 0; i < randomAnswer.Length; i++)
            {
                if (randGen.Next() % 3 == 0) randomAnswer[i] = '_';
            }

            hintTextBox.Text = "Hint: " + new string(randomAnswer);
            hintTextBox.Show();
        }
    }

    class ChooseMatchQuestion : IQuizQuestion
    {
        private string[] options;
        private string questionRegex;
        private int correctOption;
        private bool hintAvailable;

        private RadioButton[] displayedOptions;

        public void Render(Panel toRender, Font questionFont, Font answerFont)
        {
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

            displayedOptions = new RadioButton[4];
            for (int i = 0; i < 4; i++)
            {
                RadioButton newOptionButton = new RadioButton();
                newOptionButton.Parent = toRender;
                newOptionButton.Text = options[i];
                newOptionButton.Font = answerFont;
                newOptionButton.Top = txtQuestion.Bottom + 5 + i * (int)answerFont.Size * 2;
                newOptionButton.Left = 10;
                displayedOptions[i] = newOptionButton;
                toRender.Controls.Add(newOptionButton);
            }

            toRender.Controls.Add(lblQuestion);
            toRender.Controls.Add(txtQuestion);
        }

        public string GetAnswer()
        {
            for (int chosenID = 0; chosenID < 4; chosenID++)
            {
                if (displayedOptions[chosenID].Checked) return chosenID.ToString();
            }
            return "-1";
        }

        public bool HintAvailable()
        {
            return hintAvailable;
        }

        public void DisplayHint()
        {
            hintAvailable = false;

            Random randGen = new Random();
            int toRemove1, toRemove2;
            do {
                toRemove1 = randGen.Next(4);
            } while (toRemove1 == correctOption);
            do { 
                toRemove2 = randGen.Next(4);
            } while (toRemove2 == correctOption || toRemove2 == toRemove1);

            displayedOptions[toRemove1].Hide();
            displayedOptions[toRemove2].Hide();

            displayedOptions[toRemove1].Checked = false;
            displayedOptions[toRemove2].Checked = false;
        }

        public ChooseMatchQuestion(string rawOptions)
        {
            hintAvailable = true;

            string[] splitOptions = rawOptions.Split(';');
            options = new string[4];

            questionRegex = splitOptions[0];
            for (int i = 0; i < 4; i++)
            {
                options[i] = splitOptions[i + 1];
            }

            RegExp parsedQuestionRegex = new RegExp(questionRegex);

            correctOption = 0;
            while (!parsedQuestionRegex.Match(options[correctOption]))
            {
                correctOption++;
            }
        }
    }
}
