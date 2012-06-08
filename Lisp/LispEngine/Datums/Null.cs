using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LispEngine.Datums
{
    public sealed class Null : Datum
    {
        private Null()
        {
        }

        public static readonly Datum Instance = new Null();

        public override int GetHashCode()
        {
            return 0;
        }

        public T accept<T>(DatumVisitor<T> visitor)
        {
            return visitor.visit(this);
        }

        public override bool Equals(object obj)
        {
            return obj as Null != null;
        }

        public override string ToString()
        {
            return "()";
        }


    }
}
