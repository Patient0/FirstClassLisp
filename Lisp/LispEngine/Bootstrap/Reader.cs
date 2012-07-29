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

        public static LexicalEnvironment AddTo(LexicalEnvironment env)
        {
            env.Define("read", Read.Instance);
            env.Define("open-input-string", OpenInputString.Instance);
            env.Define("eof-object?", IsEof.Instance);
            env.Define("open-input-stream", OpenInputStream.Instance);
            return env;
        }
    }
}
