using System;
using System.IO;
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

        private static Token integer(string s)
        {
            return token(TokenType.Integer, s);
        }

        private void test(string text, params Token[] expected)
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
        public void TestSpace()
        {
            test(" ", space(" "));
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
            test("hello world", symbol("hello"), space(" "), symbol("world"));
        }

        [Test]
        public void testInteger()
        {
            test("55", token(TokenType.Integer, "55"));
        }

        [Test]
        public void testMixedSymbolInteger()
        {
            test("Hello 22 World", symbol("Hello"), space(" "), integer("22"), space(" "), symbol("World"));
        }

        [Test]
        public void testOpenClose()
        {
            test("(printf)", open, symbol("printf"), close);
        }
    }
}
