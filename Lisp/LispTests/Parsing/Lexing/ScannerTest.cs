using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LispEngine.Datums;
using LispEngine.Lexing;
using NUnit.Framework;

namespace LispTests.Lexing
{
    [TestFixture]
    public class ScannerTest
    {
        private static Token token(TokenType type, string contents)
        {
            return new Token(type, contents);
        }

        private static Token symbol(string s)
        {
            return token(TokenType.Symbol, s);
        }

        private static Token space(string s)
        {
            return token(TokenType.Space, s);
        }

        private static readonly Token open = token(TokenType.Open, "(");
        private static readonly Token close = token(TokenType.Close, ")");
        private static readonly Token sp = space(" ");

        private static Token integer(string s)
        {
            return token(TokenType.Integer, s);
        }

        private static Token number(string s)
        {
            return token(TokenType.Double, s);
        }

        private static Token str(string s)
        {
            return token(TokenType.String, s);
        }

        private static void test(string text, params Token[] expected)
        {
            var c = 0;
            var s = Scanner.Create(text);
            foreach (var t in s.Scan())
            {
                Console.WriteLine("Token: {0}", t);
                Assert.AreEqual(expected[c], t);
                ++c;
            }
            Assert.AreEqual(expected.Length, c);
        }

        [Test]
        public void testRecoveryAfterUnrecognizedToken()
        {
            var s = Scanner.Create("@5");
            var tokens1 = s.Scan().GetEnumerator();
            Token t2 = null;
            try
            {
                tokens1.MoveNext();
            }
            catch(Exception)
            {
                var tokens2 = s.Recover().GetEnumerator();
                tokens2.MoveNext();
                t2 = tokens2.Current;
            }
            Assert.AreEqual(new Token(TokenType.Integer, "5"), t2);
        }

        [Test]
        public void TestSpace()
        {
            test(" ", sp);
        }

        [Test]
        public void Test2Spaces()
        {
            test("  ", space("  "));
        }

        [Test]
        public void TestSymbol()
        {
            test("one", symbol("one"));
        }

        [Test]
        public void TestHelloWorld()
        {
            test("hello world", symbol("hello"), sp, symbol("world"));
        }

        [Test]
        public void testInteger()
        {
            test("55", token(TokenType.Integer, "55"));
        }

        [Test]
        public void testNegativeInteger()
        {
            test("-55", integer("-55"));
        }

        [Test]
        public void testFloat()
        {
            numberTest("4.5");
        }

        [Test]
        public void testPositiveInteger()
        {
            test("+55", integer("+55"));
        }

        [Test]
        public void testImplicitZero()
        {
            numberTest(".5");
        }

        [Test]
        public void testImplicitPositiveZero()
        {
            numberTest("+.5");
        }

        [Test]
        public void testImplicitNegativeZero()
        {
            numberTest("-.5");
        }

        private static void numberTest(string s)
        {
            test(s, number(s));
        }

        [Test]
        public void testScientific()
        {
            numberTest("1.e+10");
            numberTest("1e10");
            numberTest("+1e10");
            numberTest("+15e10");
            numberTest("-1e10");
            numberTest("1.e10");
            numberTest("1.3e2");
            numberTest("1e-2");
            numberTest("1e+2");
        }

        [Test]
        public void testInvalidExponent()
        {
            test(".e ", symbol(".e"), sp);
        }


        [Test]
        public void testInvalidExponent2()
        {
            test(".every ", symbol(".every"), sp);
        }

        [Test]
        public void testMixedSymbolInteger()
        {
            test("Hello 22 World", symbol("Hello"), sp, integer("22"), sp, symbol("World"));
        }

        [Test]
        public void testOpenClose()
        {
            test("(printf)", open, symbol("printf"), close);
        }

        [Test]
        public void TestDot()
        {
            test("5 . 6", integer("5"), sp, token(TokenType.Dot, "."), sp, integer("6"));
        }

        [Test]
        public void TestSymbolQuestionMark()
        {
            test("eq?", symbol("eq?"));
        }

        [Test]
        public void TestSymbolSpecial()
        {
            test("e-+.@", symbol("e-+.@"));
        }

        private static Token boolean(string c)
        {
            return token(TokenType.Boolean, c);
        }

        [Test]
        public void testBoolean()
        {
            test("#t #T", boolean("#t"), sp, boolean("#T"));
            test("#f #F", boolean("#f"), sp, boolean("#F"));
        }

        private static readonly Token quote = token(TokenType.Quote, DatumHelpers.quoteAbbreviation);
        private static readonly Token quasiquote = token(TokenType.Quote, DatumHelpers.quasiquoteAbbreviation);
        private static readonly Token unquote = token(TokenType.Quote, DatumHelpers.unquoteAbbreviation);
        private static readonly Token splicing = token(TokenType.Quote, DatumHelpers.splicingAbbreviation);

        [Test]
        public void testQuote()
        {
            test("'(3 4)", quote, open, integer("3"), sp, integer("4"), close);
        }

        [Test]
        public void testQuasiQuote()
        {
            test("`(,3 ,@(4 5))", quasiquote, open, unquote, integer("3"), sp, splicing, open, integer("4"), sp, integer("5"), close, close);
        }

        [Test]
        public void testStringLiteral()
        {
            test("\"Hello world\"", str("\"Hello world\""));
        }

        private static readonly Token vectorOpen = token(TokenType.VectorOpen, "#(");
        [Test]
        public void testVectorLiteral()
        {
            test("#(1)", vectorOpen, integer("1"), close);
        }
    }
}
