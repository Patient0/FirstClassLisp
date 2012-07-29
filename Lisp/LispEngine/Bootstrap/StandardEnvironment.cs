using LispEngine.Core;
using LispEngine.Evaluation;
using LispEngine.ReflectionBinding;
using LispEngine.Util;

namespace LispEngine.Bootstrap
{
    public class StandardEnvironment
    {
        public static LexicalEnvironment Create()
        {
            var env = CreateSandbox();
            // Adding reflection builtins enables a lisp engine
            // to execute any code.
            env = ReflectionBuiltins.AddTo(env);
            // Add functions for reading files, executing lisp programs
            // defined in files.
            ResourceLoader.ExecuteResource(env, "LispEngine.Bootstrap.IO.lisp");
            return env;
        }

        /**
         * Create a "sandbox" environment: the symbols defined in the
         * environment don't provide any way for lisp programs to execute
         * arbitrary code.
         */
        public static LexicalEnvironment CreateSandbox()
        {
            var env = LexicalEnvironment.Create();
            env = CoreForms.AddTo(env);
            env = Builtins.AddTo(env);
            return env;
        }
    }
}
