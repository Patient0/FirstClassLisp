using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LispEngine.Evaluation
{
    public interface Function
    {
        object evaluate(IEnumerable<object> args);
    }
}
