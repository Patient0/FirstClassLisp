using LispEngine.Evaluation;
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
                .Extend("quote", Quote.Instance)
                .Extend("define", Define.Instance)
                .Extend("set!", Set.Instance)
                .Extend("begin", Begin.Instance)
                .Extend("call/cc", CallCC.Instance)
                .Extend("ref", Ref.Instance);
            return env;
        }
    }
}
