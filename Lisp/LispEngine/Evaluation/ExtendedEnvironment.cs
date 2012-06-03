using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    internal class ExtendedEnvironment : ImmutableEnvironment
    {
        private readonly ImmutableEnvironment parent;
        private readonly string name;
        private readonly Datum value;

        public ExtendedEnvironment(ImmutableEnvironment parent, string name, Datum value)
        {
            this.parent = parent;
            this.name = name;
            this.value = value;
        }

        public Datum Lookup(string name)
        {
            if (this.name == name)
                return value;
            return parent.Lookup(name);
        }
    }

    public static class EnvironmentExtensions
    {
        public static ImmutableEnvironment Extend(this ImmutableEnvironment e, string name, Datum value)
        {
            return new ExtendedEnvironment(e, name, value);
        }

        public static Environment ToMutable(this ImmutableEnvironment e)
        {
            return new Environment(e);
        }
    }
}
