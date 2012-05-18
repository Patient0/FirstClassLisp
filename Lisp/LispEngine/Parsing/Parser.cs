using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Lexing;

namespace LispEngine.Parsing
{
    public sealed class Parser : DatumHelpers
    {
        private readonly Scanner s;
        private readonly IEnumerator<Token> tokens;
        private Token next;
        public Parser(Scanner s)
        {
            this.s = s;
            // Skip whitespace
            tokens = s.Scan().Where(token => token.Type != TokenType.Space).GetEnumerator();
        }

        private void readNext()
        {
            next = tokens.MoveNext() ? tokens.Current : null;
        }

        private void fail(String fmt, params object[] args)
        {
            var errorMsg = string.Format(fmt, args);
            var line = s.LineSoFar;
            var msg = string.Format("\n{0}\n{1}^: {2}", line, new string(' ', line.Length), errorMsg);
            throw new ParseException(msg);
        }

        private void expectNext(string what)
        {
            readNext();
            if(next == null)
                fail("Expected '{0}'", what);
        }

        private Datum symbol()
        {
            return next.Type == TokenType.Symbol ? new Symbol(next.Contents) : null;
        }

        private Datum readCdr()
        {
            expectNext(")");
            var cdr = expression();
            expectNext(")");
            if (next.Type != TokenType.Close)
                fail("more than one item found after dot (.)");
            return cdr;
        }

        private Datum compound()
        {
            if (next.Type != TokenType.Open)
                return null;
            readNext();
            var elements = new List<Datum>();
            var cdr = nil;
            while(next.Type != TokenType.Close)
            {
                if (next.Type == TokenType.Dot)
                {
                    cdr = readCdr();
                    break;
                }
                elements.Add(expression());
                expectNext(")");
            }
            elements.Reverse();
            return elements.Aggregate(cdr, (current, d) => cons(d, current));
        }

        private Datum atom()
        {
            return next.Type == TokenType.Integer ? new Atom(int.Parse(next.Contents)) : null;
        }

        // Based on the token that was just read, turn it into an expression
        private Datum expression()
        {
            Datum d;
            if ((d = symbol()) != null)
                return d;
            if ((d = atom()) != null)
                return d;
            if ((d = compound()) != null)
                return d;
            throw new Exception(string.Format("Unexpected token: {0}", next));
        }

        public Datum parse()
        {
            readNext();
            return Eof ? null : expression();
        }

        private bool Eof
        {
            get
            {
                return next == null;
            }
        }
    }
}
