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
            // Skip whitespace and comments
            tokens = s.Scan().Where(token => token.Type != TokenType.Space && token.Type != TokenType.Comment).GetEnumerator();
        }

        private void readNext()
        {
            next = tokens.MoveNext() ? tokens.Current : null;
        }

        private ParseException fail(String fmt, params object[] args)
        {
            return s.fail(fmt, args);
        }

        private void expectNext(string what)
        {
            readNext();
            if(next == null)
                throw fail("Expected '{0}'", what);
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
                throw fail("more than one item found after dot (.)");
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
            if (next.Type == TokenType.Integer)
                return atom(int.Parse(next.Contents));
            if(next.Type == TokenType.Boolean)
                return atom(next.Contents.ToLower().Equals("#t"));
            return null;
        }

        private static Datum isQuote(TokenType type)
        {
            switch(type)
            {
                case TokenType.Quote:
                    return quote;
                case TokenType.Unquote:
                    return unquote;
                case TokenType.QuasiQuote:
                    return quasiquote;
                case TokenType.UnquoteSplicing:
                    return unquoteSplicing;
            }
            return null;
        }

        private Datum quotedExpression()
        {
            var symbol = isQuote(next.Type);
            if(symbol != null)
            {
                var expression = parse();
                return cons(symbol, compound(expression));
            }
            return null;
        }

        // Based on the token that was just read, turn it into an expression
        private Datum expression()
        {
            Datum d;
            if ((d = quotedExpression()) != null)
                return d;
            if ((d = symbol()) != null)
                return d;
            if ((d = atom()) != null)
                return d;
            if ((d = compound()) != null)
                return d;
            throw fail("Unexpected token: {0}", next);
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
