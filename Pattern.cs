using System.Text;
using System.Text.RegularExpressions;

namespace BPAD
{
    /// <summary>
    /// An object can be used to search through data and identify the patterns within the data.
    /// </summary>
    public class Pattern
    {
        public static Pattern Empty => new Pattern("");

        /// <summary>
        /// The Name of this Pattern.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The steps this Pattern must evaluate properly for it to be found.
        /// </summary>
        private readonly List<Step> _steps = new List<Step>();

        /// <summary>
        /// The amount of Steps this Pattern contains.
        /// </summary>
        public int StepCount => _steps.Count;

        /// <summary>
        /// Creates an empty Pattern.
        /// </summary>
        internal Pattern(string name)
        {
            Name = name;
        }

        #region Add Steps

        /// <summary>
        /// Adds a Step to this Pattern.
        /// </summary>
        /// <param name="step"></param>
        internal void AddStep(Step step)
        {
            _steps.Add(step);
        }

        /// <summary>
        /// Adds multiple Steps to this Pattern.
        /// </summary>
        /// <param name="steps"></param>
        internal void AddSteps(params Step[] steps)
        {
            _steps.AddRange(steps);
        }

        /// <summary>
        /// Turns the given Elements into Steps, and adds them to this Pattern.
        /// </summary>
        /// <param name="steps"></param>
        internal void AddSteps(params Element[] steps)
        {
            AddSteps(steps.Select(s => new Step(s)).ToArray());
        }

        /// <summary>
        /// Parses and adds a Step to this Pattern.
        /// </summary>
        /// <param name="step"></param>
        public void AddStep(string step)
        {
            AddStep(Step.Parse(step));
        }

        /// <summary>
        /// Parses and adds all of the given Steps to this Pattern.
        /// </summary>
        /// <param name="steps"></param>
        public void AddSteps(params string[] steps)
        {
            foreach(string s in steps)
            {
                AddStep(s);
            }
        }

        /// <summary>
        /// Parses and inserts a Step to the given index position in this Pattern.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="step"></param>
        public void InsertStep(int index, string step)
        {
            if(index < 0 || index >= _steps.Count)
            {
                return;
            }

            _steps.Insert(index, Step.Parse(step));
        }

        #endregion

        #region Remove Steps

        /// <summary>
        /// Removes all Steps from this Pattern.
        /// </summary>
        public void RemoveSteps()
        {
            _steps.Clear();
        }

        /// <summary>
        /// Removes the Step at the given index.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveStepAt(int index)
        {
            if(index < 0 || index >= _steps.Count)
            {
                return;
            }

            _steps.RemoveAt(index);
        }

        #endregion

        #region Evaluation

        /// <summary>
        /// Searches for Patterns within the given names and times. 
        /// Assumes each name and time match if the index used to access each of them is the same.
        /// </summary>
        /// <param name="names">The names of the behaviours.</param>
        /// <param name="times">The time each corresponding behavior occurs at.</param>
        /// <returns>A 2D jagged array resembling every instance of the pattern. 
        /// [X][Y] where X is the instance of the Pattern, and Y is the list of indices of where the Pattern occured.</returns>
        public int[][] Evaluate(string[] names, float[] times)
        {
            //compile the list of events
            Event[] events = new Event[Math.Min(names.Length, times.Length)];
            for (int i = 0; i < events.Length; i++)
            {
                events[i] = new Event(names[i], times[i]);
            }

            //now evaluate as normal
            return Evaluate(events);
        }

