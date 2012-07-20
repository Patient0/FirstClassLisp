using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    public sealed class EmptyEnvironment : IEnvironment
    {
        public bool TryLookup(string name, out Datum datum)
        {
            datum = null;
            return false;
        }

        public void Set(string name, Datum newValue)
        {
            throw new Exception(String.Format("Symbol '{0}' not defined. Cannot be set.", name));
        }

        public string ReverseLookup(Datum value)
        {
            return null;
        }

        private EmptyEnvironment()
        {
        }

        public static readonly IEnvironment Instance = new EmptyEnvironment();
    }
}
