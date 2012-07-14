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
                .Extend("cons", Cons.Instance)
                .Extend("apply", Apply.Instance)
                .Extend("eq?", Eq.Instance)
                .Extend("macro", Macro.Instance)
                .Extend("unmacro", Macro.Unmacro)
                .Extend("quote", Quote.Instance)
                .Extend("define", Define.Instance)
                .Extend("set!", Set.Instance)
                .Extend("begin", Begin.Instance)
                .Extend("call/cc", CallCC.Instance)
                .Extend("eval", Eval.Instance)
                .Extend("env", Env.Instance);
            env = DebugFunctions.AddTo(env);
            return env;
        }
    }
}
