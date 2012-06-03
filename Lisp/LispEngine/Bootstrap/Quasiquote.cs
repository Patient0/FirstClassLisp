using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;

namespace LispEngine.Bootstrap
{
    // Implement in C# for now, until we've got it all figured out. Then
    // it can be implemented in Lisp
    class Quasiquote : DatumHelpers, Function
    {
        private readonly Datum consF;
        private readonly Datum quoteF;
        public Quasiquote(ImmutableEnvironment env)
        {
            this.consF = env.Lookup("cons");
            this.quoteF = env.Lookup("quote");
        }

        private Datum expand(Datum arg)
        {
            var pair = arg as Pair;
            if (pair != null)
            {
                // `,x => (quasiquote (unquote x)) => x
                if(pair.First.Equals(unquote))
                    return car(pair.Second);

                // `(1 2) => (cons (quote 1) (quote 2))
                return compound(consF, expand(pair.First), expand(pair.Second));
            }
            // Should expand x => (quote x)
            return cons(quoteF, compound(arg));
        }

        public Datum Evaluate(Evaluator evaluator, Datum args)
        {
            var argList = enumerate(args).ToArray();
            if (argList.Length != 1)
                throw error("invalid syntax '{0}'", args);
            return expand(argList[0]);
        }
    }
}
