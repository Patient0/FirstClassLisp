using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    // Alternative is to always wrap/unwrap
    // Function in Atom... but this could get
    // very confusing.
    public interface Function : Datum
    {
        Datum evaluate(Datum args);
    }
}
