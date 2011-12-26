using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReQuizServer
{
    class RegexQuestions
    {
        private static RegexGen generator = new RegexGen();

        public static List<IQuizQuestion> GenerateQuestions(int amount)
        {
            Random randGen = new Random();
            List<IQuizQuestion> result = new List<IQuizQuestion>(amount);
            for (int i = 0; i < amount; i++)
            {
                switch (randGen.Next(3))
                {
                    case 0: result.Add(new MatchStringQuestion(generator)); break;
                    case 1: result.Add(new MatchStringQuestion(generator)); break;
                    case 2: result.Add(new MatchStringQuestion(generator)); break;
                }
            }

            return result;
        }
    }

    class MatchStringQuestion : IQuizQuestion
    {
        private string regex;
        private RegExp parsedRegex;

        public MatchStringQuestion(RegexGen generator)
        {
            regex = generator.NextExpression(10);
            parsedRegex = new RegExp(regex);
        }

        public bool MarkAnswer(string answer)
        {
            return (parsedRegex.Match(answer));
        }

        public override string ToString()
        {
            return ("WMATCH" + Environment.NewLine + regex);
        }
    }

    class ChooseMatchQuestion : IQuizQuestion
    {
        private string regex;
        private RegExp parsedRegex;
        private string[] options = new string[4];
        private int correctID;
        
        public ChooseMatchQuestion(RegexGen generator)
        {
            regex = generator.NextExpression(5);
            parsedRegex = new RegExp(regex);

            correctID = new Random().Next(4);
        }

        public bool MarkAnswer(string answer) 
        {
            return (int.Parse(answer) == correctID);
        }
    }
}
