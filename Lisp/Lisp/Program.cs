using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LispEngine.Bootstrap;
using LispEngine.Datums;
using LispEngine.Evaluation;
using LispEngine.Lexing;
using LispEngine.Parsing;
using Environment = LispEngine.Evaluation.Environment;

namespace Lisp
{
    class Program
    {
        private static void prompt()
        {
            Console.Write("FCLisp> ");            
        }

        private readonly Environment env = StandardEnvironment.Create();
        private readonly Evaluator evaluator = new Evaluator();

        // TODO: Also, we ought to expose load as a function in the environment also
        private void load(string path)
        {
            Console.WriteLine("Loading {0}...", path);
            using(var file = new FileStream(path, FileMode.Open))
            {
                var parser = new Parser(new Scanner(new StreamReader(file)));
                Datum parsed;
                while( (parsed = parser.parse()) != null)
                {
                    evaluator.Evaluate(env, parsed);
                }
            }
        }

        public void execute(string[] args)
        {
            try
            {
                foreach (var filename in args)
                    load(filename);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Error: {0}", e);
            }
        }

        public void repl()
        {
            // TODO: We ought to implement the repl in Lisp also
            // i.e. expose (read), (eval), (print) as builtin functions.
            Console.WriteLine("First Class Lisp REPL. Ctrl-Z + Enter to exit.");
            prompt();
            string line;
            while ((line = Console.ReadLine()) != null)
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

        static void Main(string[] args)
        {
            var program = new Program();
            program.execute(args);
            program.repl();
        }
    }
}
