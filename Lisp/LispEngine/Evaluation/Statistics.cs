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
        public int Steps { get; set; }
        public int Expansions { get; set; }

        public override string ToString()
        {
            return string.Format("Steps: {0} Expansions: {1}", Steps, Expansions);
        }

        public Statistics()
        {
        }

        public Statistics(Statistics s)
        {
            this.Steps = s.Steps;
            this.Expansions = s.Expansions;
        }

        public Statistics Delta(Statistics prev)
        {
            return new Statistics {Steps = Steps - prev.Steps, Expansions = Expansions - prev.Expansions};
        }

        public Statistics Snapshot()
        {
            return new Statistics(this);
        }

        // This is a bit hacky - can't figure out a better way to "supply" the statistics
        // object yet. Basically, I want to expose "get-counter" to Lisp, but not "statistics"
        // itself... I guess.
        public Environment AddTo(Environment env)
        {
            env = env.Extend("!get-statistics", DatumHelpers.MakeFunction(Snapshot, "!get-statistics"));
            env = env.Extend("!get-statistics-delta", DatumHelpers.MakeFunction<Statistics, Statistics>(Delta, "!get-statistics-delta"));
            return env;
        }
    }
}
