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
        private class MockEnvironment : Environment
        {
            private readonly IDictionary<string, object> items = new Dictionary<string, object>();
            public object lookup(string name)
            {
                if(items.ContainsKey(name))
                    return items[name];
                return EmptyEnvironment.Instance.lookup(name);
            }

            public void Put(String name, object value)
            {
                items[name] = value;
            }
        }

        private readonly MockEnvironment env = new MockEnvironment();

        public EvaluatorTest()
        {
            env.Put("life", 42);
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
    }
}
