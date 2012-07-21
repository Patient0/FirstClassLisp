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
        DotSymbol, // Special token for symbols beginning with "."
        Integer,
        String,
        Open,
        VectorOpen,
        Close,
        Dot,
        Boolean,
        Quote,
        Comment
    }
}
