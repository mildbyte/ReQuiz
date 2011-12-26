using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;


namespace ReQuizClient
{
    public struct QuizQuestionRaw
    {
        public string type;
        public string parameters;
    }

    public struct QuizParameters
    {
        public int hintsAllowed;
        public List<QuizQuestionRaw> questions;
    }

    public struct QuizAnswers
    {
        public int hintsUsed;
        public List<string> answers;
    }

    public struct QuizResult
    {
        public bool answersAccepted;
        public int mark;
        public int correctAnswers;
    }

    interface IQuizQuestion
    {
        void Render(Panel toRender, Font questionFont, Font answerFont);
        string GetAnswer();
        bool HintAvailable();
        void DisplayHint();
    }
}