using System.Linq;
using LispEngine.Datums;
using LispEngine.Evaluation;
using Environment = LispEngine.Evaluation.Environment;

namespace LispEngine.Core
{
    class Closure : DatumHelpers, Function
    {
        private readonly Environment environment;
        private readonly Bindings bindings;
        private readonly Datum body;

        public Closure(Environment environment, Bindings bindings, Datum body)
        {
            this.environment = environment;
            this.bindings = bindings;
            this.body = body;
        }

        public Datum Evaluate(Evaluator e, Datum args)
        {
            return e.evaluate(bindings.apply(environment, args), body);
        }
    }
}
