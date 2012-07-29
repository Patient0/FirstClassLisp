using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LispEngine.Core;
using LispEngine.Datums;
using LispEngine.Evaluation;
using LispEngine.Util;

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

        private static Datum MakeInstanceMethod(Datum arg)
        {
            return new InstanceMethod(arg.CastString()).ToStack();
        }

        private static Datum GetStaticMethod(Datum type, Datum method)
        {
            return new StaticMethod((Type) type.CastObject(), method.CastString()).ToStack();
        }


        // Thanks to http://stackoverflow.com/questions/2367652/how-type-gettype-works-when-given-partially-qualified-type-name
        // for this:
        public static Type GetTypeEx(string fullTypeName)
        {
            var type = Type.GetType(fullTypeName);
            if (type != null)
                return type;
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetType(fullTypeName) != null);
            if(assembly != null)
                return assembly.GetType(fullTypeName);
            throw DatumHelpers.error("Could not locate type '{0}' in any of the {1} currently loaded assemblies",
                                     fullTypeName, AppDomain.CurrentDomain.GetAssemblies().Length);
        }

        class GetTypeFunction : Function
        {
            public Datum Evaluate(Datum args)
            {
                var argArray = args.ToArray();
                var names = argArray.Select(x => x.CastString()).ToArray();
                var fullname = string.Join(".", names);
                return GetTypeEx(fullname).ToAtom();
            }

            public override string ToString()
            {
                return ",get-type";
            }
        }

        public static LexicalEnvironment AddTo(LexicalEnvironment env)
        {
            // Invoke a given instance method on an object
            env.Define("make-instance-method", DelegateFunctions.MakeDatumFunction(MakeInstanceMethod, ",make-instance-method"));
            env.Define("get-static-method", DelegateFunctions.MakeDatumFunction(GetStaticMethod, ",get-static-method"));
            env.Define("get-type", new GetTypeFunction().ToStack());
            env.Define("new", new New().ToStack());
            env.Define("atom", new WrapAtom().ToStack());
            // Define "dot" and "slash" as a macros which allow us to use
            // Clojure-style syntax for invoking and referring to methods.
            ResourceLoader.ExecuteResource(env, "LispEngine.ReflectionBinding.ReflectionBuiltins.lisp");
            return env;
        }
    }
}
