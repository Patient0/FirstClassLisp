using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    public sealed class EmptyEnvironment : Environment
    {
        public Datum lookup(string name)
        {
            throw new Exception(String.Format("Undefined symbol '{0}'", name));
        }

        private EmptyEnvironment()
        {
        }

        public static readonly Environment Instance = new EmptyEnvironment();
    }
}