        /// <summary>
        /// Searches for Patterns within the given Events.
        /// </summary>
        /// <param name="events"></param>
        /// <returns></returns>
        internal int[][] Evaluate(Event[] events)
        {
            //make sure there are steps in the first place
            if (!_steps.Any())
            {
                return Array.Empty<int[]>();
            }

            //reset for good measure
            ResetSteps();

            //output list
            List<int[]> output = new List<int[]>();
            List<int> indicies = new List<int>();

            //keep track of the ranges we want to add to the output list
            int firstEventIndex = -1;
            int stepIndex = 0;
            float lastStepTime = -1.0f;

            //check every event, and advance from step to step until we find a pattern, log that, and go back to step 0
            for (int i = 0, j = 0; i < events.Length; i++, j = i)
            {
                if (_steps[stepIndex].Evaluate(events, ref i, ref lastStepTime, indicies, stepIndex == _steps.Count - 1))
                {
                    //set first event index if we have not found one yet
                    if (firstEventIndex < 0)
                    {
                        firstEventIndex = j;
                    }

                    //move on to the next step
                    stepIndex++;

                    //if the step index >= step count, we have found a full pattern
                    if (stepIndex >= _steps.Count)
                    {
                        //add index range to the output result list
                        output.Add(indicies.ToArray());

                        //Debug.Log($"Added: {string.Join(", ", indicies.Select(ii => ii.ToString()))}");

                        //clear out list
                        indicies.Clear();

                        //reset the steps
                        ResetSteps();

                        //reset local variables
                        stepIndex = 0;
                        firstEventIndex = -1;
                        lastStepTime = -1.0f;
                    }
                }
                else
                {
                    //the step did not evaluate. This is so sad
                    //go back to the event that the step started at, and reset all steps
                    ResetSteps();

                    indicies.Clear();

                    stepIndex = 0;
                    firstEventIndex = -1;
                    lastStepTime = -1.0f;
                }
            }

            return output.ToArray();
        }

        /// <summary>
        /// Resets all of the Steps in this Pattern.
        /// </summary>
        private void ResetSteps()
        {
            foreach (Step step in _steps)
            {
                step.Reset();
            }
        }

        /// <summary>
        /// Searches for all the given Patterns within the given names and times.
        /// Assumes each name and time match if the index used to access each of them is the same.
        /// </summary>
        /// <param name="names">The names of the behaviours.</param>
        /// <param name="times">The time each corresponding behavior occurs at.</param>
        /// <param name="patterns">The Patterns that are to be searched for.</param>
        /// <returns>A 3D jagged array resembling every instance of the pattern. 
        /// [X][Y][Z] where X is the Pattern, Y is the instance of the Pattern, and Z is the list of indices of where the Pattern occured.</returns>
        public static int[][][] EvaluateAll(string[] names, float[] times, params Pattern[] patterns)
        {
            //compile the list of events
            Event[] events = new Event[Math.Min(names.Length, times.Length)];
            for (int i = 0; i < events.Length; i++)
            {
                events[i] = new Event(names[i], times[i]);
            }

            //now evaluate as normal
            return EvaluateAll(events, patterns);
        }

        /// <summary>
        /// Searches for all the given Patterns within the given Events.
        /// </summary>
        /// <param name="events"></param>
        /// <param name="patterns"></param>
        /// <returns></returns>
        internal static int[][][] EvaluateAll(Event[] events, params Pattern[] patterns)
        {
            //TODO: create a method that evaluates all patterns simultaneously

            //bad practice method below: doing it one at a time for no reason
            //why am I doing this? Time constraints

            int[][][] output = new int[patterns.Length][][];

            for (int i = 0; i < patterns.Length; i++)
            {
                output[i] = patterns[i].Evaluate(events);
            }

            return output;
        }

        #endregion

        public override string ToString()
        {
            return $"{string.Join($" {Constants.THEN_OPERATOR} ", _steps.Select(s => s.ToString()))} => {Name}";
        }

        /// <summary>
        /// Parses the given string data into a Pattern object.
        /// </summary>
        /// <param name="data">The string of characters that represents a pattern.</param>
        /// <returns>A Pattern object that resembles the same pattern in the given string data.</returns>
        public static Pattern Parse(string data)
        {
            string[] equationSides = data.Split(Constants.RESULT_OPERATOR);

            if(equationSides.Length < 2)
            {
                throw new ParsingException($"The given string does not have a result operator! ({Constants.RESULT_OPERATOR})");
            }else if (equationSides.Length > 2)
            {
                throw new ParsingException($"The given string has too many result operators! ({Constants.RESULT_OPERATOR})");
            }

            //the right side is the name
            Pattern pattern = new Pattern(equationSides[1].Trim());

            //the left side is the steps
            string[] stepDatas = equationSides[0].Split(Constants.THEN_OPERATOR);

            //turn into actual steps
            Step[] steps = stepDatas.Select(s => Step.Parse(s)).ToArray();

            //add to pattern
            pattern.AddSteps(steps);

            return pattern;
        }
    }
}