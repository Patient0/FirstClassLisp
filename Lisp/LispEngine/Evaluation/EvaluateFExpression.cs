using LispEngine.Core;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    class EvaluateFExpression : Task
    {
        private readonly Datum args;
        private readonly Environment env;

        class FExpressionConverter : AbstractVisitor<FExpression>
        {
            public override FExpression visit(FExpression f)
            {
                return f;
            }

            public override FExpression defaultCase(Datum d)
            {
                throw DatumHelpers.error("'{0}' is not callable", d);
            }

            public override FExpression visit(StackFunction f)
            {
                return new FunctionExpression(f);
            }

            public static readonly DatumVisitor<FExpression> Instance = new FExpressionConverter();
        }
        private static FExpression toFExpression(Datum d)
        {
            return d.accept(FExpressionConverter.Instance);
        }

        public EvaluateFExpression(Datum args, Environment env)
        {
            this.args = args;
            this.env = env;
        }

        public Continuation Perform(Continuation c)
        {
            var fexpression = toFExpression(c.Result);
            return fexpression.Evaluate(c.PopResult(), env, args);
        }

        public override string ToString()
        {
            return string.Format("EvaluateFexpression: {0}", args);
        }
    }
}
