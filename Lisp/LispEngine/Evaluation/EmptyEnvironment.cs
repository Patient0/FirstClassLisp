using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    public sealed class EmptyEnvironment : IEnvironment
    {
        public bool TryLookup(Symbol name, out Datum datum)
        {
            datum = null;
            return false;
        }

        public void Set(Symbol name, Datum newValue)
        {
            throw new Exception(String.Format("Symbol '{0}' not defined. Cannot be set.", name));
        }

        public Symbol ReverseLookup(Datum value)
        {
            return null;
        }

        public void dump(TextWriter output)
        {
        }

        private EmptyEnvironment()
        {
        }

        public static readonly IEnvironment Instance = new EmptyEnvironment();
    }
}
