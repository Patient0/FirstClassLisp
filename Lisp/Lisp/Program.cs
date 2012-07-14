using System;
using System.Reflection;
using LispEngine.Bootstrap;
using LispEngine.Datums;
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
                env = env.Extend("args", DatumHelpers.atomList(args));
                ResourceLoader.ExecuteResource(Assembly.GetCallingAssembly(), env, "Lisp.REPL.lisp");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("ERROR:\n{0}\n{1}\n", ex, ex.StackTrace);
            }

        }
    }
}
