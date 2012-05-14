using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Lexing;
using LispEngine.Parsing;
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
            var p = new Parser(s.Scan());
            var actual = new List<Datum>();
            Datum parsed;
            while( (parsed = p.parse()) != null)
                actual.Add(parsed);

            Assert.AreEqual(expected, actual.ToArray());
        }

        private static readonly Symbol car = symbol("car");
        private static readonly Symbol wheel = symbol("wheel");
        private static readonly Symbol hello = symbol("hello");
        private static readonly Symbol world = symbol("world");

        [Test]
        public void testSymbol()
        {
            test("car", car);
        }

        [Test]
        public void testTwoSymbols()
        {
            test("car wheel", car, wheel);
        }

        [Test]
        public void testCompoundUtilityFunction()
        {
            Assert.AreEqual(nil, compound());
            Assert.AreEqual(cons(car, nil), compound(car));
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
            test("(car)", compound(car));
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
    }
}
