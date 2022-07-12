using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPAD
{
    public class ParsingException : Exception
    {
        public ParsingException(string? message = null) : base(message)
        {

        }
    }
}
