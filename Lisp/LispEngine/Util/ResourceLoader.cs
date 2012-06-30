using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;
using LispEngine.Lexing;
using LispEngine.Parsing;
using Environment = LispEngine.Evaluation.Environment;

namespace LispEngine.Util
{
    public class ResourceLoader
    {
        public static IEnumerable<Datum> ReadDatums(string resourceFile)
        {
            var assembly = Assembly.GetCallingAssembly();
            var stream = assembly.GetManifestResourceStream(resourceFile);
            if (stream == null)
                throw new Exception(string.Format("Unable to find '{0}' embedded resource", resourceFile));
            var s = new Scanner(new StreamReader(stream)) { Filename = resourceFile };
            var p = new Parser(s);
            Datum d;
            while ((d = p.parse()) != null)
            {
                yield return d;
            }
        }

        /**
         * Used for bootstrapping various .lisp files into the environment.
         */
        public static void ExecuteResource(Environment env, string resourceFile)
        {
            var evaluator = new Evaluator();
            foreach (var d in ReadDatums(resourceFile))
                evaluator.Evaluate(env, d);
        }
    }
}
