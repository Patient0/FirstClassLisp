using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    // An immutable environment
    public interface IEnvironment
    {
        Datum Lookup(string name);
    }
}
