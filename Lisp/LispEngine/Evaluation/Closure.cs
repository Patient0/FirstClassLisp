using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    class Closure : DatumHelpers, Function
    {
        private readonly Environment environment;
        private readonly Evaluator evaluator;
        private readonly IEnumerable<string> argNames;
        private readonly Datum body;

        public Closure(Evaluator evaluator, Environment environment, IEnumerable<string> argNames, Datum body)
        {
            this.environment = environment;
            this.evaluator = evaluator;
            this.argNames = argNames;
            this.body = body;
        }

        private delegate Environment Binding(Environment e);

        private static Binding makeBinding(string name, Datum arg)
        {
            return e => e.Extend(name, arg);
        }

        public Datum evaluate(Datum args)
        {
            // Map the names the values. A "binding" is something that can
            // extend an environment with a new mapping.
            var mappings = argNames.Zip(enumerate(args), makeBinding);
            // Extend the environment with the new mappings.
            var closureEnvironment = mappings.Aggregate(environment, (env, binding) => binding(env));
            return evaluator.evaluate(closureEnvironment, body);
        }
    }
}
