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
        private sealed class Name : Bindings
        {
            private readonly string identifier;
            public Name(string identifier)
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

        private sealed class MatchPair : Bindings
        {
            private readonly Bindings first;
            private readonly Bindings second;

            public MatchPair(Bindings first, Bindings second)
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

        private sealed class MatchExact : Bindings
        {
            private readonly Datum item;
            public MatchExact(Datum item)
            {
                this.item = item;
            }

            public Environment apply(Environment to, Datum args)
            {
                return item.Equals(args) ? to : null;
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

        public static Bindings parse(Datum args)
        {
            if (args == nil)
                return EmptyBindings.Instance;
            var s = args as Symbol;
            if (s != null)
                return new Name(s.Identifier);
            var p = args as Pair;
            if (p != null)
            {
                if (quote.Equals(p.First))
                {
                    var quoted = p.Second as Pair;
                    if(quoted != null)
                        return new MatchExact(quoted.First);
                }
                return new MatchPair(parse(p.First), parse(p.Second));
            }
            var a = args as Atom;
            if(a != null)
                return new MatchExact(a);
            throw error("'{0}' is not a valid argument list", args);
        }
    }
}
