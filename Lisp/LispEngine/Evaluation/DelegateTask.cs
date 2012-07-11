using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LispEngine.Evaluation
{
    using TaskDelegate = Func<Continuation, Continuation>;

    sealed class DelegateTask : Task
    {
        private readonly string fmt;
        private readonly object[] items;
        private readonly TaskDelegate taskDelegate;

        public DelegateTask(TaskDelegate taskDelegate, string fmt, object[] items)
        {
            this.fmt = fmt;
            this.items = items;
            this.taskDelegate = taskDelegate;
        }

        public Continuation Perform(Continuation c)
        {
            return taskDelegate(c);
        }
        public override string ToString()
        {
            return string.Format(fmt, items);
        }
    }
}
