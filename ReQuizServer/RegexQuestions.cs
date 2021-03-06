﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReQuizCommon;

namespace ReQuizServer
{
    /// <summary>
    /// Defines how regular expression questions are generated
    /// </summary>
    class RegexQuestions
    {
        //Random generators
        private static RegexGenerator generator = new RegexGenerator();
        private static Random randGen = new Random();

        /// <summary>
        /// Generates a given amount of random questions
        /// </summary>
        /// <param name="amount">Amount of questions to generate</param>
        /// <param name="minLength">Minimum amount of elements in the regular expression</param>
        /// <param name="maxLength">Maximum amount of elements in the regular expression</param>
        /// <returns>A list of generated questions</returns>
        public static List<IQuizQuestion> GenerateQuestions(int amount, int minLength, int maxLength)
        {
            List<IQuizQuestion> result = new List<IQuizQuestion>(amount);

            for (int i = 0; i < amount; i++)
            {
                //Pick a random question type and generate the needed question
                switch (randGen.Next(2))
                {
                    case 0: result.Add(new MatchStringQuestion(randGen.Next(minLength, maxLength + 1))); break;
                    case 1: result.Add(new ChooseMatchQuestion(randGen.Next(minLength, maxLength + 1))); break;
                    //case 2: result.Add(new MatchStringQuestion(3)); break;
                }
            }
            return result;
        }
    }


    /// <summary>
    /// Defines a "Given a regular expression, write a string that matches it"
    /// question
    /// </summary>
    class MatchStringQuestion : IQuizQuestion
    {
        //Regular expression used in this question
        private string regex;
        private Regex parsedRegex;

        /// <summary>
        /// Generates a "write matching string" question
        /// </summary>
        /// <param name="elementNumber">Amount of the elements in the generated
        /// regular expression</param>
        public MatchStringQuestion(int elementNumber)
        { 
            //Generate a random regular expression and parse it
            regex = RegexGenerator.GenerateExpression(elementNumber);
            parsedRegex = new Regex(regex);
        }
 
        /// <summary>
        /// Determines whether the answer given by the user is correct
        /// </summary>
        /// <param name="answer">Answer to check</param>
        public bool MarkAnswer(string answer)
        {
            return (parsedRegex.Match(answer));
        }

        /// <summary>
        /// Packs the question into one string, suitable for
        /// transferring by the server
        /// </summary>
        /// <returns>The packed question</returns>
        public string ToRawFormat()
        {
            //Return the question code and the actual expression, separated by a new line
            return ("WMATCH" + Environment.NewLine + regex);
        }
    }


    /// <summary>
    /// Defines a "given a question and 4 strings, choose the one that matches it"
    /// question
    /// </summary>
    class ChooseMatchQuestion : IQuizQuestion
    {
        //The random generator
        private static Random randomGen = new Random();

        //Regular expression used by the question
        private string regex;
        private Regex parsedRegex;

        //The 4 strings
        private string[] options = new string[4];

        //The ID of the correct string
        private int correctID;

        /// <summary>
        /// Generates a string that does not match the regular expression
        /// but looks similar to a matching string
        /// </summary>
        /// <param name="validString">A string that matches the expression</param>
        /// <returns>Unmatching string</returns>
        private string GenerateInvalidString(string validString)
        {
            //Convert the string into a char[] array for character manipulation
            char[] convertedString = validString.ToCharArray();
            string result;

            do
            {
                //Replace characters in random positions in the given string
                //with other random characters
                for (int i = 0; i < convertedString.Length; i++)
                {
                    if (randomGen.Next() % 3 == 0)
                    {
                        convertedString[i] = RegexGenerator.RandomCharacter();
                    }
                }
                result = new string(convertedString);
            
                //Repeat in case the invalidated string actually matches the expression
            } while (parsedRegex.Match(result));

            return result;
        }


        /// <summary>
        /// Generates a "choose matching string" question
        /// </summary>
        /// <param name="elementNumber">Amount of the elements in the generated
        /// regular expression</param>
        public ChooseMatchQuestion(int elementNumber)
        {
            //Generate the required regular expression
            regex = RegexGenerator.GenerateExpression(elementNumber);
            parsedRegex = new Regex(regex);

            //Pick which string will be the correct one
            correctID = randomGen.Next(4);

            for (int i = 0; i < 4; i++)
            {
                //Generate a random string that matches the expression
                string matchString = parsedRegex.RandomString();

                //If this string is not supposed to be correct, make it invalid
                if (i == correctID) options[i] = matchString;
                else options[i] = GenerateInvalidString(matchString);

                //This way, each string will be generated from a different string
                //that matches the expression, hence, the 4 strings won't look similar
            }
        }


        /// <summary>
        /// Determines whether the answer given by the user is correct
        /// </summary>
        /// <param name="answer">Answer to check</param>
        public bool MarkAnswer(string answer) 
        {
            try
            {
                return (int.Parse(answer) == correctID);
            }
            catch (FormatException ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Packs the question into one string, suitable for
        /// transferring by the server
        /// </summary>
        /// <returns>The packed question</returns>
        public string ToRawFormat()
        {
            //Pack the 4 options into one line, separated by ";"
            string result = "";
            for (int i = 0; i < 4; i++)
            {
                result += options[i] + ";";
            }

            //Return the question code, the regex and the resultant options
            return "CMATCH" + Environment.NewLine + regex + ';' + result;
        }
    }
}
