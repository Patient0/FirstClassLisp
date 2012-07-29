namespace LispEngine.Evaluation
{
    public interface IStack<T>
    {
        T Peek();
        IStack<T> Pop();
        IStack<T> Push(T t);
    }
}
