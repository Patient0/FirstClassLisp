using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    // Abstract Environment implementation which is immutable
    // w.r.t the set of names that it defines.
    // It's not immutable w.r.t the LexicalBindings that each name has.
    public interface IEnvironment
    {
        bool TryLookup(Symbol identifier, out Datum datum);
        void Set(Symbol identifier, Datum newValue);
        Symbol ReverseLookup(Datum value);
        void dump(TextWriter output);
    }
}
