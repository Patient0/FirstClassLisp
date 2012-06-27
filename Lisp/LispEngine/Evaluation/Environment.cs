using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    // Environment supports mutable extension via Define,
    // and mutable modification of existing values via
    // Set.
    public sealed class Environment : IEnvironment
    {
        private IEnvironment env;
        public Environment(IEnvironment env)
        {
            this.env = env;
        }

        public void Define(string name, Datum value)
        {
            env = env.Extend(name, value);
        }

        public void Set(string name, Datum newValue)
        {
            env.Set(name, newValue);
        }

        public bool TryLookup(string name, out Datum datum)
        {
            return env.TryLookup(name, out datum);
        }

        public Datum Lookup(string name)
        {
            Datum datum;
            if (env.TryLookup(name, out datum))
                return datum;
            else
                throw new Exception(String.Format("Undefined symbol '{0}'", name));
        }

        public Environment Extend(string name, Datum value)
        {
            return new Environment(env.Extend(name, value));
        }
    }
}
