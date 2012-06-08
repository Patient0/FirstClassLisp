using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    // An environment that can be mutated. Used to implement define, maybe
    // other things also.
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

        public Datum Lookup(string name)
        {
            return env.Lookup(name);
        }

        public Environment Extend(string name, Datum value)
        {
            return new Environment(env.Extend(name, value));
        }
    }
}
