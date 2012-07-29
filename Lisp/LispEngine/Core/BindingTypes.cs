using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;

namespace LispEngine.Core
{
    class BindingTypes : DatumHelpers
    {
        private static Bindings matchExact(Datum d)
        {
            return (env, args) => d.Equals(args) ? env : null;
        }

        private static Bindings combine(Bindings first, Bindings second)
        {
            return (env, args) =>
                       {
                           var argPair = args as Pair;
                           if (argPair == null)
                               return null;
                           var e = first(env, argPair.First);
                           return e == null ? null : second(e, argPair.Second);
                       };
        }

        class Visitor : AbstractVisitor<Bindings>
        {
            public override Bindings defaultCase(Datum d)
            {
                throw error("'{0}' is not a valid argument list", d);
            }
            public override Bindings visit(Null n)
            {
                return (env, args) => args == nil ? env : null;
            }
            public override Bindings visit(Symbol s)
            {
                return (env, args) => env.Extend(s, args);
            }
            public override Bindings visit(Atom a)
            {
                return matchExact(a);
            }
            public override Bindings visit(Pair p)
            {
                // Quoted instances also form an exact match
                if (quote.Equals(p.First))
                {
                    var quoted = p.Second as Pair;
                    if (quoted != null)
                        return matchExact(quoted.First);
                }

                return combine(p.First.accept(this), p.Second.accept(this));
            }

            public static readonly DatumVisitor<Bindings> Instance = new Visitor();
        }

        public static Bindings parse(Datum args)
        {
            return args.accept(Visitor.Instance);
        }
    }
}
