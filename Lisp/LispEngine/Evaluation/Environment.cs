using System;
using System.Collections.Generic;
using System.IO;
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
        public Environment(IEnvironment env)
        {
            this.IEnv = env;
        }

        public IEnvironment IEnv { get; private set; }

        public void Define(Symbol name, Datum value)
        {
            IEnv = IEnv.Extend(name, value);
        }

        public void Set(Symbol name, Datum newValue)
        {
            IEnv.Set(name, newValue);
        }

        public bool TryLookup(Symbol name, out Datum datum)
        {
            return IEnv.TryLookup(name, out datum);
        }

        public Datum Lookup(Symbol name)
        {
            Datum datum;
            if (IEnv.TryLookup(name, out datum))
                return datum;
            throw new Exception(String.Format("Undefined symbol '{0}'", name));
        }

        public Environment Extend(Symbol name, Datum value)
        {
            return new Environment(IEnv.Extend(name, value));
        }

        public Environment Extend(String identifier, Datum value)
        {
            // Only to be used as a convenience.
            return Extend(Symbol.GetSymbol(identifier), value);
        }

        // For debugging - attempt to find the name that has this value
        public Symbol ReverseLookup(Datum value)
        {
            return IEnv.ReverseLookup(value);
        }

        public void dump(TextWriter output)
        {
            IEnv.dump(output);
        }
    }
}
