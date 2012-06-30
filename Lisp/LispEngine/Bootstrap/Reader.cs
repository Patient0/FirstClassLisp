using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LispEngine.Core;
using LispEngine.Datums;
using LispEngine.Evaluation;
using LispEngine.Lexing;
using LispEngine.Parsing;
using Environment = LispEngine.Evaluation.Environment;

namespace LispEngine.Bootstrap
{
    class Reader
    {
        class Eof
        {
            public override string ToString()
            {
                return "#!eof";
            }
        }

        private static readonly Datum eof = new Eof().ToAtom();

        class Read : UnaryFunction
        {
            protected override Datum eval(Datum arg)
            {
                var parser = (Parser)arg.CastObject();
                return parser.parse() ?? eof;
            }

            public override string ToString()
            {
                return ",read";
            }

            public static readonly StackFunction Instance = new Read().ToStack();
        }

        class IsEof : UnaryFunction
        {
            protected override Datum eval(Datum arg)
            {
                return eof.Equals(arg).ToAtom();
            }

            public override string ToString()
            {
                return ",eof-object?";
            }

            public static readonly StackFunction Instance = new IsEof().ToStack();
        }

        class OpenInputString : UnaryFunction
        {
            protected override Datum eval(Datum arg)
            {
                return new Parser(Scanner.Create(arg.CastString())).ToAtom();
            }

            public override string ToString()
            {
                return ",open-input-string";
            }

            public static readonly StackFunction Instance = new OpenInputString().ToStack();
        }

        class OpenInputStream : UnaryFunction
        {
            protected override Datum eval(Datum arg)
            {
                return new Parser(new Scanner((TextReader) arg.CastObject())).ToAtom();
            }

            public override string ToString()
            {
                return ",open-input-stream";
            }

            public static readonly StackFunction Instance = new OpenInputStream().ToStack();
        }

        public static Environment AddTo(Environment env)
        {
            env = env.Extend("read", Read.Instance);
            env = env.Extend("open-input-string", OpenInputString.Instance);
            env = env.Extend("eof-object?", IsEof.Instance);
            env = env.Extend("open-input-stream", OpenInputStream.Instance);
            return env;
        }
    }
}
