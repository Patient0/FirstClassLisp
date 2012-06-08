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
            return this.name == name ? value : parent.Lookup(name);
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
