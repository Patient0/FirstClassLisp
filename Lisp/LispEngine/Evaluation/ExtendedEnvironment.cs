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
        private Datum value;

        public ExtendedEnvironment(IEnvironment parent, string name, Datum value)
        {
            this.parent = parent;
            this.name = name;
            this.value = value;
        }

        private ExtendedEnvironment find(string name, out IEnvironment e)
        {
            e = this;
            // To reduce the size of the stack trace when
            // looking up names in an environment that
            // has lots of names, we loop iteratively
            // rather than the more elegant
            // recursive solution.
            var ee = e as ExtendedEnvironment;
            while (ee != null)
            {
                if (ee.name.Equals(name))
                    return ee;
                e = ee.parent;
                ee = e as ExtendedEnvironment;
            }
            return null;
        }

        public Datum Lookup(string name)
        {
            IEnvironment p;
            var e = find(name, out p);
            return e == null ? p.Lookup(name) : e.value;
        }

        public void Set(string name, Datum newValue)
        {
            IEnvironment p;
            var e = find(name, out p);
            if(e == null)
                p.Set(name, newValue);
            else
                e.value = newValue;
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
