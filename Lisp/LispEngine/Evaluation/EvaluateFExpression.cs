using LispEngine.Core;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    class EvaluateFExpression : Task
    {
        private readonly Datum args;
        private readonly LexicalEnvironment env;

        class FExpressionConverter : AbstractVisitor<FExpression>
        {
            private readonly Continuation c;
            public FExpressionConverter(Continuation c)
            {
                this.c = c;
            }
            public override FExpression visit(FExpression f)
            {
                return f;
            }

            public override FExpression defaultCase(Datum d)
            {
                throw c.error("'{0}' is not callable", d);
            }

            public override FExpression visit(StackFunction f)
            {
                return new FunctionExpression(f);
            }
        }
        private static FExpression toFExpression(Continuation c)
        {
            return c.Result.accept(new FExpressionConverter(c));
        }

        public EvaluateFExpression(Datum args, LexicalEnvironment env)
        {
            this.args = args;
            this.env = env;
        }

        public Continuation Perform(Continuation c)
        {
            var fexpression = toFExpression(c);
            return fexpression.Evaluate(c.PopResult(), env, args);
        }

        public override string ToString()
        {
            return string.Format("EvaluateFExpression: {0}", args);
        }
    }
}
