using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReQuizClient
{
    enum NFAStateType {
        CharSet,
        Split,
        Match
    };

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

    interface NFAState
    {
        NFAStateType GetType();
        void Connect(NFAState toConnect);
    };

    class NFACharacterSet: NFAState
    {
        private bool[] characters;
        public NFAState nextState;
        public char RandomCharacter()
        {
            char i = (char)0;
            while (i < 256 && !characters[i]) i++;

            return i;
        }
        public bool Accepts(char chr)
        {
            return characters[chr];
        }
        public NFACharacterSet(string character)
        {
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

        public NFAStateType GetType()
        {
            return NFAStateType.CharSet;
        }
        public void Connect (NFAState toConnect) {
            if (nextState == null) nextState = toConnect;
        }
    };
    class NFASplitState : NFAState
    {
        public NFAState nextState1, nextState2;

        public void Connect (NFAState toConnect) {
            if (nextState1 == null) nextState1 = toConnect;
            if (nextState2 == null) nextState2 = toConnect;
        }

        public NFAStateType GetType()
        {
            return NFAStateType.Split;
        }

        public NFASplitState()
        {
            nextState1 = null;
            nextState2 = null;
        }

        public NFASplitState(NFAState next1, NFAState next2)
        {
            nextState1 = next1;
            nextState2 = next2;
        }
    };
    class NFAMatchState : NFAState
    {
        public NFAStateType GetType() { return NFAStateType.Match; }
        public void Connect(NFAState toConnect)
        {
            //Does nothing.
        }
    };

    class NFAFragment
    {
        public NFAState startState;
        public List<NFAState> unboundStates;
        public void ConnectUnboundStates(NFAState state)
        {
            foreach (NFAState currState in unboundStates)
            {
                currState.Connect(state);
            }
        }

        public NFAFragment()
        {
            startState = null;
            unboundStates = new List<NFAState>();
        }
        public NFAFragment(NFAState state)
        {
            startState = state;
            unboundStates = new List<NFAState>();
            unboundStates.Add(state);
        }
    };

    interface RegExpOperator
    {
        RegexOperatorType GetOperatorType();
        int GetPrecedence();
        void Execute(Stack<NFAFragment> operandStack);
    };

    class AlternateOperator : RegExpOperator
    {
        public RegexOperatorType GetOperatorType() { return RegexOperatorType.Alternate; }
        public void Execute(Stack<NFAFragment> operandStack)
        {
            //Executes the "|" operator by creating a state from which we can 
            //choose to move to either first NFA or the second

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
    class ConcatenateOperator : RegExpOperator
    {
        public RegexOperatorType GetOperatorType() { return RegexOperatorType.Concatenate; }
        public void Execute(Stack<NFAFragment> operandStack)
        {
            //Joins the two NFAs on the stack in series, so that the resultant 
            //NFA accepts the input only if both NFAs it's made of accept it.

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
    class PlusOperator : RegExpOperator
    {
        public RegexOperatorType GetOperatorType() {return RegexOperatorType.Plus;}
        public void Execute(Stack<NFAFragment> operandStack)
        {
            //The resultant NFA consists of the original NFA, the outputs from which
            //go to a split state which gives an option of going back to the same NFA.
            //Result - accepts one or more of the given NFA.

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
    class QuestionOperator : RegExpOperator
    {
        public RegexOperatorType GetOperatorType() { return RegexOperatorType.Question; }
        public void Execute(Stack<NFAFragment> operandStack)
        {
            //Question mark operator - creates a split state that allows to either
            //go through the NFA or not.

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
    class StarOperator : RegExpOperator
    {
        public RegexOperatorType GetOperatorType() { return RegexOperatorType.Star; }
        public void Execute(Stack<NFAFragment> operandStack)
        {
            //Creates a split state which allows to either go through the NFA or continue
            //The exits from the NFA will lead back to this state
            //Result - zero or more of the NFA
            //The only difference from the Plus is that the split is before the NFA.

            NFAFragment nfa1 = operandStack.Pop();

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
    class LeftBracketOperator : RegExpOperator
    {
        public RegexOperatorType GetOperatorType() { return RegexOperatorType.LeftBracket; }
        public void Execute(Stack<NFAFragment> operandStack)
        {
        }
        public int GetPrecedence() { return -1; }
    }
    class RightBracketOperator : RegExpOperator
    {
        public RegexOperatorType GetOperatorType() { return RegexOperatorType.RightBracket; }
        public void Execute(Stack<NFAFragment> operandStack)
        {
        }
        public int GetPrecedence() { return -1; }
    }
    
    class RegExp
    {
        private Stack<RegExpOperator> operatorStack;
        private Stack<NFAFragment> operandStack;

        private NFAFragment resNFA;
        private string textView;
        private Random randomGen;

        //A set of helper routines to detect different classes of characters
        private bool IsAlphaNumeric(char chr)
        {
            if (chr >= 48 && chr <= 57) return true; //Digit
            if (chr >= 65 && chr <= 90) return true; //Uppercase letter
            if (chr >= 97 && chr <= 122) return true;//Lowercase letter

            return false;
        }

        private bool IsQuantifier(RegExpOperator op)
        {
            if (op.GetOperatorType() == RegexOperatorType.Plus
                || op.GetOperatorType() == RegexOperatorType.Question
                || op.GetOperatorType() == RegexOperatorType.Star
            ) return true;

            return false;
        }

        private bool IsQuantifier(char chr)
        {
            if (chr == '?' || chr == '+' || chr == '*') return true;
            else return false;
        }

        private bool IsOperator(char chr)
        {
            if (chr == '?' || chr == '+' || chr == '*' || chr == '(' || chr == ')' || chr == '|' || chr == '/') return true;
            return false;
        }

        private RegExpOperator GenerateOperator (char op) {
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

        private string Preprocess(string regExp)
        {
            //Preprocesses the regular expression by inserting explicit
            //concatenation operators ("/")
            //between (AN means alphanumeric character):
            //AN + AN, AN + opening bracket, closing bracket + AN
            //quantifier + AN, quantifier + opening bracket, closing bracket + opening bracket
            //Also surrounds the expression with brackets (to eliminate redundant code later)

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

        private void AddStateToSet(HashSet<NFAState> set, NFAState state)
        {
            //If the state is a split state, traverse it recursively until the
            //real state is reached
            if (state.GetType() == NFAStateType.Split)
            {
                AddStateToSet(set, ((NFASplitState)state).nextState1);
                AddStateToSet(set, ((NFASplitState)state).nextState2);
            }
            else set.Add(state);
        }
        private void CreateNFA(string regExp)
        {
            //Converts the preprocessed regular expression into postfix
            //notation using a stack
            //Assumes that the input is a valid regular expression.

            //Traverse the expression character-by-character
            //Expression already set up so that one character is one token

            bool escaping = false;

            foreach (char currChar in regExp)
            {
                if (!IsOperator(currChar) || escaping)
                {
                    //For operands (characters), we simply create an NFA that matches the given letter
                    operandStack.Push(new NFAFragment(new NFACharacterSet(currChar.ToString())));
                    escaping = false;
                }
                else if (currChar == '\\') {
                    escaping = true;
                    continue;
                }
                else
                {
                    //The token is an operator
                    //No processing needed for empty stack and opening brackets
                    RegExpOperator asOp = GenerateOperator(currChar);

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

        public string RandomString()
        {
            //Generates a random string that matches the regular expression
            //Does it by randomly traversing the NFA and selecting letters
            //from the paths it takes, until it reaches the matching state

            string result = "";
            NFAState currState = resNFA.startState;

            while (true)
            {
                while (currState.GetType() == NFAStateType.Split)
                {
                    //Choose a random path through split states until we reach something else
                    if (randomGen.Next(2) == 1)
                        currState = ((NFASplitState)currState).nextState1;
                    else currState = ((NFASplitState)currState).nextState2;
                }

                //If we have reached the final state, stop looping
                if (currState.GetType() == NFAStateType.Match) break;

                //The NFA needs a letter to go further
                //Append the letter to the output and take the next state
                result += ((NFACharacterSet)currState).RandomCharacter();
                currState = ((NFACharacterSet)currState).nextState;
            }

            return result;
        }

        public bool Match(string toMatch)
        {
            //Matches the given string to the regular expression
            //Does to by performing a breadth-first traversal of the NFA
            //and keeping a set of the states it's now in.
            //Easier than converting the NFA into a DFA
            //or performing a depth-first traversal

            HashSet<NFAState> currStateSet = new HashSet<NFAState>();
            HashSet<NFAState> newStateSet = new HashSet<NFAState>();
            HashSet<NFAState> buf;

            //Add the start state, if it splits, add the two substates instead
            AddStateToSet(currStateSet, resNFA.startState);
            

            //Iterate through characters in the string
            for (int i = 0; i < toMatch.Length; i++)
            {   
                //If at any point there are no states to choose from, the matching has failed
                if (currStateSet.Count == 0) return false;

                //Iterate through states, forming the new state set
                foreach (NFAState currState in currStateSet)
                {   
                    //Add the next state (won't add if it's an end state
                    if (currState.GetType() == NFAStateType.CharSet && ((NFACharacterSet)currState).Accepts(toMatch[i]))
                    {
                        AddStateToSet(newStateSet, ((NFACharacterSet)currState).nextState);
                    }
                }

                //Replace the old state set with the new one.
                buf = currStateSet; currStateSet = newStateSet; newStateSet = buf;
                newStateSet.Clear();
            }


            //If one of the states the NFA ended in is the ending state, the string matches.
            foreach (NFAState currState in currStateSet)
            {
                if (currState.GetType() == NFAStateType.Match)
                {
                    return true;
                }
            }

            return false;
        }

        public RegExp(string regExp)
        {
            //Constructs a non-deterministic finite state automata
            //from the given string
            operatorStack = new Stack<RegExpOperator>();
            operandStack = new Stack<NFAFragment>();
            randomGen = new Random();
            textView = regExp;

            CreateNFA(Preprocess(regExp));
        }
    }
}
