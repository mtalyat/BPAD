using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPAD
{
    internal class Behavior : Element
    {
        /// <summary>
        /// The list of names of every Behavior.
        /// </summary>
        private static List<string> _names = new List<string>();

        /// <summary>
        /// The amount of Behavior names that have been documented.
        /// </summary>
        public static int NameCount => _names.Count;

        /// <summary>
        /// The identification number of this Behavior.
        /// This is used for fast comparison, and smaller memory size than storing the raw string for each instance.
        /// </summary>
        private int _id;

        /// <summary>
        /// The name of this Behavior.
        /// </summary>
        public string Name => _names[_id];

        /// <summary>
        /// Creates a new Behavior with the given name, range of times, and range of occurances.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="timing"></param>
        /// <param name="occurances"></param>
        public Behavior(string name)
        {
            //add name
            //if added, ID is the end of the list - 1
            //if not added, ID is somewhere in the list

            int index = _names.IndexOf(name);

            if(index >= 0)
            {
                //name exists
                _id = index;
            } else
            {
                //name does not exist
                _id = _names.Count;
                _names.Add(name);
            }
        }

        public override bool Equals(object? obj)
        {
            return obj != null && obj is Behavior other && other._id == _id;
        }

        public static bool operator ==(Behavior left, Behavior right)
        {
            return left._id == right._id;
        }

        public static bool operator !=(Behavior left, Behavior right)
        {
            return left._id != right._id;
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        protected override string GetString()
        {
            return Name;
        }

        /// <summary>
        /// Gets the corresponding name from the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string GetName(int index)
        {
            if(index < 0 || index >= _names.Count)
            {
                return "";
            }

            return _names[index];
        }

        internal override bool IsCompleted()
        {
            if (Occurances != null)
            {
                return Occurances.Contains(count);
            }

            return false;
        }

        protected override EvaluationResults OnEvaluate(EvaluationArgs args)
        {
            return args.e.Behavior._id == _id ? EvaluationResults.Stepped : EvaluationResults.Skipped;
        }

        protected override void OnReset()
        {
            //on reset, check for null occurances or timings
            //if null, set to defaults

            if(_occurances == null)
            {
                _occurances = Range.One;//1x
            }
        }

        internal override int[] GetIndices()
        {
            return _indices.ToArray();
        }
    }
}
