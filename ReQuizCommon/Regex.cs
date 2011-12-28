using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReQuizCommon
{
    /// <summary>
    /// Gets thrown when the engine failed to compile the given expression into an NFA
    /// </summary>
    public class RegexCompileException : ApplicationException
    {
        public RegexCompileException() { }
        public RegexCompileException(string message) : base(message) { }
    }

    /// <summary>
    /// A basic regular expression engine
    /// </summary>
    public class Regex
    {
        //The generated NFA that is used to match the expression
        private NFAFragment resNFA;

        //The unparsed regular expression
        private string textView;

        //Random generator used to generate random strings that match the given regex
        private static Random randGen = new Random();

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
                result += ((NFACharacterSet)currState).RandomCharacter(randGen);
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
            textView = regExp;

            try
            {
                //Preprocess the regular expression and compile it into an NFA
                resNFA = NFABuilder.CreateNFA(regExp);
            }
            catch (IndexOutOfRangeException)
            {
                //Exception while creating the NFA, the regular expression must be invalid
                throw new RegexCompileException();
            }
            catch (InvalidOperationException)
            {
                throw new RegexCompileException();
            }
        }
    }
}
