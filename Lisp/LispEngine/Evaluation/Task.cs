using System.Collections.Generic;

namespace LispEngine.Evaluation
{
    public interface Task
    {
        Continuation Perform(Continuation c);
    }
}
