using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;
using NUnit.Framework;

namespace LispTests.Evaluation
{
    [TestFixture]
    class LexicalEnvironmentTests : DatumHelpers
    {
        private static void check(Symbol s, LexicalEnvironment e)
        {
            var value = e.Lookup(s);
            var location = e.LookupLocation(s);
            var lValue = location.Find(e).Value;
            Assert.AreEqual(value, lValue);            
        }

        [Test]
        public void testLocation()
        {
            var e = LexicalEnvironment.Create();
            var x = "x".ToSymbol();
            var y = "y".ToSymbol();
            var z = "z".ToSymbol();

            e.Define(x, 5.ToAtom());
            e.Define(y, 6.ToAtom());

            check(x, e);
            check(y, e);

            var e2 = e.NewFrame();
            e2.Define(z, 7.ToAtom());
            e2.Define(y, 8.ToAtom());
            check(x, e2);
            check(y, e2);
            check(z, e2);
        }
    }
}
