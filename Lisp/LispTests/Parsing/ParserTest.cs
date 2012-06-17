using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using LispEngine.Datums;
using LispEngine.Lexing;
using LispEngine.Parsing;
using LispEngine.Util;
using NUnit.Framework;

namespace LispTests.Parsing
{
    [TestFixture]
    class ParserTest : DatumHelpers
    {
        private static void test(string sexp, params Datum[] expected)
        {
            Console.WriteLine("sexp: {0}", sexp);
            var s = Scanner.Create(sexp);
            var p = new Parser(s);
            var actual = new List<Datum>();
            Datum parsed;
            while( (parsed = p.parse()) != null)
                actual.Add(parsed);

            Assert.AreEqual(expected, actual.ToArray());
        }

        private static readonly Symbol house = symbol("house");
        private static readonly Symbol wheel = symbol("wheel");
        private static readonly Symbol hello = symbol("hello");
        private static readonly Symbol world = symbol("world");

        [Test]
        public void testSymbol()
        {
            test("house", house);
        }

        [Test]
        public void testTwoSymbols()
        {
            test("house wheel", house, wheel);
        }

        [Test]
        public void testCompoundUtilityFunction()
        {
            Assert.AreEqual(nil, compound());
            Assert.AreEqual(cons(house, nil), compound(house));
            Assert.AreEqual(cons(hello, cons(world, nil)), compound(hello, world));
        }

        [Test]
        public void testNull()
        {
            test("()", nil);
        }

        [Test]
        public void testCompound()
        {
            test("(house)", compound(house));
        }

        [Test]
        public void testTwoCompound()
        {
            test("(hello world)", compound(hello, world));
        }

        [Test]
        public void testTwoCompoundMultipleSpaces()
        {
            var expected = compound(hello, world);
            test("(hello    world)", expected);
            test("( hello    world)", expected);
            test("(   hello world  )", expected);
            test("  (hello world)", expected);
        }

        [Test]
        public void testIntegerAtom()
        {
            test("55", atom(55));
        }

        [Test]
        public void testNestedCompound()
        {
            var worldlist = compound(world);
            var expected = compound(hello, worldlist);
            test("(hello (world))", expected);
        }

        private static void failtest(string sexp, string errorMsg)
        {
            try
            {
                test(sexp);
            }
            catch (ParseException e)
            {
                Console.WriteLine("Got expected Parse exception: {0}", e);
                Assert.IsTrue(e.Message.Contains(errorMsg));
            }
            
        }

        [Test]
        public void testParseException()
        {
            failtest("(hello .", "(hello ");
        }

        [Test]
        public void testPair()
        {
            test("(hello . world)", cons(hello, world));
        }

        [Test]
        public void test3Tuple()
        {
            test("(hello world . house)", cons(hello, cons(world, house)));
        }

        [Test]
        public void testInvalidPair()
        {
            failtest("(5 . 6 7)", "(5 . 6");
        }

        [Test]
        public void testInvalidPairMsg()
        {
            failtest("(5 . 6 7 8)", "(5 . 6 7");
        }

        [Test]
        public void testBooleanAtom()
        {
            test("#t", atom(true));
            test("#f", atom(false));
        }

        // '3 is not equivalent to (quote . 3). It is
        // equivalent to (quote 3).
        [Test]
        public void testQuoteConsAsList()
        {
            test("'3", compound(quote, atom(3)));
        }

        // '(3 4) is not equivalent to (quote 3 4) - 
        // it's instead equivalent to (quote (3 4))
        [Test]
        public void testListQuote()
        {
            test("'(3 4)", compound(quote, atomList(3, 4)));
        }

        [Test]
        public void testQuasiQuote()
        {
            test("`(3 4)", compound(quasiquote, atomList(3, 4)));
        }

        [Test]
        public void testQuasiQuoteCombination()
        {
            var unquote3 = compound(unquote, atom(3));
            var unquote4 = compound(unquoteSplicing, atomList(4));
            test("`(,3 ,@(4))", compound(quasiquote, compound(unquote3, unquote4)));
        }

        [Test]
        public void testListDotSymbol()
        {
            test("(.house)", compound(symbol("."), symbol("house")));
        }

        [Test]
        public void testLoadLispFile()
        {
            foreach(var d in ResourceLoader.ReadDatums("LispTests.Parsing.MultilineFile.lisp"))
                Console.WriteLine("Parsed:\n{0}\n", d);
        }
    }
}
