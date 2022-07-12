using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPAD
{
    internal static class Debug
    {
        private static List<string> lines = new List<string>();

        public static void Log(string line)
        {
            lines.Add(line);
        }

        public static string GetLog()
        {
            return string.Join(Environment.NewLine, lines.ToArray());
        }
    }
}
