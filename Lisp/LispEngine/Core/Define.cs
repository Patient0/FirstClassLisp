﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;
using LispEngine.Stack;
using Environment = LispEngine.Evaluation.Environment;

namespace LispEngine.Core
{
    class Define : AbstractFExpression
    {
        public static readonly FExpression Instance = new Define();

        class DefineName : Task
        {
            private readonly Environment env;
            private readonly string name;
            public DefineName(Environment env, string name)
            {
                this.env = env;
                this.name = name;
            }

            public void Perform(EvaluatorStack stack)
            {
                var result = stack.PopResult();
                env.Define(name, result);
                stack.PushResult(result);
            }
        }

        class Ignore : Task
        {
            public static readonly Task Instance = new Ignore();
            public void Perform(EvaluatorStack stack)
            {
                stack.PopResult();
            }
        }

        public override void Evaluate(EvaluatorStack evaluator, Environment env, Datum args)
        {
            var argList = DatumHelpers.enumerate(args).ToArray();
            if (argList.Length < 2)
                throw DatumHelpers.error("Expected at least 2 arguments for define. Got {0}", argList.Length);
            var name = argList[0] as Symbol;
            if (name == null)
                throw DatumHelpers.error("Invalid define syntax. '{0}' should be a symbol", argList[0]);
            evaluator.PushTask(new DefineName(env, name.Identifier));

            // Scope any local definitions.
            var localEnv = new Environment(env);

            var remaining = argList.Skip(1).Reverse().ToArray();
            for (var i = 0; i < remaining.Length; ++i)
            {
                if(i > 0)
                    evaluator.PushTask(Ignore.Instance);
                evaluator.Evaluate(localEnv, remaining[i]);
            }
        }
    }
}
