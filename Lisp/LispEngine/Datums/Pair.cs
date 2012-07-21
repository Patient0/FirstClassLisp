using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LispEngine.Datums
{
    public sealed class Pair : Datum
    {
        public Datum First { get; set; }
        public Datum Second { get; set; }

        public Pair(Datum first, Datum second)
        {
            this.First = first;
            this.Second = second;
        }

        public Datum Cache { get; set; }

        /**
         * Location of this Pair if it was read from
         * a file
         */
        public object Location { get; set; }

        private static Pair asPair(Datum d)
        {
            return d as Pair;
        }

        private class Writer
        {
            private readonly StringBuilder sb = new StringBuilder();
            private Boolean empty = true;
            public Writer()
            {
                sb.Append('(');
            }
            public void Write(object d)
            {
                if(!empty)
                    sb.Append(' ');
                sb.Append(d);
                empty = false;
            }

            public string GetString()
            {
                sb.Append(')');
                return sb.ToString();
            }
        }

        public override string ToString()
        {
            var abbreviation = DatumHelpers.isQuote(First);
            if(abbreviation != null)
            {
                var quoted = Second as Pair;
                if(quoted != null)
                    return string.Format("{0}{1}", abbreviation, quoted.First);
            }
            var writer = new Writer();
            Pair tail;
            Datum next = this;
            while( (tail = asPair(next)) != null)
            {
                writer.Write(tail.First);
                next = tail.Second;
            }
            if(next != Null.Instance)
            {
                writer.Write(".");
                writer.Write(next);
                
            }
            return writer.GetString();
        }

        public T accept<T>(DatumVisitor<T> visitor)
        {
            return visitor.visit(this);
        }

        public bool Equals(Pair other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.First, First) && Equals(other.Second, Second);
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
                return ((First != null ? First.GetHashCode() : 0)*397) ^ (Second != null ? Second.GetHashCode() : 0);
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
