using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace LispEngine.Datums
{
    public sealed class Atom : Datum
    {
        private readonly object value;

        public Atom(object value)
        {
            this.value = value;
        }

        public object Value
        {
            [DebuggerStepThrough]
            get { return value; }
        }

        public bool Equals(Atom other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.value, value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Atom)) return false;
            return Equals((Atom) obj);
        }

        public override int GetHashCode()
        {
            return (value != null ? value.GetHashCode() : 0);
        }

        public T accept<T>(DatumVisitor<T> visitor)
        {
            return visitor.visit(this);
        }

        public static bool operator ==(Atom left, Atom right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Atom left, Atom right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            if (true.Equals(value))
                return "#t";
            if (false.Equals(value))
                return "#f";
            return string.Format("{0}", value);
        }
    }
}
