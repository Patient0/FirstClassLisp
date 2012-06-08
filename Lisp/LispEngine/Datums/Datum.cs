namespace LispEngine.Datums
{
    public interface Datum
    {
        T accept<T>(DatumVisitor<T> visitor);
    }
}
