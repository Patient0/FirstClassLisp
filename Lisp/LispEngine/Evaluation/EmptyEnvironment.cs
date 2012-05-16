using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LispEngine.Evaluation
{
    public sealed class EmptyEnvironment : Environment
    {
        public object lookup(string name)
        {
            throw new Exception(String.Format("Undefined symbol '{0}'", name));
        }

        private EmptyEnvironment()
        {
        }

        public static readonly Environment Instance = new EmptyEnvironment();
    }
}
