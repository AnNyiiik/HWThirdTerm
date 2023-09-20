namespace SecondHW;

public class LazySingleThreadImpl<T>: ILazy<T>
{
    private bool IsFirstSummon = true;
    private T? Result;
    private Exception? Exception;
    private Func<T>? Supplier;

    public LazySingleThreadImpl(Func<T> function) => Supplier = function;

	public T Get()
	{
		if (IsFirstSummon)
        {
            IsFirstSummon = false;
            try
            {
                Result = Supplier!();
            } catch (Exception)
            {
                Exception = new Exception();
            } finally
            {
                Supplier = null;
            }
        }
        return Exception == null ? Result : throw Exception;
    }
}
