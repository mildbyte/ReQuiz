using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReQuizServer
{
    /// <summary>
    /// Defines a common interface a ReQuiz question
    /// must implement server-side
    /// </summary>
    interface IQuizQuestion
    {
        /// <summary>
        /// Determines whether the answer given by the user is correct
        /// </summary>
        /// <param name="answer">Answer to check</param>
        bool MarkAnswer(string answer);

        /// <summary>
        /// Packs the question into one string, suitable for
        /// transferring by the server
        /// </summary>
        /// <returns>The packed question</returns>
        string ToRawFormat();
    }
}
