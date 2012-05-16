using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;
using LispEngine.Lexing;
using LispEngine.Parsing;
using NUnit.Framework;
using Environment = LispEngine.Evaluation.Environment;

namespace LispTests.Evaluation
{
    [TestFixture]
    class EvaluatorTest : DatumHelpers
    {
        private Environment env;

        public EvaluatorTest()
        {
            var e = EmptyEnvironment.Instance;
            e = CoreForms.AddTo(e);
            e = e.Extend("life", 42);
            this.env = e;
        }

        private void test(string sexp, object expected)
        {
            var e = new Evaluator();

            var datum = new Parser(Scanner.Create(sexp).Scan()).parse();
            var result = e.evaluate(env, datum);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void testAtom()
        {
            test("23", 23);
        }

        [Test]
        public void testSymbol()
        {
            test("life", 42);
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

        [Test]
        public void testFunction()
        {
            test("((lambda (x) x) 5)", 5);
        }
    }
}
