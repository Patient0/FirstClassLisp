using LispEngine.Datums;
using LispEngine.Evaluation;
using LispEngine.ReflectionBinding;

namespace LispEngine.Core
{
    // Because we have implemented macros as first class objects,
    // *all* core forms can be simply be defined in the environment!
    public class CoreForms
    {
        public static LexicalEnvironment AddTo(LexicalEnvironment env)
        {
            env = env
                .Define("log", Log.Instance)
                .Define("lambda", Lambda.Instance)
                .Define("cons", DelegateFunctions.MakeDatumFunction(DatumHelpers.cons, ",cons"))
                .Define("set-car!", DelegateFunctions.MakeDatumFunction(DatumHelpers.setCar, ",set-car!"))
                .Define("set-cdr!", DelegateFunctions.MakeDatumFunction(DatumHelpers.setCdr, ",set-cdr!"))
                .Define("apply", Apply.Instance)
                .Define("eq?", EqualFunctions.Eq)
                .Define("equal?", EqualFunctions.Equal)
                .Define("quote", Quote.Instance)
                .Define("define", Define.Instance)
                .Define("set!", Set.Instance)
                .Define("begin", Begin.Instance)
                .Define("call-with-current-continuation", CallCC.Instance)
                .Define("eval", Eval.Instance)
                .Define("env", Env.Instance);
            env = DebugFunctions.AddTo(env);
            env = Macro.AddTo(env);
            env = VectorFunctions.AddTo(env);
            return env;
        }
    }
}