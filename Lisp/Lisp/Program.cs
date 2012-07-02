using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using LispEngine.Bootstrap;
using LispEngine.Datums;
using LispEngine.Evaluation;
using LispEngine.Lexing;
using LispEngine.Parsing;
using LispEngine.Util;
using Environment = LispEngine.Evaluation.Environment;

namespace Lisp
{
    class Program
    {

        private readonly Environment env = StandardEnvironment.Create();

        public void repl()
        {
            // Can't seem to get ResourceLoader to figure out to use *our* assembly
            // by default.
            var assembly = Assembly.GetCallingAssembly();
            ResourceLoader.ExecuteResource(assembly, env, "Lisp.REPL.lisp");
        }

        static void Main(string[] args)
        {
            try
            {
                var program = new Program();
                program.repl();                
            }
            catch (EvaluationException ex)
            {
                Console.Error.WriteLine("ERROR:\n{0}\n{1}\n", ex, ex.LispTrace);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("ERROR:\n{0}\n{1}\n", ex, ex.StackTrace);
            }

        }
    }
}
