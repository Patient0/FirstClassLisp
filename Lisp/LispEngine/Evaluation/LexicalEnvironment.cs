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
            private readonly Symbol symbol;
            public Binding(Symbol symbol, Datum value)
            {
                this.symbol = symbol;
                this.Value = value;
            }

            public Datum Value { get; set; }

            public Symbol Symbol
            {
                get { return symbol; }
            }
        }
        private readonly LexicalEnvironment parent;
        private IStack<Binding> bindings;
        private LexicalEnvironment (LexicalEnvironment parent, IStack<Binding> bindings)
        {
            this.parent = parent;
            this.bindings = bindings;
        }

        public static FrameBindings EmptyFrame = Stack<Binding>.Empty.Push(null);

        private static LexicalEnvironment newFrame(LexicalEnvironment parent, FrameBindings bindings)
        {
            return new LexicalEnvironment(parent, bindings);
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

        private Binding find(Symbol symbol)
        {
            // TODO: We will optimize this
            var b = bindings;
            while (b.Peek() != null)
            {
                if (symbol == b.Peek().Symbol)
                    return b.Peek();
                b = b.Pop();
            }
            if (parent == null)
                throw DatumHelpers.error("Undefined symbol '{0}'", symbol);
            return parent.find(symbol);
        }

        public Datum Lookup(Symbol symbol)
        {
            return find(symbol).Value;
        }

        public void Set(Symbol symbol, Datum result)
        {
            find(symbol).Value = result;
        }
    }
}
