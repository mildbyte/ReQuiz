using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReQuizCommon
{
    /// <summary>
    /// Enumerates the possible types of a non-deterministic finite automaton state
    /// </summary>
    enum NFAStateType {
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
    interface INFAState
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
    class NFACharacterSet: INFAState
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
                } else {
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
        public void Connect (INFAState toConnect) {
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
        public void Connect (INFAState toConnect) {
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
    class NFAFragment
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
        public RegexOperatorType GetOperatorType() {return RegexOperatorType.Plus;}

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
    /// A basic regular expression engine
    /// </summary>
    public class Regex
    {
        //Stack of the currently used operators
        private Stack<IRegExpOperator> operatorStack;

        //Stack of the currently used NFAs
        private Stack<NFAFragment> operandStack;

        //The generated NFA that is used to 
        private NFAFragment resNFA;

        //The unparsed regular expression
        private string textView;

        //Random generator used to generate random strings that match the given regex
        private static Random randGen = new Random();

        /// <summary>
        /// Checks whether the given character is alphanumeric
        /// </summary>
        /// <param name="chr">The character to check</param>
        /// <returns>True if the character is alphanumeric, False if it isn't</returns>
        private bool IsAlphaNumeric(char chr)
        {
            if (chr >= 48 && chr <= 57) return true; //Digit
            if (chr >= 65 && chr <= 90) return true; //Uppercase letter
            if (chr >= 97 && chr <= 122) return true;//Lowercase letter

            return false;
        }

        //Checks whether the given IRegExpOperator is a regexp quantifier
        private bool IsQuantifier(IRegExpOperator op)
        {
            if (op.GetOperatorType() == RegexOperatorType.Plus
                || op.GetOperatorType() == RegexOperatorType.Question
                || op.GetOperatorType() == RegexOperatorType.Star
            ) return true;

            return false;
        }

        //Checks whether the given character is a regexp quantifier
        private bool IsQuantifier(char chr)
        {
            if (chr == '?' || chr == '+' || chr == '*') return true;
            else return false;
        }

        //Checks whether the given character is a regexp operator
        private bool IsOperator(char chr)
        {
            if (chr == '?' || chr == '+' || chr == '*' || chr == '(' || chr == ')' || chr == '|' || chr == '/') return true;
            return false;
        }


        /// <summary>
        /// Generates a regexp operator from the given character
        /// </summary>
        /// <param name="op">Character to generate the operator from</param>
        /// <returns>The generated operator</returns>
        private IRegExpOperator GenerateOperator (char op) {
            switch (op) {
                case '+': return new PlusOperator(); break;
                case '?': return new QuestionOperator(); break;
                case '*': return new StarOperator(); break;
                case '|': return new AlternateOperator(); break;
                case '(': return new LeftBracketOperator(); break;
                case ')': return new RightBracketOperator(); break;
                case '/': return new ConcatenateOperator(); break;
                default: throw new Exception("invalid operator");
            }
        }

        /// <summary>
        /// Preprocesses the regular expression by inserting explicit
        /// concatenation operators ("/")
        /// between (AN means alphanumeric character):
        /// AN + AN, AN + opening bracket, closing bracket + AN
        /// quantifier + AN, quantifier + opening bracket, closing bracket + opening bracket
        /// Also surrounds the expression with brackets (to eliminate redundant code later)
        /// </summary>
        /// <param name="regExp">The regular expression to preprocess</param>
        /// <returns>The preprocessed regular expression</returns>
        private string Preprocess(string regExp)
        {
            //TODO: character group support - fix or eliminate completely?

            //Traverse the original expression
            //Look at the current and the next character, copy the current into the new string
            //If we need to insert a concat operator between them, add the operator into the new string
            string result = "(";

            bool insideGroup = false;
            for (int i = 0; i < regExp.Length - 1; i++)
            {
                if (regExp[i] == '/') result += '\\';
                if (regExp[i] == '[') insideGroup = true;
                result += regExp[i];
                
                if (
                      (IsAlphaNumeric(regExp[i]) && IsAlphaNumeric(regExp[i + 1]))
                   || (IsAlphaNumeric(regExp[i]) && regExp[i + 1] == '(')
                   || (regExp[i] == ')' && IsAlphaNumeric(regExp[i + 1]))
                   || (IsQuantifier(regExp[i]) && IsAlphaNumeric(regExp[i + 1]))
                   || (IsQuantifier(regExp[i]) && regExp[i + 1] == '(')
                   || (regExp[i] == ')' && regExp[i + 1] == '(')
                    ) result += '/';
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
        private void CreateNFA(string regExp)
        {
            //Defines whether the next character is being escaped (explicitly considered to not be a metacharacter)
            bool escaping = false;

            //Traverse the expression character-by-character
            //Expression already set up so that one character is one token
            foreach (char currChar in regExp)
            {
                if (!IsOperator(currChar) || escaping)
                {
                    //For operands (characters), we simply create an NFA that matches the given letter
                    operandStack.Push(new NFAFragment(new NFACharacterSet(currChar.ToString())));
                    escaping = false;
                }
                else if (currChar == '\\') {
                    //If the escape symbol detected, set the escape flag
                    escaping = true;
                    continue;
                }
                else
                {
                    //The token is an operator
                    //No processing needed for empty stack and opening brackets
                    IRegExpOperator asOp = GenerateOperator(currChar);

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
                    } else {
                        //For other cases, keep evaluating operators 
                        while (operatorStack.Count != 0 && 
                        operatorStack.Peek().GetPrecedence() >= asOp.GetPrecedence()) {
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

            resNFA = operandStack.Pop();

            //We only need to connect all the exits from it to the matching state
            resNFA.ConnectUnboundStates(new NFAMatchState());
        }

        /// <summary>
        /// Adds a normal state to the set
        /// Traverses split states recursively until normal states they are connected to
        /// are found, then adds these states to the set
        /// </summary>
        /// <param name="set">The set to add the state to</param>
        /// <param name="state">The state to add to the set</param>
        private void AddStateToSet(HashSet<INFAState> set, INFAState state)
        {
            //If the state is a split state, traverse it recursively until the
            //real state is reached
            if (state.GetNFAType() == NFAStateType.Split)
            {
                AddStateToSet(set, ((NFASplitState)state).nextState1);
                AddStateToSet(set, ((NFASplitState)state).nextState2);
            }
            else set.Add(state);
        }

        /// <summary>
        /// Generates a random string that matches the regular expression
        /// </summary>
        /// <returns>The string that matches the regular expression</returns>
        public string RandomString()
        {
            //Randomly traverses the NFA and selects letters
            //from the paths it takes until it reaches the matching state

            string result = "";
            INFAState currState = resNFA.startState;

            while (true)
            {
                while (currState.GetNFAType() == NFAStateType.Split)
                {
                    //Choose a random path through split states until we reach something else
                    if (randGen.Next(2) == 1)
                        currState = ((NFASplitState)currState).nextState1;
                    else currState = ((NFASplitState)currState).nextState2;
                }

                //If we have reached the final state, stop looping
                if (currState.GetNFAType() == NFAStateType.Match) break;

                //The NFA needs a letter to go further
                //Append the letter to the output and take the next state
                result += ((NFACharacterSet)currState).RandomCharacter();
                currState = ((NFACharacterSet)currState).nextState;
            }

            return result;
        }


        /// <summary>
        /// Determines whether the given string matches this regular expression completely
        /// </summary>
        /// <param name="toMatch">The string to match to the expression</param>
        public bool Match(string toMatch)
        {
            //Performs a breadth-first traversal of the NFA
            //Easier than converting the NFA into a DFA, faster than performing a DFS

            //Set up the sets of states the NFA is in
            HashSet<INFAState> currStateSet = new HashSet<INFAState>();
            HashSet<INFAState> newStateSet = new HashSet<INFAState>();
            HashSet<INFAState> buf;

            //Add the start state, if it splits, add the two substates instead
            AddStateToSet(currStateSet, resNFA.startState);
            
            //Iterate through characters in the string
            for (int i = 0; i < toMatch.Length; i++)
            {   
                //If at any point there are no states to choose from, the matching has failed
                if (currStateSet.Count == 0) return false;

                //Iterate through states, forming the new state set
                foreach (INFAState currState in currStateSet)
                {   
                    //Add the next state (won't add if it's an end state)
                    if (currState.GetNFAType() == NFAStateType.CharSet && ((NFACharacterSet)currState).Accepts(toMatch[i]))
                    {
                        AddStateToSet(newStateSet, ((NFACharacterSet)currState).nextState);
                    }
                }

                //Replace the old state set with the new one.
                buf = currStateSet; currStateSet = newStateSet; newStateSet = buf;
                newStateSet.Clear();
            }

            //If one of the states the NFA ended in is the ending state, the string matches.
            foreach (INFAState currState in currStateSet)
            {
                if (currState.GetNFAType() == NFAStateType.Match)
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Compiles the given regular expression
        /// </summary>
        /// <param name="regExp">The regular expression to parse</param>
        public Regex(string regExp)
        {
            //Initialisation
            operatorStack = new Stack<IRegExpOperator>();
            operandStack = new Stack<NFAFragment>();
            textView = regExp;

            //Preprocess the regular expression and compile it into an NFA
            CreateNFA(Preprocess(regExp));
        }
    }
}
