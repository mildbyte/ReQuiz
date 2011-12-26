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

            toRender.Controls.Add(lblQuestion);
            toRender.Controls.Add(txtQuestion);
            toRender.Controls.Add(txtAnswer);
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
            RegExp parsedExp = new RegExp(regExp);
            char[] randomAnswer = parsedExp.RandomString().ToCharArray();

            for (int i = 0; i < randomAnswer.Length; i += 2)
            {
                randomAnswer[i] = '_';
            }

            MessageBox.Show("Hint: a string that matches this expression is " + Environment.NewLine + new string(randomAnswer));
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
        }

        public ChooseMatchQuestion(string rawOptions)
        {
            hintAvailable = false;

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
