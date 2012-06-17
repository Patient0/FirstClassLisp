using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;
using Environment = LispEngine.Evaluation.Environment;

namespace LispEngine.Core
{
    class ApplyMethod : AbstractFExpression
    {
        public static readonly FExpression Instance = new ApplyMethod();

        private ApplyMethod()
        {
        }

        private static Func<Datum, object> CreateEvaluator(Continuation c, Environment env)
        {
            Evaluator e = new Evaluator();
            return datum =>
                {
                    var c1 = c.Evaluate(env, datum);
                    var d1 = e.Evaluate(c1);

                    // TEMP: treat quoted symbols as string literals, until we have proper string expressions
                    var symbol = d1 as Symbol;
                    if (symbol == null)
                        return DatumHelpers.castAtom(d1);
                    else
                        return symbol.Identifier;
                };
        }

        private static bool MethodMatches(bool isStatic, string name, Type[] argTypes, MethodInfo mi)
        {
            if (mi.IsStatic != isStatic)
                return false;

            if (mi.Name != name)
                return false;

            // TODO: implement variable argument lists
            var pis = mi.GetParameters();
            if (pis.Length != argTypes.Length)
                return false;

            // TODO: give preference to e.g. Int32 arg passed to Int32 param (instead of Int32 arg passed to Object param)
            // TODO: implement all of the rest of the C# method binding rules
            for (int i = 0; i < pis.Length; i++)
            {
                if (!pis[i].ParameterType.IsAssignableFrom(argTypes[i]))
                    return false;
            }

            return true;
        }

        public override Continuation Evaluate(Continuation c, Environment env, Datum args)
        {
            var datumArgs = args.ToArray();
            if (datumArgs.Length < 2)
                throw c.error("ApplyMethod expects at least 2 arguments. {0} passed", datumArgs.Length);

            var symbol = datumArgs[0] as Symbol;
            if (symbol == null)
                throw c.error("'{0}' is not a symbol", datumArgs[0]);

            var evaluator = CreateEvaluator(c, env);
            var instance = evaluator(datumArgs[1]);
            var argValues = datumArgs.Skip(2).Select(evaluator).ToArray();
            var argTypes = Array.ConvertAll(argValues, obj => obj == null ? typeof(object) : obj.GetType());
            var instanceType = instance.GetType();
            var mi = instanceType.GetMethods().Where(m => MethodMatches(false, symbol.Identifier, argTypes, m)).FirstOrDefault();
            if (mi == null)
                throw c.error("No method for {0}.{1}({2})", instanceType.Name, symbol.Identifier, string.Join(", ", argTypes.Select(t => t.Name)));

            var result = mi.Invoke(instance, argValues);
            return c.PushResult(new Atom(result));
        }
    }
}
