using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LispEngine.Stack
{
    public interface Task
    {
        Continuation Perform(Continuation c);
    }
}
