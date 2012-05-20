using System.Collections.Generic;
using System.IO;
using System.Text;
using LispEngine.Parsing;

namespace LispEngine.Lexing
{
    public class Scanner
    {
        private readonly TextReader input;
        private readonly StringBuilder lineSoFar = new StringBuilder();
        private StringBuilder sb;

        public static Scanner Create(string s)
        {
            return new Scanner(new StringReader(s));
        }

        public Scanner(TextReader input)
        {
            this.input = input;
        }

        private char peek()
        {
            return (char) input.Peek();
        }

        private void readChar()
        {
            var next = (char) input.Read();
            sb.Append(next);
            lineSoFar.Append(next);
        }

        private bool more()
        {
            return input.Peek() != -1;
        }

        private Token tok(TokenType type)
        {
            return sb.Length > 0 ? new Token(type, sb.ToString()) : null;
        }

        private bool isLetter()
        {
            return more() && char.IsLetter(peek());
        }

        private bool isDigit()
        {
            return more() && char.IsDigit(peek());
        }

        private bool isWhiteSpace()
        {
            return more() && char.IsWhiteSpace(peek());
        }

        // Based on http://people.csail.mit.edu/jaffer/r5rs_9.html
        private Token dot()
        {
            if (peek() == '.')
            {
                readChar();
                return tok(TokenType.Dot);                
            }
            return null;
        }

        private bool isOneOf(string chars)
        {
            return more() && chars.IndexOf(peek()) != -1;
        }

        private bool isSpecialSubsequent()
        {
            return isOneOf("+-.@");
        }

        private bool isSpecialInitial()
        {
            return isOneOf("!$%&+-*/:<=>?^_~");
        }

        private bool isInitial()
        {
            return isLetter() || isSpecialInitial();
        }

        private bool isSubsequent()
        {
            return isInitial() || isDigit() || isSpecialSubsequent();
        }

        private Token symbol()
        {
            if(!isInitial())
                return null;
            readChar();
            while(isSubsequent())
                readChar();
            return tok(TokenType.Symbol);
        }

        private Token space()
        {
            while(isWhiteSpace())
                readChar();
            return tok(TokenType.Space);
        }

        private Token integer()
        {
            while (isDigit())
                readChar();
            
            return tok(TokenType.Integer);
        }

        private Token openClose()
        {
            if (peek() == '(')
            {
                readChar();
                return tok(TokenType.Open);
            }
            if (peek() == ')')
            {
                readChar();
                return tok(TokenType.Close);
            }
            return null;
        }

        private Token boolean()
        {
            if (peek() != '#')
                return null;
            readChar();
            if(isOneOf("tfTF"))
            {
                readChar();
                return tok(TokenType.Boolean);
            }
            throw fail("Unrecognized token");
        }

        public Token GetNext()
        {
            Token t;
            sb = new StringBuilder();
            if (!more())
                return null;
            if ((t = symbol()) != null)
                return t;
            if ((t = space()) != null)
                return t;
            if ((t = integer()) != null)
                return t;
            if ((t = openClose()) != null)
                return t;
            if ((t = dot()) != null)
                return t;
            if ((t = boolean()) != null)
                return t;
            return null;
        }

        public IEnumerable<Token> Scan()
        {
            Token next;
            while ((next = GetNext()) != null)
                yield return next;
        }

        public string LineSoFar
        {
            get
            {
                return lineSoFar.ToString();
            }
        }

        public ParseException fail(string fmt, params object[] args)
        {
            var errorMsg = string.Format(fmt, args);
            var line = LineSoFar;
            var msg = string.Format("\n{0}\n{1}^: {2}", line, new string(' ', line.Length), errorMsg);
            return new ParseException(msg);
        }
    }
}
