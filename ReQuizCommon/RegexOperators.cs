using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReQuizCommon
{
    /// <summary>
    /// Enumerates the possible types of metacharacters
    /// </summary>
    enum RegexOperatorType
    {
        Concatenate,
        Plus,
        Star,
        Question,
        LeftBracket,
        RightBracket,
        Alternate
    };


    /// <summary>
    /// Defines an interface for a regular expression operator
    /// </summary>
    interface IRegExpOperator
    {
        RegexOperatorType GetOperatorType();

        /// <summary>
        /// Gets the precedence of the operator
        /// </summary>
        /// <returns>The order of precedence of the operator, bigger will be executed first</returns>
        int GetPrecedence();

        /// <summary>
        /// Executes the operator on the stack of NFAs and puts the resultant NFA on top of the stack
        /// </summary>
        /// <param name="operandStack">The operand stack to execute the operator on</param>
        void Execute(Stack<NFAFragment> operandStack);
    };

    /// <summary>
    /// Implements the "|" operator
    /// </summary>
    class AlternateOperator : IRegExpOperator
    {
        public RegexOperatorType GetOperatorType() { return RegexOperatorType.Alternate; }

        /// <summary>
        /// Executes the "|" operator by creating a state from which we can 
        /// choose to move to either first NFA or the second
        /// </summary>
        /// <param name="operandStack">The operand stack to execute the operator on</param>
        public void Execute(Stack<NFAFragment> operandStack)
        {
            //Get the two operands
            NFAFragment nfa2 = operandStack.Pop();
            NFAFragment nfa1 = operandStack.Pop();

            //The new NFA fragment starts with a split state
            //The unbound states in it are those of both nfa1 and nfa2
            NFAFragment newNFA = new NFAFragment();
            newNFA.startState = new NFASplitState(nfa1.startState, nfa2.startState);
            newNFA.unboundStates = nfa1.unboundStates;
            newNFA.unboundStates.AddRange(nfa2.unboundStates);

            //Put the resultant NFA back
            operandStack.Push(newNFA);
        }

        public int GetPrecedence() { return 0; }
    }


    /// <summary>
    /// Implements the concatenation operator
    /// </summary>
    class ConcatenateOperator : IRegExpOperator
    {
        public RegexOperatorType GetOperatorType() { return RegexOperatorType.Concatenate; }

        /// <summary>
        /// Joins the two NFAs on the stack in series, so that the resultant 
        /// NFA accepts the input only if both NFAs it's made of accept it.
        /// </summary>
        /// <param name="operandStack">The operand stack to execute the operator on</param>
        public void Execute(Stack<NFAFragment> operandStack)
        {
            //Get the two operands
            NFAFragment nfa2 = operandStack.Pop();
            NFAFragment nfa1 = operandStack.Pop();

            //Connect all the unbound states (the dangling arrows)
            //in the first NFA to the second NFA's input
            nfa1.ConnectUnboundStates(nfa2.startState);

            //The unbound states in the resultant NFAs are all that of nfa2
            //because we have connected all the ones from nfa1
            nfa1.unboundStates = nfa2.unboundStates;

            //Push the resultant operand back onto the stack
            operandStack.Push(nfa1);
        }
        public int GetPrecedence() { return 1; }
    }


    /// <summary>
    /// Implements the "+" operator
    /// </summary>
    class PlusOperator : IRegExpOperator
    {
        public RegexOperatorType GetOperatorType() { return RegexOperatorType.Plus; }

        /// <summary>
        /// Executes the "+" operator by creating an NFA that consists of the original NFA, the outputs from which
        //  go to a split state which gives an option of going back to the same NFA.
        //  Result - accepts one or more of the given NFA.
        /// </summary>
        /// <param name="operandStack">The operand stack to execute the operator on</param>
        public void Execute(Stack<NFAFragment> operandStack)
        {


            //Get the needed NFA fragment
            NFAFragment nfa1 = operandStack.Pop();

            //Create the split state, with the possibility to return to the original NFA
            NFASplitState splitState = new NFASplitState(nfa1.startState, null);

            //Connect all exits in the original NFA to the created state
            nfa1.ConnectUnboundStates(splitState);

            //The only unbound state now is the split state
            nfa1.unboundStates.Clear();
            nfa1.unboundStates.Add(splitState);

            //Push the resultant NFA back
            operandStack.Push(nfa1);
        }
        public int GetPrecedence() { return 2; }
    }


    /// <summary>
    /// Implements the "?" operator
    /// </summary>
    class QuestionOperator : IRegExpOperator
    {
        public RegexOperatorType GetOperatorType() { return RegexOperatorType.Question; }

        /// <summary>
        /// Executes the "?" operator by creating a split state that allows to either
        /// go through the NFA or not.
        /// </summary>
        /// <param name="operandStack">The operand stack to execute the operator on</param>
        public void Execute(Stack<NFAFragment> operandStack)
        {
            //Pop the needed NFA
            NFAFragment nfa1 = operandStack.Pop();

            //Create the split state
            NFASplitState splitState = new NFASplitState(nfa1.startState, null);

            //The beginning of the NFA is now the split state
            nfa1.startState = splitState;

            //Add the split state to the list of unbound states
            nfa1.unboundStates.Add(splitState);

            //Push the resultant NFA into stack
            operandStack.Push(nfa1);
        }
        public int GetPrecedence() { return 2; }
    }


    /// <summary>
    /// Implements the "*" operator
    /// </summary>
    class StarOperator : IRegExpOperator
    {
        public RegexOperatorType GetOperatorType() { return RegexOperatorType.Star; }

        /// <summary>
        /// Executes the "*" operator by creating a split state which allows to either go through the NFA or continue
        /// The exits from the NFA will lead back to this state
        /// Result - zero or more of the NFA
        /// The only difference from the Plus is that the split is before the NFA.
        /// </summary>
        /// <param name="operandStack">The operand stack to execute the operator on</param>
        public void Execute(Stack<NFAFragment> operandStack)
        {
            //Pop the needed NFA from the stack
            NFAFragment nfa1 = operandStack.Pop();

            //Create the split state
            NFASplitState splitState = new NFASplitState(nfa1.startState, null);

            //Make the split state the beginning of the NFA
            nfa1.startState = splitState;

            //Connect the exits from the NFA with the split state
            nfa1.ConnectUnboundStates(splitState);

            //The only unbound state now is the split state
            nfa1.unboundStates.Clear();
            nfa1.unboundStates.Add(splitState);

            //Push the resultant NFA back
            operandStack.Push(nfa1);
        }
        public int GetPrecedence() { return 2; }
    }


    /// <summary>
    /// Placeholder for the "(" operator
    /// </summary>
    class LeftBracketOperator : IRegExpOperator
    {
        public RegexOperatorType GetOperatorType() { return RegexOperatorType.LeftBracket; }

        /// <summary>
        /// Dummy function, the behaviour of the left bracket is defined in the actual code
        /// for parsing the regex
        /// </summary>
        public void Execute(Stack<NFAFragment> operandStack)
        {
        }

        public int GetPrecedence() { return -1; }
    }


    /// <summary>
    /// Placeholder for the ")" operator
    /// </summary>
    class RightBracketOperator : IRegExpOperator
    {
        public RegexOperatorType GetOperatorType() { return RegexOperatorType.RightBracket; }

        /// <summary>
        /// Dummy function, the actual behaviour of the closing bracket is specified
        /// in the actual code for parsing the regex
        /// </summary>
        public void Execute(Stack<NFAFragment> operandStack)
        {
        }

        public int GetPrecedence() { return -1; }
    }


    /// <summary>
    /// Defines the behaviour of various metacharacters
    /// </summary>
    class RegexOperators
    {
        /// <summary>
        /// Checks whether the given character is alphanumeric
        /// </summary>
        /// <param name="chr">The character to check</param>
        /// <returns>True if the character is alphanumeric, False if it isn't</returns>
        private static bool IsAlphaNumeric(char chr)
        {
            if (chr >= 48 && chr <= 57) return true; //Digit
            if (chr >= 65 && chr <= 90) return true; //Uppercase letter
            if (chr >= 97 && chr <= 122) return true;//Lowercase letter

            return false;
        }

        /// <summary>
        /// Checks whether the given IRegExpOperator is a regexp quantifier
        /// </summary>
        /// <param name="op">Operator to check</param>
        public static bool IsQuantifier(IRegExpOperator op)
        {
            if (op.GetOperatorType() == RegexOperatorType.Plus
                || op.GetOperatorType() == RegexOperatorType.Question
                || op.GetOperatorType() == RegexOperatorType.Star
            ) return true;

            return false;
        }

        /// <summary>
        /// Checks whether the given character is a regexp quantifier
        /// </summary>
        /// <param name="chr">Character to check</param>
        public static bool IsQuantifier(char chr)
        {
            if (chr == '?' || chr == '+' || chr == '*') return true;
            else return false;
        }

        /// <summary>
        /// Checks whether the given character is a regexp operator
        /// </summary>
        /// <param name="chr">Character to check</param>
        public static bool IsOperator(char chr)
        {
            return (chr == '?' || chr == '+' || chr == '*' || chr == '('
                || chr == ')' || chr == '|' || chr == (char)1);
        }

        /// <summary>
        /// Generates a regexp operator from the given character
        /// </summary>
        /// <param name="op">Character to generate the operator from</param>
        /// <returns>The generated operator</returns>
        public static IRegExpOperator GenerateOperator(char op)
        {
            switch (op)
            {
                case '+': return new PlusOperator();
                case '?': return new QuestionOperator();
                case '*': return new StarOperator();
                case '|': return new AlternateOperator();
                case '(': return new LeftBracketOperator();
                case ')': return new RightBracketOperator();
                case (char)1: return new ConcatenateOperator();
                default: throw new Exception("invalid operator");
            }
        }
    }
}
