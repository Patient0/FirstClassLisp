using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    using FrameBindings = IStack<LexicalEnvironment.Binding>;

    public class LexicalEnvironment
    {
        public class Binding
        {
            private readonly int symbolId;

            public Binding(Symbol symbol, Datum value)
            {
                this.symbolId = symbol.ID;
                this.Value = value;
            }

            public Datum Value { get; set; }
            public int SymbolID { get { return symbolId; } }
        }

        private Statistics statistics;
        private readonly LexicalEnvironment parent;
        private IStack<Binding> bindings;

        private LexicalEnvironment(LexicalEnvironment parent, IStack<Binding> bindings)
        {
            this.statistics = parent == null ? null : parent.statistics;
            this.parent = parent;
            this.bindings = bindings;
        }

        public static FrameBindings EmptyFrame = Stack<Binding>.Empty.Push(null);

        private static LexicalEnvironment newFrame(LexicalEnvironment parent, FrameBindings bindings)
        {
            return new LexicalEnvironment(parent, bindings);
        }

        public Statistics Statistics
        {
            set { statistics = value; }
        }

        public LexicalEnvironment NewFrame(FrameBindings frameBindings)
        {
            return newFrame(this, frameBindings);
        }

        public LexicalEnvironment NewFrame()
        {
            return NewFrame(EmptyFrame);
        }

        public static LexicalEnvironment Create()
        {
            return newFrame(null, EmptyFrame);
        }

        public LexicalEnvironment Define(Symbol name, Datum value)
        {
            this.bindings = bindings.Push(new Binding(name, value));
            return this;
        }

        public LexicalEnvironment Define(string name, Datum binding)
        {
            return Define(Symbol.GetSymbol(name), binding);
        }

        private static Exception undefined(Symbol symbol)
        {
            return DatumHelpers.error("Undefined symbol '{0}'", symbol);
        }

        public Binding Find(Symbol symbol)
        {
            if (statistics != null)
                statistics.Lookups++;
            var b = bindings;
            var id = symbol.ID;
            while (b.Peek() != null)
            {
                if (id == b.Peek().SymbolID)
                    return b.Peek();
                b = b.Pop();
            }
            if (parent == null)
                throw undefined(symbol);
            return parent.Find(symbol);
        }

        public void Set(Symbol symbol, Datum value)
        {
            Find(symbol).Value = value;
        }

        public Datum Lookup(Symbol symbol)
        {
            return Find(symbol).Value;
        }

        public sealed class Location
        {
            private readonly int frame;
            private readonly int index;

            public Location(int frame, int index)
            {
                this.frame = frame;
                this.index = index;
            }

            public Binding Find(LexicalEnvironment e)
            {
                for (var f = 0; f < frame; ++f)
                    e = e.parent;
                var b = e.bindings;
                for (var i = 0; i < index; ++i)
                    b = b.Pop();
                return b.Peek();
            }

            public override string ToString()
            {
                return string.Format("Frame: {0} Index: {1}", frame, index);
            }

            public bool Equals(Location other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return other.frame == frame && other.index == index;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof (Location)) return false;
                return Equals((Location) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (frame*397) ^ index;
                }
            }

            public static bool operator ==(Location left, Location right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(Location left, Location right)
            {
                return !Equals(left, right);
            }
        }

        private int getIndex(Symbol symbol)
        {
            var index = 0;
            var b = bindings;
            var id = symbol.ID;
            while (b.Peek() != null)
            {
                if (id == b.Peek().SymbolID)
                    return index;
                b = b.Pop();
                ++index;
            }
            return -1;
        }

        public Location LookupLocation(Symbol symbol)
        {
            var e = this;
            var frame = 0;
            while(e != null)
            {
                var index = e.getIndex(symbol);
                if(index != -1)
                    return new Location(frame, index);
                ++frame;
                e = e.parent;
            }
            throw undefined(symbol);
        }
    }
}
