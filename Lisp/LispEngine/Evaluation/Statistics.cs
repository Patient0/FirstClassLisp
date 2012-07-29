using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using LispEngine.Core;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    /**
     * Used by the interpreter to update statistics about the run time.
     */
    public class Statistics
    {
        public int Steps { get; set; }
        public int Expansions { get; set; }
        public int Lookups { get; set; }

        public override string ToString()
        {
            return string.Format("Steps: {0} Expansions: {1} Lookups: {2}", Steps, Expansions, Lookups);
        }

        public Statistics()
        {
        }

        public Statistics(Statistics s)
        {
            this.Steps = s.Steps;
            this.Expansions = s.Expansions;
            this.Lookups = s.Lookups;
        }

        public Statistics Delta(Statistics prev)
        {
            return new Statistics {Steps = Steps - prev.Steps, Expansions = Expansions - prev.Expansions, Lookups = Lookups - prev.Lookups};
        }

        public Statistics Snapshot()
        {
            return new Statistics(this);
        }

        // This is a bit hacky - can't figure out a better way to "supply" the statistics
        // object yet. Basically, I want to expose "get-counter" to Lisp, but not "statistics"
        // itself... I guess.
        public LexicalEnvironment AddTo(LexicalEnvironment env)
        {
            env.Define("!get-statistics", DelegateFunctions.MakeFunction(Snapshot, "!get-statistics"));
            env.Define("!get-statistics-delta", DelegateFunctions.MakeFunction<Statistics, Statistics>(Delta, "!get-statistics-delta"));
            return env;
        }
    }
}
