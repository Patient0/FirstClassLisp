using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    using FrameBindings = IStack<LexicalEnvironment.Binding>;

    public class LexicalEnvironment
    {
        public class Binding
        {
            // Just for debugging
            private readonly Symbol symbol;
            private readonly int symbolId;

            public Binding(Symbol symbol, Datum value)
            {
                this.symbol = symbol;
                this.symbolId = symbol.ID;
                this.Value = value;
            }

            public Datum Value { get; set; }
            public int SymbolID { get { return symbolId; } }
            public Symbol Symbol { get { return symbol; } }
        }

        private Statistics statistics;
        private readonly LexicalEnvironment parent;
        private IStack<Binding> bindings;

        private LexicalEnvironment(LexicalEnvironment parent, IStack<Binding> bindings)
        {
            this.statistics = parent == null ? null : parent.statistics;
            this.parent = parent;
            this.bindings = bindings;
        }

        public static FrameBindings EmptyFrame = Stack<Binding>.Empty.Push(null);

        private static LexicalEnvironment newFrame(LexicalEnvironment parent, FrameBindings bindings)
        {
            return new LexicalEnvironment(parent, bindings);
        }

        public Statistics Statistics
        {
            set { statistics = value; }
        }

        public LexicalEnvironment NewFrame(FrameBindings frameBindings)
        {
            return newFrame(this, frameBindings);
        }

        public LexicalEnvironment NewFrame()
        {
            return NewFrame(EmptyFrame);
        }

        public static LexicalEnvironment Create()
        {
            return newFrame(null, EmptyFrame);
        }

        public LexicalEnvironment Define(Symbol name, Datum value)
        {
            this.bindings = bindings.Push(new Binding(name, value));
            return this;
        }

        public LexicalEnvironment Define(string name, Datum binding)
        {
            return Define(Symbol.GetSymbol(name), binding);
        }

        private static Exception undefined(Symbol symbol)
        {
            return DatumHelpers.error("Undefined symbol '{0}'", symbol);
        }

        private Binding findInFrame(int id)
        {
            var b = bindings;
            Binding binding;
            while( (binding = b.Peek()) != null)
            {
                if (binding.SymbolID == id)
                    return binding;
                b = b.Pop();
            }
            return null;
        }

        private static Binding checkCached(LexicalEnvironment e, Symbol symbol)
        {
            if (symbol.Env == null)
                return null;
            while(e != null)
            {
                if (ReferenceEquals(symbol.Env, e))
                    return symbol.CachedBinding;
                e = e.parent;
            }
            return null;
        }

        private static Binding findAndCache(LexicalEnvironment e, Symbol symbol)
        {
            if (e.statistics != null)
                e.statistics.Lookups++;
            var id = symbol.ID;
            while(e != null)
            {
                var b = e.findInFrame(id);
                if (b != null)
                {
                    symbol.Env = e;
                    return (symbol.CachedBinding = b);
                }
                e = e.parent;
            }
            throw undefined(symbol);
        }

        public Binding Find(Symbol symbol)
        {
            return checkCached(this, symbol) ?? findAndCache(this, symbol);
        }
         
        public void Set(Symbol symbol, Datum value)
        {
            Find(symbol).Value = value;
        }

        public Datum Lookup(Symbol symbol)
        {
            return Find(symbol).Value;
        }
    }
}
