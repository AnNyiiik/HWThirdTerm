namespace SecondHW;

public class LazySingleThreadImpl<T>: ILazy<T>
{
    private bool isFirstSummon = true;
    private T? Result;
    private Exception? Exception;
    private Func<T?>? Supplier;

    public LazySingleThreadImpl(Func<T> function) => Supplier = function;

	public T? Get()
	{
		if (isFirstSummon)
        {
            isFirstSummon = false;
            try
            {
                Result = Supplier!();
            } catch (Exception e)
            {
                Exception = e;
            } finally
            {
                Supplier = null;
            }
        }
        return (Exception == null) ? Result : throw Exception;
    }
}
