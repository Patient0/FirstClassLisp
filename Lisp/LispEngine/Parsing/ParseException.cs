using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LispEngine.Parsing
{
    public class ParseException : Exception
    {
        public ParseException(string fmt)
            : base(fmt)
        {
        }
    }
}
