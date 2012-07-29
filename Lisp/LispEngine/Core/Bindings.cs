﻿using System.Collections.Generic;
using LispEngine.Datums;
using LispEngine.Evaluation;

namespace LispEngine.Core
{
    // Something that knows how to bind some arguments to an environment.
    delegate IEnvironment Bindings(IEnvironment to, Datum args);
}
