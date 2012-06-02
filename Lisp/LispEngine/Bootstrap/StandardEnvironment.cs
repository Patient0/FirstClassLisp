using LispEngine.Core;
using LispEngine.Evaluation;
using Environment = LispEngine.Evaluation.Environment;

namespace LispEngine.Bootstrap
{
    public class StandardEnvironment
    {
        public static Environment Create()
        {
            var env = new Environment(EmptyEnvironment.Instance);
            env = CoreForms.AddTo(env);
            env = Builtins.AddTo(env);
            return env;
        }
    }
}
