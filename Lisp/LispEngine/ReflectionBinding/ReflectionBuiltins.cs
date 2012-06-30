using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;
using LispEngine.Util;
using Environment = LispEngine.Evaluation.Environment;

namespace LispEngine.ReflectionBinding
{
    class ReflectionBuiltins
    {
        class InvokeInstanceMethod : Function
        {
            public Datum Evaluate(Datum args)
            {
                var argArray = args.ToArray();
                var method = argArray[0].CastString();
                var target = argArray[1].CastObject();
                var methodArgs = args.Enumerate().Skip(2).Select(DatumHelpers.castObject).ToArray();
                var result = target.GetType().InvokeMember(method, BindingFlags.Default | BindingFlags.InvokeMethod, null, target, methodArgs);
                return result.ToAtom();
            }

            public override string  ToString()
            {
                return ",invoke-instance";
            }
        }

        class StaticMethod : Function
        {
            private readonly Type type;
            private readonly string methodName;
            public StaticMethod(Type type, string methodName)
            {
                this.type = type;
                this.methodName = methodName;
            }

            public Datum Evaluate(Datum args)
            {
                var methodArgs = args.Enumerate().Select(DatumHelpers.castObject).ToArray();
                var result = type.InvokeMember(methodName,
                                               BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Static,
                                               null, null, methodArgs);
                return result == null ? Null.Instance : DatumHelpers.atom(result);
            }

            public override string ToString()
            {
                return string.Format("{0}.{1}", type.FullName, methodName);
            }
        }

        class Ref : AbstractFExpression
        {
            public override Continuation Evaluate(Continuation c, Environment env, Datum args)
            {
                var datumArgs = args.ToArray();
                if (datumArgs.Length != 1)
                    throw c.error("Ref expect 1s arguments. {0} passed", datumArgs.Length);

                var assemblyName = DatumHelpers.getIdentifier(datumArgs[0]);
                var assembly = Assembly.Load(assemblyName);

                // Keep a hash set to avoid overloads
                var symbols = new HashSet<object>();
                // Add all static methods
                foreach (var t in assembly.GetTypes())
                {
                    foreach (var mi in t.GetMethods())
                    {
                        var symbol = String.Format("{0}.{1}", t.FullName, mi.Name);
                        if (mi.IsStatic && symbols.Add(symbol))
                            env.Define(symbol, new StaticMethod(t, mi.Name).ToStack());
                    }
                    // TODO Maybe add the types themselves as symbols too?
                    // Then we could implement "new"
                }
                // Return the list of symbols that we've added to the environment.
                return c.PushResult(DatumHelpers.atomList(symbols.ToArray()));
            }

            public override string ToString()
            {
                return ",ref";
            }
        }

        public static Environment AddTo(Environment env)
        {
            // Invoke a given instance method on an object
            env = env.Extend("invoke-instance", new InvokeInstanceMethod().ToStack());
            // Bring all static symbols from a particular assembly into the current environment.
            env = env.Extend("ref", new Ref());
            // Define "." as a macro to invoke a given method
            ResourceLoader.ExecuteResource(env, "LispEngine.ReflectionBinding.ReflectionBuiltins.lisp");
            return env;
        }
    }
}
