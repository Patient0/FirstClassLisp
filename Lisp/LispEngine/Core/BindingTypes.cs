using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;
using Environment = LispEngine.Evaluation.Environment;

namespace LispEngine.Core
{
    class BindingTypes : DatumHelpers
    {
        private sealed class Rest : Bindings
        {
            private readonly string identifier;
            public Rest(string identifier)
            {
                this.identifier = identifier;
            }

            public Environment apply(Environment to, Datum args)
            {
                return to.Extend(identifier, args);
            }

            public override string ToString()
            {
                return identifier;
            }
        }

        private static Bindings rest(string identifier)
        {
            return new Rest(identifier);
        }

        private sealed class PairBindings : Bindings
        {
            private readonly Bindings first;
            private readonly Bindings second;

            public PairBindings(Bindings first, Bindings second)
            {
                this.first = first;
                this.second = second;
            }
            public Environment apply(Environment to, Datum args)
            {
                var argPair = args as Pair;
                if (argPair == null)
                    return null;
                var env = first.apply(to, argPair.First);
                return env == null ? null : second.apply(env, argPair.Second);
            }

            public override string ToString()
            {
                return string.Format("({0} . {1})", first, second);
            }
        }

        private sealed class AtomBindings : Bindings
        {
            private readonly Atom atom;
            public AtomBindings(Atom atom)
            {
                this.atom = atom;
            }

            public Environment apply(Environment to, Datum args)
            {
                return atom.Equals(args) ? to : null;
            }
        }

        private sealed class EmptyBindings : Bindings
        {
            public static readonly Bindings Instance = new EmptyBindings();
            public Environment apply(Environment to, Datum args)
            {
                return args != nil ? null : to;
            }

            public override string ToString()
            {
                return "()";
            }
        }

        private static Bindings pair(Bindings first, Bindings second)
        {
            return new PairBindings(first, second);
        }

        public static Bindings parse(Datum args)
        {
            if (args == nil)
                return EmptyBindings.Instance;
            var s = args as Symbol;
            if (s != null)
                return rest(s.Identifier);
            var p = args as Pair;
            if(p != null)
                return pair(parse(p.First), parse(p.Second));
            var a = args as Atom;
            if(a != null)
                return new AtomBindings(a);
            throw error("'{0}' is not a valid argument list", args);
        }
    }
}
