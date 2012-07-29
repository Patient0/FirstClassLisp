using System;
using System.Reflection;
using LispEngine.Bootstrap;
using LispEngine.Datums;
using LispEngine.Evaluation;
using LispEngine.Util;

namespace Lisp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var env = StandardEnvironment.Create();
                env.Define("args", DatumHelpers.atomList(args));
                var statistics = new Statistics();
                env = statistics.AddTo(env);
                ResourceLoader.ExecuteResource(statistics, Assembly.GetExecutingAssembly(), env, "Lisp.REPL.lisp");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("ERROR:\n{0}\n{1}\n", ex, ex.StackTrace);
            }

        }
    }
}
