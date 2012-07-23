using LispEngine.Datums;
using LispEngine.Evaluation;
using LispEngine.ReflectionBinding;
using Environment = LispEngine.Evaluation.Environment;

namespace LispEngine.Core
{
    // Because we have implemented macros as first class objects,
    // *all* core forms can be simply be defined in the environment!
    public class CoreForms
    {
        public static Environment AddTo(Environment env)
        {
            env = env
                .Extend("log", Log.Instance)
                .Extend("lambda", Lambda.Instance)
                .Extend("cons", DelegateFunctions.MakeDatumFunction(DatumHelpers.cons, ",cons"))
                .Extend("set-car!", DelegateFunctions.MakeDatumFunction(DatumHelpers.setCar, ",set-car!"))
                .Extend("set-cdr!", DelegateFunctions.MakeDatumFunction(DatumHelpers.setCdr, ",set-cdr!"))
                .Extend("apply", Apply.Instance)
                .Extend("eq?", EqualFunctions.Eq)
                .Extend("equal?", EqualFunctions.Equal)
                .Extend("quote", Quote.Instance)
                .Extend("define", Define.Instance)
                .Extend("set!", Set.Instance)
                .Extend("begin", Begin.Instance)
                .Extend("call-with-current-continuation", CallCC.Instance)
                .Extend("eval", Eval.Instance)
                .Extend("env", Env.Instance);
            env = DebugFunctions.AddTo(env);
            env = Macro.AddTo(env);
            env = VectorFunctions.AddTo(env);
            return env;
        }
    }
}