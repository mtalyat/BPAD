using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPAD
{
    internal class Step
    {
        private Element _element;

        internal Step(Element element)
        {
            _element = element;

            //set defaults, if needed
            _element.Initialize(new Range(1), new Range(0, 1));
        }

        public bool Evaluate(Event[] events, ref int currentIndex, ref float time, List<int> indices, bool isLastStep)
        {
            EvaluationArgs args = new EvaluationArgs(time, indices);

            EvaluationResults result = EvaluationResults.Invalid;
            int lastStepIndex = currentIndex;

            for (; currentIndex < events.Length; currentIndex++)
            {
                //give the args the event we are on
                args.eIndex = currentIndex;
                args.e = events[currentIndex];

                result = _element.Evaluate(args);

                //Debug.Log($"Evaluated {currentIndex}: {result}");

                time = args.lastTime;

                switch (result)
                {
                    case EvaluationResults.Canceled:
                    case EvaluationResults.Invalid:
                        currentIndex = lastStepIndex;
                        return false;//if any of these occur, this step stops the pattern
                    case EvaluationResults.Stepped:
                    case EvaluationResults.SteppedAndTechnicallyComplete:
                        lastStepIndex = currentIndex;
                        break;
                    case EvaluationResults.Completed:
                        //set time
                        time = args.currentTime;
                        if (isLastStep)     //if at the very end...
                            currentIndex--;                 //go back one event
                        else                //if finished a step and has more to go...
                            currentIndex = lastStepIndex;   //go back to last step
                        return true;//we are done!
                    default:
                        break;//continue on checking events
                }
            }

            //if we reached the end of the events, if the event was technically completed, then check if technically completed
            if(result <= EvaluationResults.TechnicallyComplete)
            {
                //add indices from element
                indices.AddRange(_element.GetIndices());

                //set time
                time = args.currentTime;

                //adjust index
                if (isLastStep)     //if at the very end...
                    currentIndex--;                 //go back one event
                else                //if finished a step and has more to go...
                    currentIndex = lastStepIndex;   //go back to last step

                //completed
                return true;
            } else
            {
                //not completed
                return false;
            }
        }

        public void Reset()
        {
            _element.Reset();
        }

        public override string ToString()
        {
            return _element.ToString() ?? "";
        }

        #region Parsing

        public static Step Parse(string data)
        {
            //get the string tokens
            string[] stringTokens = SplitIntoTokens(data);

            //turn it into parsing tokens for easy parsing
            ParsingToken[] tokens = StringToParsingTokens(stringTokens);

            //put it in reverse polish notation for easy reading
            tokens = InfixToPostfix(tokens);

            //now compile it into elements (behaviors and conditionals)
            return new Step(CompileElement(tokens));


        }

        private static int CheckForMatch(string data, int index, string[] strs)
        {
            for (int i = 0; i < strs.Length; i++)
            {
                if (CheckForMatch(data, index, strs[i]))
                {
                    return strs[i].Length;
                }
            }

            return -1;
        }

        private static bool CheckForMatch(string data, int index, string str)
        {
            //check for escape character
            if(index - 1 >= 0 && data[index - 1] == Constants.ESCAPE_CHARACTER)
            {
                //does not match if escape character found
                return false;
            }

            for (int i = 0; i < str.Length; i++)
            {
                if (data[i + index] != str[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Given the string of characteres, this will split the string into "tokens".
        /// Ex. "Behavior1 | Behavior[2+] => PatternName" becomes "Behavior1" "|" "Behavior" "[" "2" "+" "]" "=>" "PatternName"
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static string[] SplitIntoTokens(string data)
        {
            StringBuilder current = new StringBuilder();
            List<string> tokens = new List<string>();

            int match = -1;
            bool bracketedMode = false;

            for (int i = 0; i < data.Length; i++)
            {
                //check each string for brackets or splitters
                match = CheckForMatch(data, i, Constants.BRACKETS);

                if (match >= 0)
                {
                    bracketedMode = !bracketedMode;
                }

                if (match < 0 && bracketedMode)
                {
                    match = CheckForMatch(data, i, Constants.FLAGS);
                }

                if (match < 0)
                {
                    match = CheckForMatch(data, i, Constants.SPLITTERS);
                }

                //if a match is found, split
                if (match >= 0)
                {
                    //match
                    //add existing to split, if it is valid
                    if (current.Length > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(current.ToString()))
                        {
                            tokens.Add(current.ToString().Trim());
                        }

                        current.Clear();
                    }

                    //add match to split as well
                    tokens.Add(data.Substring(i, match));

                    //move i to end of match
                    i += match - 1;//-1, as the loop will add 1 when the loop starts over
                }
                else
                {
                    //no match, just add current character to the current

                    //do not add if it is an escape character
                    if(data[i] != Constants.ESCAPE_CHARACTER)
                    {
                        current.Append(data[i]);
                    }
                }
            }

            //if something left over, add it
            if (current.Length > 0)
            {
                tokens.Add(current.ToString());
                current.Clear();
            }

            //return list
            return tokens.ToArray();
        }

        private static ParsingToken[] StringToParsingTokens(string[] stringTokens)
        {
            //group up the strings from right to left

            //now left to right
            Array.Reverse(stringTokens);

            List<ParsingToken> tokens = new List<ParsingToken>();
            ParsingToken? t;

            for (int i = 0; i < stringTokens.Length; i++)
            {
                t = ParsingToken.Parse(stringTokens, ref i);
                if (t != null)
                {
                    tokens.Add(t);
                }
            }

            tokens.Reverse();

            return tokens.ToArray();
        }

        private static ParsingToken[] InfixToPostfix(ParsingToken[] inTokens)
        {
            //infix notation, grab in order from a queue
            Queue<ParsingToken> tokens = new Queue<ParsingToken>(inTokens);

            Stack<ParsingToken> outputs = new Stack<ParsingToken>();
            Stack<ParsingToken> operators = new Stack<ParsingToken>();

            ParsingToken token;

            while (tokens.Any())
            {
                token = tokens.Dequeue();

                if (token.Precedence > 0)//if the precedence > 0, we know it is an "operator"
                {
                    if (operators.Any())
                    {
                        ParsingToken op = operators.Peek();

                        //if the "operator" on the stack has a higher precedence, remove it
                        //keep removing as long as it is not an opening bracket
                        while (op.Precedence > token.Precedence && !(op is BracketToken bs && bs.IsOpening))
                        {
                            outputs.Push(operators.Pop());

                            if (!operators.Any())
                            {
                                //no more operators, so stopm looping
                                break;
                            }
                            else
                            {
                                //still more, so check the next one
                                op = operators.Peek();
                            }
                        }
                    }

                    operators.Push(token);
                }
                else if (token is BracketToken bo && bo.IsOpening)//open parenthesis
                {
                    operators.Push(token);
                }
                else if (token is BracketToken bc && bc.IsClosing)//closing parenthesis
                {
                    //add all within the brackets to output
                    while (operators.Any() && !(operators.Peek() is BracketToken bss && bss.IsOpening))
                    {
                        outputs.Push(operators.Pop());
                    }

                    //check for mismatched parenthesis
                    if (!operators.Any())
                    {
                        throw new ParsingException("Missing opening parenthesis.");
                    }

                    //remove the open parenthesis
                    if (operators.Peek() is BracketToken bs && bs.IsOpening)
                    {
                        operators.Pop();
                    }
                }
                else
                {
                    //something else, or behavior
                    outputs.Push(token);
                }
            }

            //add the remaining operators to the output
            while (operators.Any())
            {
                //if an operator is a parenthesis, that means we have another mismatch
                if (operators.Peek() is BracketToken bt && bt.IsOpening)
                {
                    throw new ParsingException("Missing closing parenthesis.");
                }
                else
                {
                    outputs.Push(operators.Pop());
                }
            }

            //now in postfix notation
            return outputs.ToArray();
        }

        private static Element CompileElement(ParsingToken[] postfixTokens)
        {
            int index = 0;
            return CompileElement(postfixTokens, ref index);
        }

        private static Element CompileElement(ParsingToken[] postfixTokens, ref int index)
        {
            //use these for temporary values to set values to elements
            FlagsToken? flags = null;
            TimingToken? timing = null;

            ParsingToken token;

            for (; index < postfixTokens.Length; index++)
            {
                token = postfixTokens[index];

                if(token is FlagsToken ft)
                {
                    flags = ft;
                } else if (token is TimingToken tt)
                {
                    timing = tt;
                } else if (token is OperatorToken ot)
                {
                    //if operator, make it a conditional
                    Conditional conditional;

                    //get the left and right sides
                    index++;
                    Element left = CompileElement(postfixTokens, ref index);

                    index++;
                    Element right = CompileElement(postfixTokens, ref index);

                    //now create the conditional
                    if(ot.Operator == Constants.AND_OPERATOR)
                    {
                        conditional = new ConditionalAnd(left, right);
                    } else if (ot.Operator == Constants.OR_OPERATOR)
                    {
                        conditional = new ConditionalOr(left, right);
                    } else
                    {
                        //unknown operator
                        throw new ParsingException("Unknown operator when compiling.");
                    }

                    //apply the flags and timing if able
                    if(flags != null)
                    {
                        conditional.TrySetOccurances(flags.OccurancesRange);
                    }

                    if(timing != null)
                    {
                        conditional.TrySetTimings(timing.Timings);
                    }

                    //return it
                    return conditional;
                } else if (token is BehaviorToken bt)
                {
                    //create the behavior
                    Behavior behavior = new Behavior(bt.Name);

                    //apply the flags and timing if able
                    if (flags != null)
                    {
                        behavior.TrySetOccurances(flags.OccurancesRange);
                    }

                    if (timing != null)
                    {
                        behavior.TrySetTimings(timing.Timings);
                    }

                    //return it
                    return behavior;
                } else
                {
                    //unknown token
                    throw new ParsingException("Unknown token when compiling.");
                }
            }

            //hmmm... shouldn't get here
            throw new ParsingException("Mismatch behaviors to operators when compiling.");
        }

        #endregion
    }
}
