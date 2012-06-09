using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Bootstrap;
using LispEngine.Evaluation;
using LispEngine.Lexing;
using LispEngine.Parsing;

namespace Lisp
{
    class Program
    {
        private static void prompt()
        {
            Console.Write("FCLisp> ");            
        }
        static void Main(string[] args)
        {
            // Really simple REPL
            var env = StandardEnvironment.Create();
            string line;
            var evaluator = new Evaluator();
            Console.WriteLine("First Class Lisp. Ctrl-Z + Enter to exit.");
            prompt();
            while((line = Console.ReadLine()) != null)
            {
                try
                {
                    var parsed = new Parser(Scanner.Create(line)).parse();
                    var result = evaluator.Evaluate(env, parsed);
                    Console.WriteLine("{0}", result);
                    prompt();
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Error: {0}", e);
                    prompt();
                }
            }
        }
    }
}
