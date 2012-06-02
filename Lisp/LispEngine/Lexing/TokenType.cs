using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LispEngine.Lexing
{
    public enum TokenType
    {
        Space,
        Symbol,
        Integer,
        Open,
        Close,
        Dot,
        Boolean,
        Quote,
        Comment
    }
}
