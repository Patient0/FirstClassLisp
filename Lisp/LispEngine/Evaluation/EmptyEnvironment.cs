using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    public sealed class EmptyEnvironment : ImmutableEnvironment
    {
        public Datum Lookup(string name)
        {
            throw new Exception(String.Format("Undefined symbol '{0}'", name));
        }

        private EmptyEnvironment()
        {
        }

        public static readonly ImmutableEnvironment Instance = new EmptyEnvironment();
    }
}
