using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPAD
{
    /// <summary>
    /// Represents a range of values.
    /// </summary>
    internal class Range
    {
        /// <summary>
        /// Shorthand for new Range(1).
        /// </summary>
        public static Range One => new Range(1);

        /// <summary>
        /// Shorthand for new Range(0).
        /// </summary>
        public static Range Zero => new Range(0);

        /// <summary>
        /// The minimum value of the Range.
        /// </summary>
        public float Minimum { get; set; }

        /// <summary>
        /// The maximum value of the Range.
        /// </summary>
        public float Maximum { get; set; }

        /// <summary>
        /// Gets the distance between the Maximum and Minimum values.
        /// </summary>
        public float Distance => Maximum - Minimum;

        /// <summary>
        /// Creates a new Range from the given minimum value to the given maximum value.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when min or max is below 0, or when max is less than min.</exception>
        public Range(float min, float max)
        {
            if (min < 0)
            {
                throw new ArgumentOutOfRangeException("The Range minimum must be greater than or equal to 0.");
            }
            if (max < 0)
            {
                throw new ArgumentOutOfRangeException("The Range maximum must be greater than or equal to 0.");
            }
            if (max < min)
            {
                throw new ArgumentOutOfRangeException("The Range maximum must be greater than or equal to the Range minimum.");
            }

            Minimum = min;
            Maximum = max;
        }

        /// <summary>
        /// Creates a new Range from the given value, which acts as both the minimum and the maximum.
        /// </summary>
        /// <param name="minmax"></param>
        public Range(float minmax) : this(minmax, minmax)
        {

        }

        /// <summary>
        /// Checks if this Range contains the given value. Minimum and Maximum are inclusive.
        /// </summary>
        /// <param name="value">The value that is being checked agains the Range.</param>
        /// <returns>True if the range contains the value, otherwise false.</returns>
        public bool Contains(float value)
        {
            return value >= Minimum && value <= Maximum;
        }

        public override bool Equals(object? obj)
        {
            return obj != null && obj is Range other && other.Minimum == Minimum && other.Maximum == Maximum;
        }

        public override int GetHashCode()
        {
            return Minimum.GetHashCode() ^ Maximum.GetHashCode();
        }

        public string ToString(string open, string close, bool combineIfMinMaxEqual = true)
        {
            if(combineIfMinMaxEqual && Minimum == Maximum)
            {
                return $"{open}{Minimum}{close}";
            } else
            {
                return $"{open}{Minimum}{Constants.RANGE_FLAG}{Maximum}{close}";
            }
            
        }

        public override string ToString()
        {
            return $"({Minimum}{Constants.RANGE_FLAG}{Maximum})";
        }

        /// <summary>
        /// Creates a new Range with the given minimum, and positive infinity as the maximum.
        /// </summary>
        /// <param name="min">The minimum value of the Range.</param>
        /// <returns>A new Range using the given minimum value.</returns>
        public static Range NoCeiling(float min = 0.0f)
        {
            return new Range(min, float.PositiveInfinity);
        }

        /// <summary>
        /// Creates a new Range using the given value as the minimum, and float.PositiveInfinity as the maximum.
        /// </summary>
        /// <param name="min"></param>
        /// <returns></returns>
        public static Range More(float min = 1.0f)
        {
            return new Range(min, float.PositiveInfinity);
        }

        /// <summary>
        /// Creates a new Range using the given value as the maximum, and 0 as the minimum.
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public static Range Less(float max = 1.0f)
        {
            return new Range(0.0f, max);
        }

        public static Range Parse(string[] stringTokens, ref int index)
        {
            //if the first one is a range...
            if(stringTokens[index] == Constants.RANGE_FLAG)
            {
                index++;
                //if first is range, there is no maximum

                if(float.TryParse(stringTokens[index], out float n))
                {
                    index++;

                    //got the minimum
                    return Range.More(n);
                }
            } else if (float.TryParse(stringTokens[index], out float n))
            {
                //if not a range, then it is a number
                index++;

                //check for range after
                if(stringTokens[index] == Constants.RANGE_FLAG)
                {
                    index++;

                    //there was a range, so check if there is a complete range or not
                    if(float.TryParse(stringTokens[index], out float m))
                    {
                        index++;

                        //full range
                        return new Range(m, n);
                    } else
                    {
                        //there is not a minimum
                        return Range.Less(n);
                    }

                } else
                {
                    //no range, so just one number which is the min and the max
                    return new Range(n);
                }
            }

            //if it got here, it was nothing
            return Range.Zero;
        }
    }
}
