using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using NUnit.Framework;

namespace LispTests.Datums
{
    [TestFixture]
    class DatumTest : DatumHelpers
    {
        private static void check(string s, Datum d)
        {
            Assert.AreEqual(s, d.ToString());
        }
        [Test]
        public void testPairToString()
        {
            var p = cons(atom(5), atom(6));
            check("(5 . 6)", p);
        }

        [Test]
        public void testListToString()
        {
            var l = atomList(5, 6);
            check("(5 6)", l);
        }

        [Test]
        public void testLongerListToString()
        {
            var l = atomList(5, 6, 7);
            check("(5 6 7)", l);
        }


        [Test]
        public void testEmptyListToString()
        {
            check("()", nil);
        }

        [Test]
        public void testImproperListToString()
        {
            check("(5 6 . 7)", cons(atom(5), cons(atom(6), atom(7))));
        }

        [Test]
        public void testBooleanToString()
        {
            check("#t", atom(true));
            check("#f", atom(false));
        }

        [Test]
        public void testAtomToString()
        {
            check("\"hello\"", atom("hello"));
        }

        [Test]
        public void testEscapedStringAtomToString()
        {
            check("\"hello\\t\\nworld\"", atom("hello\t\nworld"));
        }

        [Test]
        public void testQuoteToString()
        {
            check("'5", compound(quote, atom(5)));
        }

        [Test]
        public void testQuotedListToString()
        {
            check("'(1 2 3)", compound(quote, atomList(1, 2, 3)));
        }

        [Test]
        public void testUnquoteToString()
        {
            check(",3", compound(unquote, atom(3)));
        }

        [Test]
        public void testQuasiQuoteToString()
        {
            check("`3", compound(quasiquote, atom(3)));
        }

        [Test]
        public void testSplicingToString()
        {
            check(",@3", compound(unquoteSplicing, atom(3)));
        }


        [Test]
        public void testDatumEnumerate()
        {
            var five = atom("5");
            var listfive = compound(five);
            var l = enumerate(listfive);
            Assert.AreEqual(1, l.Count());

            var listfivefive = compound(five, five);
            l = enumerate(listfivefive);
            Assert.AreEqual(2, l.Count());
        }

    }
}
