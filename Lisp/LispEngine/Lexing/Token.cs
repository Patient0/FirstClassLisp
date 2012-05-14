using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LispEngine.Lexing
{
    public sealed class Token
    {
        private readonly TokenType type;
        private readonly string contents;

        public Token(TokenType type, string contents)
        {
            this.type = type;
            this.contents = contents;
        }

        public string Contents
        {
            get { return contents; }
        }

        public TokenType Type
        {
            get { return type; }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Token)) return false;
            return Equals((Token)obj);
        }

        public bool Equals(Token other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.type, type) && Equals(other.contents, contents);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (type.GetHashCode() * 397) ^ (contents != null ? contents.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", type, contents);
        }
    }

}
