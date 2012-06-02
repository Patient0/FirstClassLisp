using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private delegate void MatchDelegate(Scanner s);

        private sealed class Matcher
        {
            public TokenType TokenType
            {
                [DebuggerStepThrough]
                get { return tokenType; }
            }

            public MatchDelegate MatchDelegate
            {
                [DebuggerStepThrough]
                get { return matchDelegate; }
            }

            private readonly TokenType tokenType;
            private readonly MatchDelegate matchDelegate;
            public Matcher(TokenType tokenType, MatchDelegate matchDelegate)
            {
                this.tokenType = tokenType;
                this.matchDelegate = matchDelegate;
            }
        }

        private static Matcher match(TokenType tokenType, MatchDelegate matchDelegate)
        {
            return new Matcher(tokenType, matchDelegate);
        }

        // Used to match a single token
        private static Matcher match(TokenType tokenType, char c)
        {
            return match(tokenType, s =>
                { if (s.peek() == c)
                    s.readChar(); });
        }

        // Match characters until the given predicate returns false
        private static Matcher match(TokenType tokenType, Func<Scanner, bool>  predicate)
        {
            return match(tokenType, s =>
                    {
                        while(predicate(s))
                            s.readChar();
                    }
                 );
        }

        private static readonly Matcher[] matchers = new[]
                                                         {
                                                            match(TokenType.Quote, '\''),
                                                            match(TokenType.Symbol,
                                                                   s =>
                                                                       {
                                                                           if (!s.isInitial())
                                                                               return;
                                                                           s.readChar();
                                                                           while (s.isSubsequent())
                                                                               s.readChar();
                                                                       }),
                                                            match(TokenType.Space, 
                                                                s => s.isWhiteSpace()),
                                                            match(TokenType.Integer, 
                                                                s => s.isDigit()),
                                                            match(TokenType.Open, '('),
                                                            match(TokenType.Close, ')'),
                                                            match(TokenType.Dot, '.'),
                                                            match(TokenType.Boolean,
                                                                s =>
                                                                    {
                                                                        if (s.peek() != '#')
                                                                            return;
                                                                        s.readChar();
                                                                        if(s.isOneOf("tfTF"))
                                                                        {
                                                                            s.readChar();
                                                                            return;
                                                                        }
                                                                        throw s.fail("Unrecognized token");
                                                                    })
                                                         };

        private Token match()
        {
            foreach(var m in matchers)
            {
                m.MatchDelegate(this);
                var token = tok(m.TokenType);
                if (token != null)
                    return token;
            }
            return null;
        }

        public Token GetNext()
        {
            sb = new StringBuilder();
            return !more() ? null : match();
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
