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
        static void Main(string[] args)
        {
            // Really simple REPL
            var env = StandardEnvironment.Create();
            string line;
            var evaluator = new Evaluator();
            Console.WriteLine("First Class Lisp. Ctrl-Z + Enter to exit.");
            Console.Write("FCLisp> ");
            while((line = Console.ReadLine()) != null)
            {
                var parsed = new Parser(Scanner.Create(line)).parse();
                var result = evaluator.Evaluate(env, parsed);
                Console.WriteLine("{0}", result);
                Console.Write("FCLisp> ");
            }
        }
    }
}
