using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CSharp;

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

        // http://stackoverflow.com/questions/323640/can-i-convert-a-c-sharp-string-value-to-an-escaped-string-literal
        // Seems the simplest approach, although I have no idea how performant it is.
        private static readonly CSharpCodeProvider provider = new CSharpCodeProvider();
        private static string ToLiteral(string input)
        {
            var writer = new StringWriter();
            provider.GenerateCodeFromExpression(new CodePrimitiveExpression(input), writer, null);
            return writer.GetStringBuilder().ToString();
        }

        public override string ToString()
        {
            if (true.Equals(value))
                return "#t";
            if (false.Equals(value))
                return "#f";
            var s = value as string;
            if (s != null)
                return ToLiteral(s);
            return string.Format("{0}", value);
        }
    }
}
