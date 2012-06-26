using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;
using Environment = LispEngine.Evaluation.Environment;

namespace LispEngine.Core
{
    class Set : AbstractFExpression
    {
        public static readonly FExpression Instance = new Set();

        class SetName : Task
        {
            private readonly Environment env;
            private readonly string name;
            public SetName(Environment env, string name)
            {
                this.env = env;
                this.name = name;
            }

            public Continuation Perform(Continuation c)
            {
                env.Set(name, c.Result);
                return c;
            }

            public override string ToString()
            {
                return string.Format("set! '{0}'", name);
            }
        }

        public override Continuation Evaluate(Continuation c, Environment env, Datum args)
        {
            var argList = args.ToArray();
            if (argList.Length != 2)
                throw c.error("Expected 2 arguments: (set! <symbol> <expression>). Got {0} instead", argList.Length);
            var name = argList[0] as Symbol;
            if (name == null)
                throw c.error("Invalid syntax for set!. '{0}' should be a symbol", argList[0]);
            var expression = argList[1];
            c = c.PushTask(new SetName(env, name.Identifier));
            return c.Evaluate(env, expression);
        }

        public override string ToString()
        {
            return ",set!";
        }
    }
}
