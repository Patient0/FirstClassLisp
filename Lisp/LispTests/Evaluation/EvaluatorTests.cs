using System;
using System.Collections.Generic;
using System.Linq;
using LispEngine.Bootstrap;
using LispEngine.Datums;
using LispEngine.Evaluation;
using LispEngine.Util;
using NUnit.Framework;

namespace LispTests.Evaluation
{
    [TestFixture("PatternMatchingTests.lisp")]
    [TestFixture("CallCCTests.lisp")]
    [TestFixture("QuasiquoteTests.lisp")]
    [TestFixture("EvaluatorTests.lisp")]
    [TestFixture("ArithmeticTests.lisp")]
    [TestFixture("MacroBuiltinTests.lisp")]
    [TestFixture("DotNetTests.lisp")]
    [TestFixture("EvalTests.lisp")]
    [TestFixture("LibraryTests.lisp")]
    [TestFixture("AmbTests.lisp")]
    [TestFixture("VectorTests.lisp")]
    [TestFixture("SudokuTests.lisp")]
    class EvaluatorTests : DatumHelpers
    {
        private readonly string lispResourceFile;
        private Evaluator e;
        private LexicalEnvironment env;

        public EvaluatorTests(string lispResourceFile)
        {
            this.lispResourceFile = lispResourceFile;
        }

        [TestFixtureSetUp]
        public void setupFixture()
        {
            e = new Evaluator();
            env = StandardEnvironment.Create();
            var setupDatum = getLispFromResource("setup");
            if (setupDatum != nil)
                foreach(var f in setupDatum.Enumerate())
                    e.Evaluate(env, f);
        }

        [Test, TestCaseSource("TestCases")]
        public Datum evaluate(Datum expression)
        {
            try
            {
                var result = e.Evaluate(env, expression);
                Console.WriteLine("Expression: {0}", expression);
                Console.WriteLine("Result: {0}", result);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ex: {0}", ex);
                throw;
            }
        }

        private static Datum checkQuote(Datum d)
        {
            var p = d as Pair;
            if (p != null && p.First.Equals(quote))
                return p.Second.ToArray()[0];
            return null;
        }

        private static TestCaseData datumToTestCase(Datum d)
        {
            var ignore = false;
            var quoted = checkQuote(d);
            if(quoted != null)
            {
                d = quoted;
                ignore = true;
            }
            var combo = d.ToArray();
            
            if (combo.Length < 3)
                throw new Exception(string.Format("'{0}' is not a valid test case", d));
            var name = combo[0] as Symbol;
            if (name == null)
                throw new Exception(string.Format("'{0}' is not a valid test case", d));

            var expected = combo[1];
            var expression = combo[2];
            var testCase = new TestCaseData(expression);
            testCase.Returns(expected);
            testCase.SetName(name.Identifier);
            if (ignore)
                testCase.Ignore("quoted");
            return testCase;
        }

        // We have to do this "inefficiently" rather than picking out
        // tests and setup in one pass because otherwise the error reporting
        // in NUnit does not work properly. This is because NUnit *first*
        // does "new EvaluatorTests(file).TestCases
        // and only *then* does
        // "new EvaluatorTests(file).setupFixture()". So we have to read the resource file
        // twice. Main thing is that we can arrange for "setup" to only
        // be executed once, while still having it run inside setupFixture(), which
        // is required for better NUnit error reporting. Throwing exceptions from the
        // TestCases method doesn't give great results.
        private Datum getLispFromResource(string name)
        {
            foreach (var d in ResourceLoader.ReadDatums(string.Format("LispTests.Evaluation.{0}", lispResourceFile)))
            {
                var list = d as Pair;
                if (list == null)
                    throw error("Expected a list instead of '{0}'", d);
                if (list.First.Equals(symbol(name)))
                    return list.Second;
            }
            return nil;
        }

        public IEnumerable<TestCaseData> TestCases
        {
            get
            {
                var testsDatum = getLispFromResource("tests");
                return testsDatum.Enumerate().Select(datumToTestCase).ToArray();
            }
        }
    }
}
