using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPAD
{
    internal abstract class Element
    {
        protected Range? _occurances;
        public Range? Occurances => _occurances;

        protected Range? _timings;
        public Range? Timings => _timings;

        protected HashSet<int> _indices { get; private set; }

        protected int count => _indices.Count;

        protected Element()
        {
            _occurances = null;
            _timings = null;
            _indices = new HashSet<int>();
        }

        internal void Initialize(Range occurances, Range timings)
        {
            TryInitializeOccurances(occurances);
            TryInitializeTimings(timings);
        }

        public virtual bool TrySetOccurances(Range range)
        {
            //only set if already null
            if(_occurances == null)
            {
                _occurances = range;
                return true;
            }

            return false;
        }

        public virtual bool TrySetTimings(Range range)
        {
            //only set if already null
            if (_timings == null)
            {
                _timings = range;
                return true;
            }

            return false;
        }

        public virtual bool TryInitializeOccurances(Range range)
        {
            //only set if already null
            if (_occurances == null)
            {
                _occurances = range;
                return true;
            }

            return false;
        }

        public virtual bool TryInitializeTimings(Range range)
        {
            //only set if already null
            if (_timings == null)
            {
                _timings = range;
                return true;
            }

            return false;
        }

        internal abstract bool IsCompleted();

        internal abstract int[] GetIndices();

        public EvaluationResults Evaluate(EvaluationArgs args)
        {
            //check if out of time range, if there is a time range
            if(args.lastTime >= 0.0f && Timings != null)
            {
                float time = args.e.Time - args.lastTime;

                if(time < Timings.Minimum)
                {
                    return EvaluationResults.BeforeTime;
                } else if (time > Timings.Maximum)
                {
                    //if after time, check for occurances
                    if(IsCompleted())
                    {
                        //fully completed, so add indices to the overall list
                        args.indices.AddRange(GetIndices());
                        return EvaluationResults.Completed;
                    } else
                    {
                        _indices.Clear();
                        return EvaluationResults.Canceled;
                    }
                }
            }

            //check if this behavior matches
            EvaluationResults results = OnEvaluate(args);

            if(results == EvaluationResults.Stepped)
            {
                //add current index to list
                _indices.Add(args.eIndex);

                //update last time recorded
                if(args.lastTime < 0.0f)
                {
                    args.lastTime = args.e.Time;
                }
            } else if (results == EvaluationResults.Completed)
            {
                //add indices to final indices list
                args.indices.AddRange(_indices);
            }
            else if (args.lastTime < 0.0f)
            {
                //if there was no last time, and the result did not step, then we have not started this step
                //(this step should be the initial step if lastTime < 0)
                return EvaluationResults.Invalid;
            }

            //check for occurances out of range, if there is an occurance range
            if(Occurances != null)
            {
                if(count < Occurances.Minimum)
                {
                    if(results == EvaluationResults.Stepped)
                    {
                        return EvaluationResults.Stepped;
                    } else
                    {
                        return EvaluationResults.NotEnoughOccurances;
                    }
                } else if(count > Occurances.Maximum)
                {
                    _indices.Clear();
                    return EvaluationResults.Canceled;
                } else
                {
                    if(results == EvaluationResults.Stepped)
                    {
                        return EvaluationResults.SteppedAndTechnicallyComplete;
                    } else
                    {
                        return EvaluationResults.TechnicallyComplete;
                    }
                }
            }

            //now just return whatever the OnEvaluate results were
            return results;
        }

        public void Reset()
        {
            _indices.Clear();

            OnReset();
        }

        protected abstract void OnReset();

        protected abstract EvaluationResults OnEvaluate(EvaluationArgs args);

        public override string ToString()
        {
            return $"{(Timings != null ? Timings.ToString(Constants.TIMING_OPEN, Constants.TIMING_CLOSE) : string.Empty)}{GetString()}{(Occurances != null ? Occurances.ToString(Constants.FLAG_OPEN, Constants.FLAG_CLOSE) : string.Empty)}";
        }

        protected abstract string GetString();
    }
}
