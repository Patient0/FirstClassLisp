using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    using FrameBindings = Tuple<IStack<Symbol>, IStack<Datum>>;

    class LexicalEnvironment
    {
        private readonly LexicalEnvironment parent;
        private IStack<Symbol> symbols;
        private IStack<Datum> LexicalBindings;
        private LexicalEnvironment (LexicalEnvironment parent, IStack<Symbol> symbols, IStack<Datum> LexicalBindings)
        {
            this.parent = parent;
            this.symbols = symbols;
            this.LexicalBindings = LexicalBindings;
        }

        private static readonly IStack<Symbol> emptySymbols = Stack<Symbol>.Empty.Push(null);
        private static readonly IStack<Datum> emptyDatums = Stack<Datum>.Empty;

        public static FrameBindings EmptyFrame = Tuple.Create(emptySymbols, emptyDatums);

        private static LexicalEnvironment newFrame(LexicalEnvironment parent)
        {
            return new LexicalEnvironment(parent, emptySymbols, emptyDatums);
        }

        public LexicalEnvironment NewFrame()
        {
            return newFrame(this);
        }

        public static LexicalEnvironment Create()
        {
            return newFrame(null);
        }

        public void Define(Symbol name, Datum binding)
        {
            this.symbols = symbols.Push(name);
            this.LexicalBindings = LexicalBindings.Push(binding);
        }

        public Datum Lookup(Symbol symbol)
        {
            var s = symbols;
            var b = LexicalBindings;
            while(s.Peek() != null)
            {
                if(symbol == s.Peek())
                    return b.Peek();
                b = b.Pop();
                s = s.Pop();
            }
            if (parent != null)
                return parent.Lookup(symbol);
            return null;
        }
    }
}
