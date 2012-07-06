using System;
using System.Reflection;
using LispEngine.Bootstrap;
using LispEngine.Util;

namespace Lisp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ResourceLoader.ExecuteResource(Assembly.GetCallingAssembly(), StandardEnvironment.Create(), "Lisp.REPL.lisp");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("ERROR:\n{0}\n{1}\n", ex, ex.StackTrace);
            }

        }
    }
}
