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
        private readonly int id;

        // These two used for optimizing lookups
        public LexicalEnvironment Env { get; set; }
        public LexicalEnvironment.Binding CachedBinding { get; set; }

        private Symbol(string identifier, int id)
        {
            this.identifier = identifier;
            this.id = id;
        }

        public int ID
        {
            get { return id; }
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

        public Symbol clone()
        {
            return new Symbol(identifier, id);
        }

        /*
         * Optimize symbol comparison by
         * interning all symbols.
         */
        class SymbolFactory
        {
            private int counter;
            private readonly IDictionary<string, int> ids = new Dictionary<string, int>();

            private int GetId(string identifier)
            {
                int id;
                if (ids.TryGetValue(identifier, out id))
                    return id;
                id = ++counter;
                ids[identifier] = id;
                return id;
            }

            public Symbol GetSymbol(string identifier)
            {
                return new Symbol(identifier, GetId(identifier));
            }

            public Symbol Unique()
            {
                ++counter;
                return new Symbol(string.Format("!!unique-{0}", counter), counter);
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
            return SymbolFactory.Instance.GetSymbol(identifier);
        }

        public static Symbol GenUnique()
        {
            return SymbolFactory.Instance.Unique();
        }

        public bool Equals(Symbol other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.id == id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Symbol)) return false;
            return Equals((Symbol) obj);
        }

        public override int GetHashCode()
        {
            return id;
        }

        public static bool operator ==(Symbol left, Symbol right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Symbol left, Symbol right)
        {
            return !Equals(left, right);
        }
    }
}
