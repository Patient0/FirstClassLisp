﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LispEngine.Core;
using LispEngine.Datums;
using LispEngine.Evaluation;
using LispEngine.Util;
using Environment = LispEngine.Evaluation.Environment;

namespace LispEngine.ReflectionBinding
{
    class ReflectionBuiltins
    {
        private static object unwrapDatum(Datum d)
        {
            var atom = d as Atom;
            if (atom != null)
                return atom.Value;
            // For now, assume that if anything other than atom is
            // passed into the .Net layer then the target function
            // actually expects a Datum.
            return d;
        }

        private static object[] unwrap(IEnumerable<Datum> datums)
        {
            return datums.Select(unwrapDatum).ToArray();
        }

        class InstanceMethod : Function
        {
            private readonly string name;
            public InstanceMethod(string name)
            {
                this.name = name;
            }

            public Datum Evaluate(Datum args)
            {
                var argArray = args.ToArray();

                var target = unwrapDatum(argArray[0]);
                var methodArgs = unwrap(args.Enumerate().Skip(1));
                var result = target.GetType().InvokeMember(name, BindingFlags.Default | BindingFlags.InvokeMethod, null, target, methodArgs);
                return result.ToAtom();
            }

            public override string ToString()
            {
                return string.Format(".{0}", name);
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
                var methodArgs = unwrap(args.Enumerate());
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

        class New : Function
        {
            public Datum Evaluate(Datum args)
            {
                var argsArray = args.ToArray();
                if(argsArray.Length < 1)
                    throw DatumHelpers.error("No type specified for 'new'");

                var type = (Type)argsArray[0].CastObject();
                var constructorArgs = argsArray.Skip(1).Select(DatumHelpers.castObject).ToArray();
                var instance = Activator.CreateInstance(type, constructorArgs);
                return instance.ToAtom();
            }

            public override string ToString()
            {
                return ",new";
            }
        }

        class WrapAtom : UnaryFunction
        {
            protected override Datum eval(Datum arg)
            {
                return arg.ToAtom();
            }
        }

        class Ref : AbstractFExpression
        {
            public override Continuation Evaluate(Continuation c, Environment env, Datum args)
            {
                var datumArgs = args.ToArray();
                if (datumArgs.Length != 1)
                    throw c.error("Ref expect 1s arguments. {0} passed", datumArgs.Length);

                var assemblyName = datumArgs[0].CastIdentifier();
                var assembly = Assembly.Load(assemblyName);

                // Keep a hash set to avoid adding things twice.
                // Overload resolution is (hopefully) handled by
                // InvokeMethod.
                var symbols = new HashSet<object>();
                // Add all static methods
                foreach (var t in assembly.GetTypes())
                {
                    // Make each type have a symbol in the environment also.
                    env.Define(t.FullName, t.ToAtom());
                    foreach (var mi in t.GetMethods())
                    {
                        var symbol = string.Format("{0}.{1}", t.FullName, mi.Name);
                        if (mi.IsStatic && symbols.Add(symbol))
                            env.Define(symbol, new StaticMethod(t, mi.Name).ToStack());
                    }
                }
                return c.PushResult(DatumHelpers.nil);
            }

            public override string ToString()
            {
                return ",ref";
            }
        }

        private static Datum makeInstanceMethod(Datum arg)
        {
            return new InstanceMethod(arg.CastString()).ToStack();
        }

        private static Datum getType(Datum arg)
        {
            return Type.GetType(arg.CastString()).ToAtom();
        }

        public static Environment AddTo(Environment env)
        {
            // Invoke a given instance method on an object
            env = env.Extend("make-instance-method", DelegateFunctions.MakeDatumFunction(makeInstanceMethod, ",make-instance-method"));
            env = env.Extend("get-type", DelegateFunctions.MakeDatumFunction(getType, ",get-type"));
            env = env.Extend("new", new New().ToStack());
            // Bring all static symbols from a particular assembly into the current environment.
            env = env.Extend("ref", new Ref());
            env = env.Extend("atom", new WrapAtom().ToStack());
            // Define "." as a macro to invoke a given method
            ResourceLoader.ExecuteResource(env, "LispEngine.ReflectionBinding.ReflectionBuiltins.lisp");
            return env;
        }
    }
}
