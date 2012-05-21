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
        [Test]
        public void testPairToString()
        {
            var p = cons(atom(5), atom(6));
            Assert.AreEqual("(5 . 6)", p.ToString());
        }

        [Test]
        public void testListToString()
        {
            var l = atomList(5, 6);
            Assert.AreEqual("(5 6)", l.ToString());
        }

        [Test]
        public void testLongerListToString()
        {
            var l = atomList(5, 6, 7);
            Assert.AreEqual("(5 6 7)", l.ToString());
        }


        [Test]
        public void testEmptyListToString()
        {
            Assert.AreEqual("()", nil.ToString());
        }

        [Test]
        public void testImproperListToString()
        {
            Assert.AreEqual("(5 6 . 7)", cons(atom(5), cons(atom(6), atom(7))).ToString());
        }
    }
}
