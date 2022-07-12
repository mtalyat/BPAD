using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPAD
{
    internal abstract class ParsingToken
    {
        public virtual int Precedence => 0;

        private static bool IsOneOf(string token, string[] strs)
        {
            foreach (string str in strs)
            {
                if (str == token)
                {
                    return true;
                }
            }

            return false;
        }

        public static ParsingToken? Parse(string[] stringTokens, ref int index)
        {
            string current = stringTokens[index];

            if (IsOneOf(current, Constants.OPERATORS))
            {
                return new OperatorToken(current);
            }
            else if (current == Constants.GROUP_OPEN || current == Constants.GROUP_CLOSE)
            {
                return new BracketToken(current == Constants.GROUP_OPEN);
            }
            else if (current == Constants.FLAG_CLOSE)
            {
                index++;
                return FlagsToken.Parse(stringTokens, ref index);
            }
            else if (current == Constants.TIMING_CLOSE)
            {
                index++;
                return TimingToken.Parse(stringTokens, ref index);
            }
            else if (!string.IsNullOrWhiteSpace(current))
            {
                //if nothing else, must be a behavior, if the string has content
                return new BehaviorToken(current.Trim());
            } else
            {
                //string had no content, so it was nothing
                return null;
            }
        }
    }
}
