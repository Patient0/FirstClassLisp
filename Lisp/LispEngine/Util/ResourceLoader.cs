using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using LispEngine.Datums;
using LispEngine.Lexing;
using LispEngine.Parsing;

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
    }
}
