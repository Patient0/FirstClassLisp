using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LispEngine.Evaluation
{
    public interface Environment
    {
        object lookup(string name);
    }
}
