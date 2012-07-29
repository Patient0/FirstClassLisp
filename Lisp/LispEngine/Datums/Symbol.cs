using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
