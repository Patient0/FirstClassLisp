using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    // Abstract Environment implementation which is immutable
    // w.r.t the set of names that it defines.
    // It's not immutable w.r.t the bindings that each name has.
    public interface IEnvironment
    {
        bool TryLookup(string name, out Datum datum);
        void Set(string name, Datum newValue);
    }
}
