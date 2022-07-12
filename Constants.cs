using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPAD
{
    /// <summary>
    /// The identifiers, values, and syntax used within the program.
    /// </summary>
    internal class Constants
    {
        public const string GROUP_OPEN = "(";
        public const string GROUP_CLOSE = ")";

        public const string FLAG_OPEN = "[";
        public const string FLAG_CLOSE = "]";

        public const string TIMING_OPEN = "{";
        public const string TIMING_CLOSE = "}";

        public const string OR_OPERATOR = "|";
        public const string AND_OPERATOR = "&";
        public const string EXCEPT_OPERATOR = "^";

        public const string THEN_OPERATOR = "->";
        public const string RESULT_OPERATOR = "=>";

        public static string SECONDS_FLAG = "s";
        public static string OCCURANCES_FLAG = "x";
        public static string OCCURANCES_MORE_FLAG = "+";
        public static string OCCURANCES_LESS_FLAG = "-";
        public static string RANGE_FLAG = ":";
        public static string EXCEPT_FLAG = "!";
        public static string OPTIONAL_FLAG = "?";
        public static string OPTIONAL_MORE_FLAG = "*";

        public static char ESCAPE_CHARACTER = '\\';

        public static readonly string[] BRACKETS = new string[]
        {
                FLAG_OPEN, FLAG_CLOSE,
                TIMING_OPEN, TIMING_CLOSE,
        };

        public static readonly string[] SPLITTERS = new string[]
        {
                GROUP_OPEN, GROUP_CLOSE,
                OR_OPERATOR, AND_OPERATOR, EXCEPT_OPERATOR,
                THEN_OPERATOR, RESULT_OPERATOR,
        };

        public static readonly string[] FLAGS = new string[]
        {
                SECONDS_FLAG,
                OCCURANCES_FLAG, RANGE_FLAG, OCCURANCES_MORE_FLAG, OCCURANCES_LESS_FLAG,
                OPTIONAL_FLAG, OPTIONAL_MORE_FLAG, EXCEPT_FLAG,
        };

        public static readonly string[] OPERATORS = new string[]
        {
            OR_OPERATOR, AND_OPERATOR
        };
    }
}
