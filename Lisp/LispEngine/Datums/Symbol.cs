using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Evaluation;

namespace LispEngine.Datums
{
    public sealed class Symbol : Datum
    {
        private readonly string identifier;

        private Symbol(string identifier)
        {
            this.identifier = identifier;
        }

        public string Identifier
        {
            get { return identifier; }
        }

        public override string ToString()
        {
            return identifier;
        }

        public T accept<T>(DatumVisitor<T> visitor)
        {
            return visitor.visit(this);
        }

        /*
         * Optimize symbol comparison by
         * interning all symbols.
         */
        class SymbolFactory
        {
            private int counter;
            private readonly IDictionary<string, Symbol> symbols = new Dictionary<string, Symbol>();

            public Symbol Get(string identifier)
            {
                Symbol s;
                if (symbols.TryGetValue(identifier, out s))
                    return s;
                s = new Symbol(identifier);
                symbols[identifier] = s;
                return s;
            }

            public Symbol Unique()
            {
                ++counter;
                return new Symbol(string.Format("!!unique-{0}", counter));
            }

            private SymbolFactory()
            {
            }

            private static readonly SymbolFactory instance = new SymbolFactory();

            public static SymbolFactory Instance
            {
                get { return instance; }
            }
        }

        public static Symbol GetSymbol(string identifier)
        {
            return SymbolFactory.Instance.Get(identifier);
        }

        public static Symbol GenUnique()
        {
            return SymbolFactory.Instance.Unique();
        }
    }
}
