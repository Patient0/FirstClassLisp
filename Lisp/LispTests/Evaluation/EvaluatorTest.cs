using System;
using System.Collections.Generic;
using System.Linq;
using LispEngine.Bootstrap;
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
        private readonly Environment env;

        public EvaluatorTest()
        {
            this.env = StandardEnvironment.Create().Extend("life", atom(42));
        }

        private void test(string sexp, Datum expected)
        {
            var e = new Evaluator();

            var datum = new Parser(Scanner.Create(sexp)).parse();
            var result = e.evaluate(env, datum);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void testAtom()
        {
            test("23", atom(23));
        }

        [Test]
        public void testSymbol()
        {
            test("life", atom(42));
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
            test("((lambda (x) x) 5)", atom(5));
        }

        [Test]
        public void testConstantFunction()
        {
            test("((lambda () 6))", atom(6));
        }

        [Test]
        public void testLambdaList()
        {
            test("((lambda x x) 1 2 3)", atomList(1,2,3));
        }

        [Test]
        public void testList()
        {
            test("(list 1 2 3)", atomList(1, 2, 3));
        }

        [Test]
        public void testRecursiveFunctions()
        {
            test("(cons 3 (cons 4 5))", cons(atom(3), cons(atom(4), atom(5))));
        }

        [Test]
        public void testCons()
        {
            test("(cons 3 4)", cons(atom(3), atom(4)));
        }

        [Test]
        public void testApply()
        {
            test("(apply cons (list 3 4))", cons(atom(3), atom(4)));
        }

        [Test]
        public void testDotArgList()
        {
            test("((lambda (x . y) x) 4)", atom(4));
        }

        [Test]
        public void testCarList()
        {
            test("(car (list 4 5))", atom(4));
        }

        [Test]
        public void testCdrList()
        {
            test("(cdr (list 4 5))", atomList(5));
        }

        // Our lambda macro actually does full "structure" matching
        // on all of its arguments - this was actually as simple to
        // implement as the standard, which is a subset.
        // If then allowed multiple alternative argument lists
        // we could then support pattern matching.
        [Test]
        public void testStructuredMatching()
        {
            test("((lambda (a (b c)) c) 4 (list 5 6)))", atom(6));
            test("((lambda ((a b) c) b) (list 5 6) 4))", atom(6));
            test("((lambda (((a))) a) (list (list 5)))", atom(5));
        }
    }
}
