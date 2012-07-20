using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Core;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    /**
     * Used by the interpreter to update statistics about the run time.
     * We use this to expose performance data to the REPL.
     * I've probably over-optimized for performance here: Each "statistic"
     * is a typesafe mutable value so that when a relevant part of the interpreter
     * has it it can simply "bump" the values as necessary. I didn't
     * want the act of measuring something to interfere too much with
     * the runtime performance.
     */

    public class Statistics
    {
        public class Statistic<T>
        {
            public T Value { get; set; }
        }

        // Does this class exist as a concept somewhere in .Net?
        private class DefaultDictionary<T>
        {
            private readonly IDictionary<string, Statistic<T>> dictionary = new Dictionary<string, Statistic<T>>();

            public Statistic<T> Get(string name)
            {
                Statistic<T> s;
                if (!dictionary.TryGetValue(name, out s))
                {
                    s = new Statistic<T>();
                    dictionary[name] = s;
                }
                return s;
            }
        }

        private readonly DefaultDictionary<int> counters = new DefaultDictionary<int>();
    
        public Statistic<int> GetCounter(string name)
        {
            return counters.Get(name);
        }

        private class GetStatistic<T> : UnaryFunction
        {
            private readonly Func<string, Statistic<T> > accessor;
            public GetStatistic(Func<string, Statistic<T> > accessor)
            {
                this.accessor = accessor;
            }

            protected override Datum eval(Datum name)
            {
                return accessor(name.CastString()).Value.ToAtom();
            }
        }

        private static UnaryFunction makeStatisticFunction<T>(Func<String, Statistic<T>> func)
        {
            return new GetStatistic<T>(func);
        }

        // This is a bit hacky - can't figure out a better way to "supply" the statistics
        // object yet. Basically, I want to expose "get-counter" to Lisp, but not "statistics"
        // itself... I guess.
        public Environment AddTo(Environment env)
        {
            env = env.Extend("get-counter", makeStatisticFunction(GetCounter).ToStack());
            env = env.Extend("statistics", this.ToAtom());
            return env;
        }

        public static Statistics Get(Environment env)
        {
            Datum d;
            Statistics statistics = null;
            if (env.TryLookup("statistics", out d))
            {
                var a = d as Atom;
                if (a != null)
                    statistics = a.Value as Statistics;
            }
            return statistics ?? new Statistics();
        }
    }
}
