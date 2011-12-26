using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;


namespace ReQuizClient
{
    /// <summary>
    /// Unparsed quiz question fetched from the server
    /// </summary>
    public struct QuizQuestionRaw
    {
        public string type;
        public string parameters;
    }


    /// <summary>
    /// Set of parameters for the quiz fetched from the server
    /// </summary>
    public struct QuizParameters
    {
        public int hintsAllowed;
        public List<QuizQuestionRaw> questions;
    }


    /// <summary>
    /// Holds the data about the answers to the quiz from the user
    /// </summary>
    public struct QuizAnswers
    {
        public int hintsUsed;
        public List<string> answers;
    }


    /// <summary>
    /// Scores for this quiz as received from the server
    /// </summary>
    public struct QuizResult
    {
        public bool answersAccepted;    //If false, means that the server has rejected our answers
        public int mark;
        public int correctAnswers;
    }

    /// <summary>
    /// Provides the common interface for a parsed quiz question
    /// </summary>
    interface IQuizQuestion
    {
        /// <summary>
        /// Renders the question and its options to the given Panel control
        /// </summary>
        /// <param name="toRender">Panel control to render the question to</param>
        /// <param name="questionFont">Font used to render the actual question text</param>
        /// <param name="answerFont">Font used to render the answer text</param>
        void Render(Panel toRender, Font questionFont, Font answerFont);

        /// <summary>
        /// Formats the answer received from the user in a format markable by the server
        /// </summary>
        /// <returns>The formatted answer</returns>
        string GetAnswer();

        /// <summary>
        /// Specifies whether a hint to this question is available
        /// </summary>
        bool HintAvailable();

        /// <summary>
        /// Shows a hint to this question to the user
        /// </summary>
        void DisplayHint();
    }
}