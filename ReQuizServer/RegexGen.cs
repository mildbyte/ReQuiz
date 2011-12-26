using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReQuizServer
{
    class RegexGen
    {
        private Random m_randGen;

        public RegexGen() {
            m_randGen = new Random();
        }

        private char RandomCharacter()
        {
            //Generates a random alphanumeric character

            //There are 62 alphanumeric characters (26*2 letters + 10 digits)
            int code = m_randGen.Next(62);

            //code = 0..9 corresponds to a digit
            if (code < 10) return (char)(code + 48);

            //code = 10..35 becomes a capital letter (starts at 65, so ASCII = code - 10 + 65 = code + 55)
            if (code < 36) return (char)(code + 55);

            //36..61 => small letter
            return (char)(code + 61);
        }

        private char RandomQuantifier()
        {
            //Generates a random quantifier (appended after an expression to repeat it)
            //3 options: ?, *, +

            int type = m_randGen.Next(3);

            //Output the character based on the value of the random number
            switch (type)
            {
                case 0: return '?';
                case 1: return '*';
                default: return '+';
            }
        }

        private string GenerateCharacterSet(int length)
        {
            //Generates several alphanumeric characters with
            //randomly placed quantifiers

            string result = "";

            for (int i = 0; i < length; i++)
            {
                //Add a random character to the expression
                result += RandomCharacter();

                //With a probability of 10%, add a quantifier
                if (m_randGen.Next(10) == 0) result += RandomQuantifier();
            }

            return result;
        }

        private string GenerateAlternateGroups(int amount)
        {
            //Generates an expression of type (expression|expression...|expression)
            //Also can add quantifier to the result (probability 10%)
            //If only one expression in brackets, then the quantifier is certainly added

            string result = "(";

            //If only one expression in brackets, it should be longer than 1 and
            //the brackets should have a quantifier after them (otherwise, they
            //could have been left out.
            if (amount == 1)
            {
                result += GenerateCharacterSet(m_randGen.Next(2, 5));
                result += ")";
                result += RandomQuantifier();
                return result;
            }

            //For > 1 expressions in a group.
            
            //Each option has 1 to 4 alphanumeric characters (quantifiers not included)
            result += GenerateCharacterSet(m_randGen.Next(1, 4));

            for (int i = 1; i < amount; i++)
            {
                result += '|';
                result += GenerateCharacterSet(m_randGen.Next(1, 4));
            }

            result += ")";

            //Add a quantifier with 0.1 probability.
            if (m_randGen.Next(10) == 0) result += RandomQuantifier();

            return result;
        }

        public string NextExpression(int elementCount)
        {
            //Generates a random regular expression out of elementCount elements
            //one element - either a set of letters with random quantifiers
            //or a group of (setofletters|setofletters...)

            string newExpression = "";

        
            for (int i = 0; i < elementCount; i++)
            {
                //50/50 chance of the next element being a group or a simple set        
                if (m_randGen.Next(2) == 0)
                {
                    newExpression += GenerateCharacterSet(m_randGen.Next(1, 5));
                }
                else
                {
                    newExpression += GenerateAlternateGroups(m_randGen.Next(1, 6));
                }
            }
            return newExpression;
        }
    }
}
