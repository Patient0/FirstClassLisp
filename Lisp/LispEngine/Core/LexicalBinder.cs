using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;

namespace LispEngine.Core
{
    using FrameBindings = IStack<LexicalEnvironment.Binding>;
    using FrameBinder = Func<Datum, IStack<LexicalEnvironment.Binding>>;

    using Binder = Func<Datum, IStack<LexicalEnvironment.Binding>, IStack<LexicalEnvironment.Binding>>;

    class LexicalBinder : DatumHelpers
    {
        private static Binder matchExact(Datum d)
        {
            return (arg, frame) => d.Equals(arg) ? frame : null;
        }

        private static FrameBindings bindSymbol(Symbol symbol, Datum value, FrameBindings bindings)
        {
            return bindings.Push(new LexicalEnvironment.Binding(symbol, value));
        }

        private static Binder combine(Binder first, Binder second)
        {
            return (arg, frame) =>
            {
                var argPair = arg as Pair;
                if (argPair == null)
                    return null;
                var e = first(argPair.First, frame);
                return e == null ? null : second(argPair.Second, e);
            };
        }

        class BinderPartFactory : AbstractVisitor<Binder>
        {
            public override Binder defaultCase(Datum d)
            {
                throw error("'{0}' is not a valid argument list", d);
            }

            public override Binder visit(Null n)
            {
                return (arg, frame) => arg == nil ? frame : null;
            }

            public override Binder visit(Symbol s)
            {
                return (arg, frame) => bindSymbol(s, arg, frame);
            }

            public override Binder visit(Pair p)
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

            public override Binder visit(Atom a)
            {
                return matchExact(a);
            }
        }

        public static readonly DatumVisitor<Binder> Factory = new BinderPartFactory();

        private static Binder create(Datum d)
        {
            return d.accept(Factory);
        }
        
        public static FrameBinder Create(Datum argPattern)
        {
            var binder = create(argPattern);
            return arg => binder(arg, LexicalEnvironment.EmptyFrame);
        }
    }
}
