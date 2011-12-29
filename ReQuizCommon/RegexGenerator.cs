using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReQuizCommon
{
    /// <summary>
    /// Generates a random regular expression
    /// </summary>
    public class RegexGenerator
    {
        //Random number generator
        static private Random randGen = new Random();

        /// <summary>
        /// Generates a random alphanumeric character
        /// </summary>
        public static char RandomCharacter()
        {
            //There are 62 alphanumeric characters (26*2 letters + 10 digits)
            int code = randGen.Next(62);

            //code = 0..9 corresponds to a digit
            if (code < 10) return (char)(code + 48);

            //code = 10..35 becomes a capital letter (starts at 65, so ASCII = code - 10 + 65 = code + 55)
            if (code < 36) return (char)(code + 55);

            //36..61 => small letter
            return (char)(code + 61);
        }

        /// <summary>
        /// Generates a random quantifier (appended after an expression to repeat it)
        /// </summary>
        /// <returns>Either ?, * or +</returns>
        private static char RandomQuantifier()
        {
            int type = randGen.Next(3);

            //Output the character based on the value of the random number
            switch (type)
            {
                case 0: return '?';
                case 1: return '*';
                default: return '+';
            }
        }

        /// <summary>
        /// Generates several alphanumeric characters with randomly placed quantifiers
        /// </summary>
        /// <param name="length">Length of the generated string</param>
        private static string GenerateCharacterSet(int length)
        {
            string result = "";

            for (int i = 0; i < length; i++)
            {
                //Add a random character to the expression
                result += RandomCharacter();

                //With a probability of 10%, add a quantifier
                if (randGen.Next(10) == 0) result += RandomQuantifier();
            }

            return result;
        }

        /// <summary>
        /// Generates an expression of type (expression|expression...|expression)
        /// Also can add quantifier to the result (probability 10%)
        /// If only one expression in brackets, then the quantifier is certainly added
        /// </summary>
        /// <param name="amount">Amount of expressions inside the group</param>
        private static string GenerateAlternateGroups(int amount)
        {
            string result = "(";

            //If only one expression in brackets, it should be longer than 1 and
            //the brackets should have a quantifier after them (otherwise, they
            //could have been left out).
            if (amount == 1)
            {
                result += GenerateCharacterSet(randGen.Next(2, 5));
                result += ")";
                result += RandomQuantifier();
                return result;
            }

            //For > 1 expressions in a group.
            
            //Each option has 1 to 4 alphanumeric characters (quantifiers not included)
            result += GenerateCharacterSet(randGen.Next(1, 4));

            for (int i = 1; i < amount; i++)
            {
                result += '|';
                result += GenerateCharacterSet(randGen.Next(1, 4));
            }

            result += ")";

            //Add a quantifier with 0.1 probability.
            if (randGen.Next(10) == 0) result += RandomQuantifier();

            return result;
        }

        /// <summary>
        /// Generates a random regular expression out of elementNumber elements.
        /// One element - either a set of letters with random quantifiers
        /// or a group of (setofletters|setofletters...)
        /// </summary>
        /// <param name="elementNumber">Number of elements in the generated expression</param>
        /// <returns></returns>
        public static string GenerateExpression(int elementNumber)
        {
            string newExpression = "";

            for (int i = 0; i < elementNumber; i++)
            {
                //50/50 chance of the next element being a group or a simple set        
                if (randGen.Next(2) == 0)
                {
                    newExpression += GenerateCharacterSet(randGen.Next(1, 5));
                }
                else
                {
                    newExpression += GenerateAlternateGroups(randGen.Next(1, 6));
                }
            }
            return newExpression;
        }
    }
}
