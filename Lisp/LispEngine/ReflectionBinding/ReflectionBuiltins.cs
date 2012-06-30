using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;
using LispEngine.Util;
using Environment = LispEngine.Evaluation.Environment;

namespace LispEngine.ReflectionBinding
{
    class ReflectionBuiltins
    {
        class InvokeInstanceMethod : Function
        {
            public Datum Evaluate(Datum args)
            {
                var argArray = args.ToArray();
                var target = argArray[0].CastObject();
                var method = argArray[1].CastString();
                var methodArgs = argArray[2].Enumerate().Select(DatumHelpers.castObject).ToArray();
                var result = target.GetType().InvokeMember(method, BindingFlags.Default | BindingFlags.InvokeMethod, null, target, methodArgs);
                return result.ToAtom();
            }
        }

        public static Environment AddTo(Environment env)
        {
            env = env.Extend("invoke-instance", new InvokeInstanceMethod().ToStack());
            ResourceLoader.ExecuteResource(env, "LispEngine.ReflectionBinding.ReflectionBuiltins.lisp");
            return env;
        }
    }
}
