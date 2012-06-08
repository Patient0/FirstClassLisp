using System;
using System.Collections.Generic;
using System.Linq;
using LispEngine.Bootstrap;
using LispEngine.Datums;
using LispEngine.Evaluation;
using LispEngine.Lexing;
using LispEngine.Parsing;
using LispEngine.Util;
using NUnit.Framework;
using Environment = LispEngine.Evaluation.Environment;

namespace LispTests.Evaluation
{
    [TestFixture("EvaluatorTests.lisp")]
    [TestFixture("PatternMatchingTests.lisp")]
    [TestFixture("QuasiquoteTests.lisp")]
    [TestFixture("ArithmeticTests.lisp")]
    [TestFixture("CallCCTests.lisp")]
    class EvaluatorTests : DatumHelpers
    {
        private readonly string lispResourceFile;
        private Evaluator e;
        private Environment env;

        public EvaluatorTests(string lispResourceFile)
        {
            this.lispResourceFile = lispResourceFile;
        }

        [SetUp]
        public void setup()
        {
            e = new Evaluator();
            env = StandardEnvironment.Create().Extend("life", atom(42));            
        }

        [Test, TestCaseSource("TestCases")]
        public Datum evaluate(Datum expression)
        {
            var result = e.Evaluate(env, expression);
            Console.WriteLine("Expression: {0}", expression);
            Console.WriteLine("Result: {0}", result);
            return result;
        }

        private static IEnumerable<TestCaseData> loadTestCases(string resourceFile)
        {
            foreach (var d in ResourceLoader.ReadDatums(string.Format("LispTests.Evaluation.{0}", resourceFile)))
            {
                var combo = enumerate(d).ToArray();
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
                yield return testCase;
            }
        }

        public IEnumerable<TestCaseData> TestCases
        {
            get
            {
                return loadTestCases(lispResourceFile);
            }
        }

    }
}
