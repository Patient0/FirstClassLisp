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

        public void Define(Symbol name, Datum value)
        {
            env = env.Extend(name, value);
        }

        public void Set(Symbol name, Datum newValue)
        {
            env.Set(name, newValue);
        }

        public bool TryLookup(Symbol name, out Datum datum)
        {
            return env.TryLookup(name, out datum);
        }

        public Datum Lookup(Symbol name)
        {
            Datum datum;
            if (env.TryLookup(name, out datum))
                return datum;
            throw new Exception(String.Format("Undefined symbol '{0}'", name));
        }

        public Environment Extend(Symbol name, Datum value)
        {
            return new Environment(env.Extend(name, value));
        }

        public Environment Extend(String identifier, Datum value)
        {
            // Only to be used as a convenience.
            return Extend(Symbol.GetSymbol(identifier), value);
        }

        // For debugging - attempt to find the name that has this value
        public Symbol ReverseLookup(Datum value)
        {
            return env.ReverseLookup(value);
        }
    }
}
