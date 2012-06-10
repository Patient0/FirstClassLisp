using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    public sealed class EmptyEnvironment : IEnvironment
    {
        public Datum Lookup(string name)
        {
            throw new Exception(String.Format("Undefined symbol '{0}'", name));
        }

        public IEnvironment Set(string name, Datum newValue)
        {
            throw new Exception(String.Format("Symbol '{0}' not defined. Cannot be set.", name));
        }

        private EmptyEnvironment()
        {
        }

        public static readonly IEnvironment Instance = new EmptyEnvironment();
    }
}
