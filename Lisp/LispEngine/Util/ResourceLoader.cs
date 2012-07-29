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

namespace LispEngine.Util
{
    public class ResourceLoader
    {
        public static IEnumerable<Datum> ReadDatums(string resourceFile)
        {
            return ReadDatums(Assembly.GetCallingAssembly(), resourceFile);
        }

        public static IEnumerable<Datum> ReadDatums(Assembly assembly, string resourceFile)
        {
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

        public static void ExecuteResource(LexicalEnvironment env, string resourceFile)
        {
            ExecuteResource(new Statistics(), Assembly.GetCallingAssembly(), env, resourceFile);
        }

        /**
         * Used for bootstrapping various .lisp files into the environment.
         */
        public static void ExecuteResource(Statistics statistics, Assembly assembly, LexicalEnvironment env, string resourceFile)
        {
            var evaluator = new Evaluator();
            foreach (var d in ReadDatums(assembly, resourceFile))
                evaluator.Evaluate(statistics, env, d);
        }
    }
}
