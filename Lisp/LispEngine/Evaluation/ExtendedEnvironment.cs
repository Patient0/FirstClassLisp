using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    internal class ExtendedEnvironment : IEnvironment
    {
        private readonly IEnvironment parent;
        private readonly string name;
        private readonly Datum value;

        public ExtendedEnvironment(IEnvironment parent, string name, Datum value)
        {
            this.parent = parent;
            this.name = name;
            this.value = value;
        }

        public Datum Lookup(string name)
        {
            IEnvironment e = this;
            // To reduce the size of the stack trace when
            // looking up names in an environment that
            // has lots of names, we loop iteratively
            // rather than the more elegant
            // recursive solution.
            var ee = e as ExtendedEnvironment;
            while (ee != null)
            {
                if (ee.name.Equals(name))
                    return ee.value;
                e = ee.parent;
                ee = e as ExtendedEnvironment;
            }
            return e.Lookup(name);
        }

        public IEnvironment Set(string name, Datum newValue)
        {
            if(this.name.Equals(name))
                return new ExtendedEnvironment(parent, name, newValue);
            // Name might not be defined at this level. Try getting modified
            // parent variable
            return new ExtendedEnvironment(parent.Set(name, newValue), this.name, this.value);
        }
    }

    public static class EnvironmentExtensions
    {
        public static IEnvironment Extend(this IEnvironment e, string name, Datum value)
        {
            return new ExtendedEnvironment(e, name, value);
        }

        public static Environment ToMutable(this IEnvironment e)
        {
            return new Environment(e);
        }
    }
}
