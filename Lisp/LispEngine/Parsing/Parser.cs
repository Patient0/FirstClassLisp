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
        private readonly IEnumerator<Token> tokens;
        private Token next;
        public Parser(IEnumerable<Token> tokens)
        {
            // Skip whitespace
            this.tokens = tokens.Where(token => token.Type != TokenType.Space).GetEnumerator();
        }

        private void readNext()
        {
            next = tokens.MoveNext() ? tokens.Current : null;
        }

        private Datum symbol()
        {
            if (next.Type == TokenType.Symbol)
                return new Symbol(next.Contents);
            return null;
        }

        private Datum compound()
        {
            if (next.Type != TokenType.Open)
                return null;
            readNext();
            var elements = new List<Datum>();
            while(next.Type != TokenType.Close)
            {
                elements.Add(expression());
                readNext();
            }
            elements.Reverse();
            return elements.Aggregate(Null.Instance, (current, d) => cons(d, current));
        }

        private Datum atom()
        {
            if(next.Type == TokenType.Integer)
                return new Atom(int.Parse(next.Contents));
            return null;
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
