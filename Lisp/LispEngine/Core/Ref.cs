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
    class Ref : AbstractFExpression
    {
        public static readonly FExpression Instance = new Ref();

        private Ref()
        {
        }

        static Converter<Datum, object> CreateEvaluator(Continuation c, Environment env)
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

        static bool MethodMatches(Type[] argTypes, MethodInfo mi)
        {
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

        abstract class ClrSymbol : AbstractFExpression
        {
            public abstract void Add(MethodInfo member);
            public abstract Tuple<HashSet<MethodInfo>, object, object[]> ParseArgs(Continuation c, Environment env, Datum args);

            public override Continuation Evaluate(Continuation c, Environment env, Datum args)
            {
                var t = ParseArgs(c, env, args);
                var argTypes = Array.ConvertAll(t.Item3, arg => arg == null ? typeof(object) : arg.GetType());
                var mi = t.Item1.Where(m => MethodMatches(argTypes, m)).First();
                var result = mi.Invoke(t.Item2, t.Item3);
                return c.PushResult(DatumHelpers.atom(result));
            }
        }

        class ClrInstanceSymbol : ClrSymbol
        {
            private readonly Dictionary<Tuple<Type, string>, HashSet<MethodInfo>> members = new Dictionary<Tuple<Type, string>, HashSet<MethodInfo>>();

            public override void Add(MethodInfo member)
            {
                var key = Tuple.Create(member.DeclaringType, member.Name);
                HashSet<MethodInfo> set;
                if (!members.TryGetValue(key, out set))
                {
                    set = new HashSet<MethodInfo>();
                    members.Add(key, set);
                }

                set.Add(member);
            }

            HashSet<MethodInfo> LookupMembers(Type declaringType, string name)
            {
                HashSet<MethodInfo> set;
                while (!members.TryGetValue(Tuple.Create(declaringType, name), out set))
                {
                    if (declaringType.BaseType == null)
                        throw DatumHelpers.error("No method {0}.{1}", declaringType.Name, name);
                }

                return set;
            }

            public override Tuple<HashSet<MethodInfo>, object, object[]> ParseArgs(Continuation c, Environment env, Datum args)
            {
                var argArray = args.ToArray();
                if (argArray.Length < 2)
                    throw c.error("At least 2 arguments expected");

                var symbol = argArray[0] as Symbol;
                if (symbol == null)
                    throw c.error("Expected Symbol, got '{0}'", argArray[0]);

                var evaluator = CreateEvaluator(c, env);
                var instance = evaluator(argArray[1]);
                var instanceMembers = LookupMembers(instance.GetType(), symbol.Identifier);
                var argValues = argArray.Skip(2).Select(arg => evaluator(arg)).ToArray();
                return Tuple.Create(instanceMembers, instance, argValues);
            }
        }

        class ClrStaticSymbol : ClrSymbol
        {
            private readonly HashSet<MethodInfo> members = new HashSet<MethodInfo>();

            public override void Add(MethodInfo member)
            {
                members.Add(member);
            }

            public override Tuple<HashSet<MethodInfo>, object, object[]> ParseArgs(Continuation c, Environment env, Datum args)
            {
                var evaluator = CreateEvaluator(c, env);
                var argValues = Array.ConvertAll(args.ToArray(), evaluator);
                return Tuple.Create(members, (object) null, argValues);
            }
        }

        class ReferenceAssembly : Task
        {
            private readonly Environment env;
            private readonly string assemblyName;
            public ReferenceAssembly(Environment env, string assemblyName)
            {
                this.env = env;
                this.assemblyName = assemblyName;
            }

            void EnsureClrSymbol(MethodInfo member, bool isStatic)
            {
                string name;
                if (isStatic)
                    name = member.DeclaringType.FullName + "." + member.Name;
                else
                    name = ".";

                ClrSymbol symbol;
                Datum datum;
                if (env.TryLookup(name, out datum))
                    symbol = datum as ClrSymbol;
                else
                    symbol = null;

                if (symbol == null)
                {
                    symbol = isStatic ? (ClrSymbol) new ClrStaticSymbol() : new ClrInstanceSymbol();
                    env.Define(name, symbol);
                }

                symbol.Add(member);
            }

            public Continuation Perform(Continuation c)
            {
                var assembly = Assembly.Load(assemblyName);
                foreach (var t in assembly.GetTypes())
                {
                    foreach (var mi in t.GetMethods())
                        EnsureClrSymbol(mi, mi.IsStatic);
                }

                return c.PushResult(DatumHelpers.atom(null));
            }

            public override string ToString()
            {
                return string.Format("Reference assembly '{0}'", assemblyName);
            }
        }

        public override Continuation Evaluate(Continuation c, Environment env, Datum args)
        {
            var datumArgs = args.ToArray();
            if (datumArgs.Length != 1)
                throw c.error("Ref expect 1 arguments. {0} passed", datumArgs.Length);

            var name = datumArgs[0] as Symbol;
            if (name == null)
                throw c.error("'{0}' is not a symbol", datumArgs[0]);

            return c.PushTask(new ReferenceAssembly(env, name.Identifier));
        }
    }
}
