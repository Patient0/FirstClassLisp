using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    internal class ExtendedEnvironment : Environment
    {
        private readonly Environment parent;
        private readonly string name;
        private readonly Datum value;

        public ExtendedEnvironment(Environment parent, string name, Datum value)
        {
            this.parent = parent;
            this.name = name;
            this.value = value;
        }

        public Datum lookup(string name)
        {
            if (this.name == name)
                return value;
            return parent.lookup(name);
        }
    }

    public static class EnvironmentExtensions
    {
        public static Environment Extend(this Environment e, string name, Datum value)
        {
            return new ExtendedEnvironment(e, name, value);
        }
    }
}
