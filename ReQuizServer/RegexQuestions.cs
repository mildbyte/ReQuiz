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
                    case 0: result.Add(new MatchStringQuestion(10)); break;
                    case 1: result.Add(new ChooseMatchQuestion(10)); break;
                    case 2: result.Add(new MatchStringQuestion(10)); break;
                }
            }

            return result;
        }
    }

    class MatchStringQuestion : IQuizQuestion
    {
        private string regex;
        private RegExp parsedRegex;

        public MatchStringQuestion(int elementNumber)
        {
            regex = RegexGen.GenerateExpression(elementNumber);
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
        private static Random randomGen = new Random();
        private string regex;
        private RegExp parsedRegex;
        private string[] options = new string[4];
        private int correctID;

        private string GenerateInvalidString(string validString)
        {
            char[] convertedString = validString.ToCharArray();
            string result;

            do
            {
                for (int i = 0; i < convertedString.Length; i++)
                {
                    if (randomGen.Next() % 3 == 0)
                    {
                        convertedString[i] = RegexGen.RandomCharacter();
                    }
                }
                result = convertedString.ToString();
            } while (parsedRegex.Match(result));

            return result;
        }
        
        public ChooseMatchQuestion(int elementNumber)
        {
            regex = RegexGen.GenerateExpression(elementNumber);
            parsedRegex = new RegExp(regex);

            correctID = randomGen.Next(4);

            for (int i = 0; i < 3; i++)
            {
                string matchString = parsedRegex.RandomString();
                if (i == correctID) options[i] = matchString;
                else options[i] = GenerateInvalidString(matchString);
            }
        }

        public bool MarkAnswer(string answer) 
        {
            return (int.Parse(answer) == correctID);
        }

        public override string ToString()
        {
            string result = "";
            for (int i = 0; i < 3; i++)
            {
                result += options[i] + ";";
            }

            return result;
        }
    }
}
