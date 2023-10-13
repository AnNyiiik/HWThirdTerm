namespace SecondHW
{
	public class LazyMultyThreadImpl<T>: ILazy<T>
	{
		private bool IsFirstSummon = true;
		private bool ValueFlag = false;
		private bool ExceptionFlag = false;
		private T? Result;
		private volatile Exception? Exception;
		private Func<T>? Supplier;
		private Object SynchronizationObject;

		public LazyMultyThreadImpl(Func<T> function)
		{
			SynchronizationObject = new Object();
			Supplier = function;
		}

		public T? Get()
		{
			if (Volatile.Read(ref IsFirstSummon))
			{
				lock(SynchronizationObject)
				{
					if (Volatile.Read(ref IsFirstSummon))
					{
						try
						{
                            Result = Supplier!();
							ValueFlag = true;
						} catch (Exception)
						{
							Exception = new Exception();
                            ExceptionFlag = true;
                        } finally
						{
							Supplier = null;
                            Volatile.Write(ref IsFirstSummon, false);
                        }
                    } 
                }
			}

            if (!Volatile.Read(ref ExceptionFlag) && ValueFlag)
			{
				return Result;
			} else if (ExceptionFlag) {
				throw Exception;
			} else
			{
				throw new Exception();
			}
		}
	}
}