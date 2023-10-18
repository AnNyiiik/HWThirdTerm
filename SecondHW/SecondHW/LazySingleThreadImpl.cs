namespace SecondHW;

public class LazySingleThreadImpl<T>: ILazy<T>
{
    private bool isFirstSummon = true;
    private T? result;
    private Exception? exception;
    private Func<T?>? supplier;

    public LazySingleThreadImpl(Func<T> function) => supplier = function;

	public T? Get()
	{
	    if (isFirstSummon)
        {
            isFirstSummon = false;
            try
            {
                result = supplier!();
            } catch (Exception e)
            {
                exception = e;
            } finally
            {
                supplier = null;
            }
        }
        return (exception == null) ? result : throw exception;
    }
}