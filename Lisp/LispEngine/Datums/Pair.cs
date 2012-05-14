using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LispEngine.Datums
{
    public sealed class Pair : Datum
    {
        private readonly Datum first;
        private readonly Datum second;
        public Pair(Datum first, Datum second)
        {
            this.first = first;
            this.second = second;
        }

        public Datum Second
        {
            get { return second; }
        }

        public Datum First
        {
            get { return first; }
        }

        public override string ToString()
        {
            // Not ideal but good enough for now
            return string.Format("({0} . {1})", first, second);
        }

        public bool Equals(Pair other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.first, first) && Equals(other.second, second);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Pair)) return false;
            return Equals((Pair) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (first.GetHashCode()*397) ^ second.GetHashCode();
            }
        }

        public static bool operator ==(Pair left, Pair right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Pair left, Pair right)
        {
            return !Equals(left, right);
        }
    }
}
