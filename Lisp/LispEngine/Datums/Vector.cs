using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LispEngine.Datums
{
    public sealed class Vector : Datum
    {
        private readonly Datum[] elements;
        public Vector(Datum[] elements)
        {
            this.elements = elements;
        }

        public Datum[] Elements
        {
            get { return elements; }
        }

        public T accept<T>(DatumVisitor<T> visitor)
        {
            return visitor.visit(this);
        }

        public override string ToString()
        {
            var s = new StringBuilder();
            s.Append("#(");
            var second = false;
            foreach(var d in elements)
            {
                if (second)
                    s.Append(' ');
                s.Append(d.ToString());
                second = true;
            }
            s.Append(")");
            return s.ToString();
        }

        public override int GetHashCode()
        {
            return elements.Aggregate(elements.Length, (current, d) => current*17 + d.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            var rhs = obj as Vector;
            if (rhs == null)
                return false;
            if (elements.Length != rhs.Elements.Length)
                return false;
            return !elements.Where((t, i) => !t.Equals(rhs.elements[i])).Any();
        }
    }
}
