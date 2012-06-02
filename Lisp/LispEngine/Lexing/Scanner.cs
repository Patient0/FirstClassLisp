using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using LispEngine.Parsing;

namespace LispEngine.Lexing
{
    public class Scanner
    {
        private readonly TextReader input;
        private String filename = "";

        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }

        private int lineNumber = 1;

        public int LineNumber
        {
            [DebuggerStepThrough]
            get { return lineNumber; }
        }

        private readonly IList<string> previousLines = new List<string>();
        private readonly StringBuilder lineSoFar = new StringBuilder();

        private delegate TokenType? Matcher(Scanner s);

        private static Matcher match(TokenType tokenType, Action<Scanner> matchDelegate)
        {
            return s =>
                    {
                        matchDelegate(s);
                        TokenType? result = null;
                        if (s.sb.Length > 0)
                            result = tokenType;
                        return result;
                    };
        }

        // Used to match a single character
        private static Matcher match(TokenType tokenType, char c)
        {
            return match(tokenType,
                        s =>
                        {
                            if (s.peek() == c)
                                s.readChar();
                        });
        }

        // Match characters until the given predicate returns false
        private static Matcher match(TokenType tokenType, Func<Scanner, bool> predicate)
        {
            return match(tokenType,
                        s =>
                        {
                            while (predicate(s))
                                s.readChar();
                        }
                 );
        }

        private static TokenType? unquote(Scanner s)
        {
            if (s.peek() != ',')
                return null;
            s.readChar();
            if (s.peek() == '@')
            {
                s.readChar();
                return TokenType.UnquoteSplicing;
            }
            return TokenType.Unquote;
        }

        private static readonly Matcher[] matchers = new[]
                    {
                    match(TokenType.QuasiQuote, '`'),
                    unquote,
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
                    match(TokenType.Comment, 
                        s =>
                        {
                            if (s.peek() != ';')
                                return;
                            while (s.peek() != '\n')
                                s.readChar();
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
            if(next == '\n')
            {
                ++lineNumber;
                previousLines.Add(LineSoFar);
                if(previousLines.Count > 3)
                    previousLines.RemoveAt(0);
                lineSoFar.Clear();
            }
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

        private Token match()
        {
            return (from matcher in matchers
                    select matcher(this) into tokenType
                    where tokenType.HasValue
                    select tok(tokenType.Value)).FirstOrDefault();
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
                return lineSoFar.Replace("\r", "").Replace("\n", "").ToString();
            }
        }

        public ParseException fail(string fmt, params object[] args)
        {
            var errorMsg = string.Format(fmt, args);
            var line = LineSoFar;
            var w = new StringWriter();
            w.WriteLine("{0}({1}): {2}", Filename, LineNumber, errorMsg);
            var ln = LineNumber - previousLines.Count;
            foreach(var l in previousLines)
                w.WriteLine("{0}({1}): {2}", Filename, ln++, l);
            w.WriteLine("{0}({1}): {2}", Filename, ln++, line);
            w.WriteLine("{0}({1}): {2}^: {3}", Filename, ln++, new string(' ', line.Length), errorMsg);
            w.Flush();
            var msg = w.ToString();
            return new ParseException(msg);
        }
    }
}
