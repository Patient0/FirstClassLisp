using System;
using System.Linq;
using LispEngine.Datums;
using LispEngine.Evaluation;
using Environment = LispEngine.Evaluation.Environment;

namespace LispEngine.Core
{
    class Closure : DatumHelpers, Function
    {
        private readonly Environment environment;
        private readonly Arguments arguments;
        private readonly Datum body;

        public class Arguments
        {
            // Used for toString
            private readonly Datum argDatum;
            private readonly Bindings bindings;
            public Arguments(Datum argDatum)
            {
                this.argDatum = argDatum;
                this.bindings = BindingTypes.parse(argDatum);
            }

            public Environment Bind(Environment environment, Datum args)
            {
                try
                {
                    return bindings.apply(environment, args);
                }
                catch (Exception e)
                {
                    throw error(e, "Could not bind '{0}' to '{1}'", args, argDatum);
                }
            }

            public override string ToString()
            {
                return argDatum.ToString();
            }
        }

        public Closure(Environment environment, Datum args, Datum body)
        {
            this.environment = environment;
            this.arguments = new Arguments(args);
            this.body = body;
        }

        public Datum Evaluate(Evaluator e, Datum args)
        {
            return e.Evaluate(arguments.Bind(environment, args), body);
        }

        public override string ToString()
        {
            return string.Format("(lambda {0} ...)", arguments);
        }
    }
}
