using System;
using System.Collections.Generic;
using System.Linq;
using LispEngine.Bootstrap;
using LispEngine.Datums;
using LispEngine.Evaluation;
using LispEngine.Lexing;
using LispEngine.Parsing;
using LispEngine.Stack;
using NUnit.Framework;
using Environment = LispEngine.Evaluation.Environment;

namespace LispTests.Evaluation
{
    [TestFixture]
    class EvaluatorTest : DatumHelpers
    {
        private StackEvaluator e;
        private Environment env;

        private void test(string sexp, params Datum[] expected)
        {
            var parser = new Parser(Scanner.Create(sexp));
            Datum datum = null;
            var actual = new List<Datum>();
            while ((datum = parser.parse()) != null)
            {
                var result = e.Evaluate(env, datum);
                actual.Add(result);
            }
            Assert.AreEqual(expected, actual);
        }

        [SetUp]
        public void setup()
        {
            e = new StackEvaluator();
            env = StandardEnvironment.Create().Extend("life", atom(42));            
        }

        [Test]
        public void testAtom()
        {
            test("23", atom(23));
        }

        [Test, TestCaseSource(typeof(EvaluatorTestCases), "TestCases")]
        public Datum evaluate(Datum expression)
        {
            var result = e.Evaluate(env, expression);
            Console.WriteLine("Expression: {0}", expression);
            Console.WriteLine("Result: {0}", result);
            return result;
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
        public void testDefine()
        {
            test("(define x 5) x", atom(5), atom(5));
        }
    }
}
