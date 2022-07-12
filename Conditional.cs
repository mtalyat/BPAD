using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPAD
{
    internal abstract class Conditional : Element
    {
        protected Element _left;
        protected Element _right;

        protected Conditional(Element left, Element right)
        {
            _left = left;
            _right = right;
        }

        protected override void OnReset()
        {
            //just reset the left and right sides
            _left.Reset();
            _right.Reset();
        }

        internal override int[] GetIndices()
        {
            HashSet<int> output = new HashSet<int>(_indices);
            foreach(var i in _left.GetIndices())
            {
                output.Add(i);
            }
            foreach(var i in _right.GetIndices())
            {
                output.Add(i);
            }

            return output.ToArray();
        }

        public override bool TryInitializeOccurances(Range range)
        {
            if(_occurances == null)
            {
                bool l = _left.TryInitializeOccurances(range);
                bool r = _right.TryInitializeOccurances(range);
                return l || r;
            }

            return false;
        }

        public override bool TryInitializeTimings(Range range)
        {
            if (_timings == null)
            {
                bool l = _left.TryInitializeTimings(range);
                bool r = _right.TryInitializeTimings(range);
                return l || r;
            }

            return false;
        }
    }
}
