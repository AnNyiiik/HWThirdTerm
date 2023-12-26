using StyleCop;

namespace SecondHW
{
	public class LazyMultyThreadImplementation<T>: ILazy<T>
	{
		private bool isFirstSummon = true;
		private volatile bool valueFlag = false;
		private bool exceptionFlag = false;
		private T? result;
		private volatile Exception? exception;
		private Func<T>? supplier;
		private Object synchronizationObject;

		public LazyMultyThreadImplementation(Func<T> function)
		{
			synchronizationObject = new Object();
			supplier = function;
		}

		public T? Get()
		{
		    if (Volatile.Read(ref isFirstSummon))
			{
				lock(synchronizationObject)
				{
					if (Volatile.Read(ref isFirstSummon))
					{
						try
						{
							result = supplier!();
						    valueFlag = true;
						} catch (Exception e)
						{
							exception = e;
							exceptionFlag = true;
						} finally
						{
							supplier = null;
							Volatile.Write(ref isFirstSummon, false);
						}
					}
				}
			}

            if (!Volatile.Read(ref exceptionFlag) && valueFlag)
			{
				return result;
			} else if (exceptionFlag && exception != null) {
				throw exception;
			} else
			{
				throw new Exception();
			}
		}
	}
}