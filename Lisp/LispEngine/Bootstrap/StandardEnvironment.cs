using LispEngine.Core;
using LispEngine.Evaluation;
using LispEngine.ReflectionBinding;
using Environment = LispEngine.Evaluation.Environment;

namespace LispEngine.Bootstrap
{
    public class StandardEnvironment
    {
        public static Environment Create()
        {
            var env = CreateSandbox();
            // Adding reflection builtins enables a lisp engine
            // to execute any code.
            env = ReflectionBuiltins.AddTo(env);
            return env;
        }

        /**
         * Create a "sandbox" environment: the symbols defined in the
         * environment don't provide any way for lisp programs to execute
         * arbitrary code.
         */
        public static Environment CreateSandbox()
        {
            var env = new Environment(EmptyEnvironment.Instance);
            env = new Statistics().AddTo(env);
            env = CoreForms.AddTo(env);
            env = Builtins.AddTo(env);
            return env;
        }
    }
}
