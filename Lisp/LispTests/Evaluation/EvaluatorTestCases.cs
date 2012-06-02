using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Util;
using NUnit.Framework;

namespace LispTests.Evaluation
{
    public class EvaluatorTestCases : DatumHelpers
    {
        public static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                foreach (var d in ResourceLoader.ReadDatums("LispTests.Evaluation.EvaluatorTests.lisp"))
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
        }
    }
}
