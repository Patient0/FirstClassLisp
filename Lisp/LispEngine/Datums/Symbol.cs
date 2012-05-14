using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LispEngine.Datums
{
    public sealed class Symbol : Datum
    {
        private readonly string identifier;
        public Symbol(string identifier)
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

        public bool Equals(Symbol other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.identifier, identifier);
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
            return identifier.GetHashCode();
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
