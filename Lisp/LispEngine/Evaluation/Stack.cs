using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LispEngine.Evaluation
{
    sealed class Stack<T> : IStack<T>
    {
        private sealed class EmptyStack : IStack<T>
        {
            public T Peek()
            {
                throw new Exception("Empty stack");
            }

            public IStack<T> Pop()
            {
                throw new Exception("Empty stack");
            }

            public IStack<T> Push(T t)
            {
                return new Stack<T>(t, this);
            }
        }

        public static readonly IStack<T> Empty = new EmptyStack();

        private readonly T head;
        private readonly IStack<T> tail;
 
        private Stack(T head, IStack<T> tail)
        {
            this.head = head;
            this.tail = tail;
        }
        public T Peek()
        {
            return head;
        }

        public IStack<T> Pop()
        {
            return tail;
        }

        public IStack<T> Push(T t)
        {
            return new Stack<T>(t, this);
        }
    }
}
