using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReQuizCommon
{
    /// <summary>
    /// Enumerates the possible types of a non-deterministic finite automaton state
    /// </summary>
    public enum NFAStateType
    {
        CharSet,
        Split,
        Match
    };


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
    /// Provides a common interface an NFA used in Thompson's algorithm must implement
    /// </summary>
    public interface INFAState
    {
        NFAStateType GetNFAType();

        /// <summary>
        /// Connects the output of the current NFA to the input of the specified NFA
        /// </summary>
        /// <param name="toConnect">The NFA to connect to</param>
        void Connect(INFAState toConnect);
    };


    /// <summary>
    /// An NFA that only accepts the characters from a given character class
    /// </summary>
    class NFACharacterSet : INFAState
    {
        //Array of characters this NFA accepts
        private bool[] characters;

        //The state this NFA connects to
        public INFAState nextState;

        /// <summary>
        /// Generates a random character that would be accepted by this NFA
        /// </summary>
        public char RandomCharacter()
        {
            //TODO: implement randomness (only the first character is returned for now)
            char i = (char)0;
            while (i < 256 && !characters[i]) i++;

            return i;
        }

        /// <summary>
        /// Determines whether this NFA accepts the given character
        /// </summary>
        /// <param name="chr">The character to check</param>
        public bool Accepts(char chr)
        {
            return characters[chr];
        }

        /// <summary>
        /// Constructs a character-set NFA from a given character group
        /// </summary>
        /// <param name="character">A character group (bracketless) to construct an NFA from</param>
        public NFACharacterSet(string character)
        {
            //Initialize the parameters
            nextState = null;
            characters = new bool[256];

            int currPos = 0;
            char currChar = ' ';

            while (currPos < character.Length)
            {
                if (character[currPos] == '-')
                {
                    if (currChar < character[currPos + 1])
                    {
                        for (char i = currChar; i < character[currPos + 1]; i++)
                        {
                            characters[i] = true;
                        }
                    }
                }
                else
                {
                    currChar = character[currPos];
                    characters[currChar] = true;
                    currPos++;
                }
            }
        }

        public NFAStateType GetNFAType()
        {
            return NFAStateType.CharSet;
        }

        /// <summary>
        /// Connects the output of the current NFA to the input of the specified NFA
        /// </summary>
        /// <param name="toConnect">The NFA to connect to</param>
        public void Connect(INFAState toConnect)
        {
            if (nextState == null) nextState = toConnect;
        }
    };

    /// <summary>
    /// Defines a NFA that has two different outputs based on what input it receives
    /// </summary>
    class NFASplitState : INFAState
    {
        //The two outputs of the NFA
        public INFAState nextState1, nextState2;

        /// <summary>
        /// Connects the output of the current NFA to the input of the specified NFA
        /// </summary>
        /// <param name="toConnect">The NFA to connect to</param>
        public void Connect(INFAState toConnect)
        {
            if (nextState1 == null) nextState1 = toConnect;
            if (nextState2 == null) nextState2 = toConnect;
        }

        public NFAStateType GetNFAType()
        {
            return NFAStateType.Split;
        }

        /// <summary>
        /// Constructs an empty split NFA
        /// </summary>
        public NFASplitState()
        {
            nextState1 = null;
            nextState2 = null;
        }

        /// <summary>
        /// Constructs a split NFA state with given outputs
        /// </summary>
        /// <param name="next1">The first output of the NFA</param>
        /// <param name="next2">The second output of the NFA</param>
        public NFASplitState(INFAState next1, INFAState next2)
        {
            nextState1 = next1;
            nextState2 = next2;
        }
    };


    /// <summary>
    /// Defines the final state of an NFA. If this state is reached, a given string matches this regexp
    /// </summary>
    class NFAMatchState : INFAState
    {
        public NFAStateType GetNFAType() { return NFAStateType.Match; }

        /// <summary>
        /// Dummy method used to make NFAMatchState conform to the INFAState interface
        /// </summary>
        /// <param name="toConnect"></param>
        public void Connect(INFAState toConnect)
        {
        }
    };


    /// <summary>
    /// A non-deterministic automaton (NFA)
    /// </summary>
    public class NFAFragment
    {
        //State this NFA starts from
        public INFAState startState;

        //All states which lead nowhere (unbound)
        public List<INFAState> unboundStates;

        /// <summary>
        /// Connects all unbound states of this NFA to the given state
        /// </summary>
        /// <param name="state">State to connect this NFA to</param>
        public void ConnectUnboundStates(INFAState state)
        {
            foreach (INFAState currState in unboundStates)
            {
                currState.Connect(state);
            }
        }

        /// <summary>
        /// Creates an empty NFA fragment
        /// </summary>
        public NFAFragment()
        {
            startState = null;
            unboundStates = new List<INFAState>();
        }

        /// <summary>
        /// Creates an NFA fragment from a given state
        /// </summary>
        /// <param name="state"></param>
        public NFAFragment(INFAState state)
        {
            startState = state;
            unboundStates = new List<INFAState>();
            unboundStates.Add(state);
        }
    };
    
    
    /// <summary>
    /// Builds an NFA from a given regular expression
    /// </summary>
    public class NFABuilder
    {
        //Stack of the currently used operators
        private static Stack<IRegExpOperator> operatorStack = new Stack<IRegExpOperator>();

        //Stack of the currently used NFAs
        private static Stack<NFAFragment> operandStack = new Stack<NFAFragment>();


        /// <summary>
        /// Preprocesses the regular expression by inserting explicit
        /// concatenation operators (code 1)
        /// between (AN means alphanumeric character):
        /// AN + AN, AN + opening bracket, closing bracket + AN
        /// quantifier + AN, quantifier + opening bracket, closing bracket + opening bracket
        /// Also surrounds the expression with brackets (to eliminate redundant code later)
        /// </summary>
        /// <param name="regExp">The regular expression to preprocess</param>
        /// <returns>The preprocessed regular expression</returns>
        private static string Preprocess(string regExp)
        {
            //TODO: character group support - fix or eliminate completely?

            //Traverse the original expression
            //Look at the current and the next character, copy the current into the new string
            //If we need to insert a concat operator between them, add the operator into the new string
            string result = "(";

            //bool insideGroup = false;
            
            for (int i = 0; i < regExp.Length - 1; i++)
            {
                result += regExp[i];

                if (
                      (!RegexOperators.IsOperator(regExp[i]) && !RegexOperators.IsOperator(regExp[i + 1]))
                   || (!RegexOperators.IsOperator(regExp[i]) && regExp[i + 1] == '(')
                   || (regExp[i] == ')' && !RegexOperators.IsOperator(regExp[i + 1]))
                   || (RegexOperators.IsQuantifier(regExp[i]) && !RegexOperators.IsOperator(regExp[i + 1]))
                   || (RegexOperators.IsQuantifier(regExp[i]) && regExp[i + 1] == '(')
                   || (regExp[i] == ')' && regExp[i + 1] == '(')
                    ) result += (char)1;
            }
            //Add the last character of the regexp to the result
            result += regExp[regExp.Length - 1] + ")";

            return result;
        }


        /// <summary>
        /// Implements the Thompson's Algorithm
        /// Creates an NFA from the given (preprocessed) regular expression using a stack
        /// Assumes that the input is a valid regular expression.
        /// </summary>
        /// <param name="regExp">The regular expression to parse</param>
        public static NFAFragment CreateNFA(string regExp)
        {
            //Preprocess the expression
            regExp = Preprocess(regExp);

            //Traverse the expression character-by-character
            //Expression already set up so that one character is one token
            foreach (char currChar in regExp)
            {
                if (!RegexOperators.IsOperator(currChar))
                {
                    //For operands (characters), we simply create an NFA that matches the given letter
                    operandStack.Push(new NFAFragment(new NFACharacterSet(currChar.ToString())));
                }
                else
                {
                    //The token is an operator
                    //No processing needed for empty stack and opening brackets
                    IRegExpOperator asOp = RegexOperators.GenerateOperator(currChar);

                    if (operatorStack.Count == 0 || asOp.GetOperatorType() == RegexOperatorType.LeftBracket)
                    {
                        operatorStack.Push(asOp);
                    }
                    else if (asOp.GetOperatorType() == RegexOperatorType.RightBracket)
                    {
                        //Evaluate everything between opening and closing brackets
                        while (operatorStack.Peek().GetOperatorType() != RegexOperatorType.LeftBracket)
                        {
                            operatorStack.Pop().Execute(operandStack);
                        }
                        operatorStack.Pop();
                    }
                    else
                    {
                        //For other cases, keep evaluating operators 
                        while (operatorStack.Count != 0 &&
                        operatorStack.Peek().GetPrecedence() >= asOp.GetPrecedence())
                        {
                            operatorStack.Pop().Execute(operandStack);
                        }
                        operatorStack.Push(asOp);
                    }
                }
            }

            //Evaluate the remaining operators
            while (operatorStack.Count != 0)
            {
                operatorStack.Pop().Execute(operandStack);
            }

            //At this point the only one NFA in the operandStack is the NFA
            //for the given expression.
            NFAFragment resNFA = operandStack.Pop();

            //We only need to connect all the exits from it to the matching state
            resNFA.ConnectUnboundStates(new NFAMatchState());

            return resNFA;
        }
    }
}
