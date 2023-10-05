namespace ThirdHW;

public class MyTask<T1> : IMyTask<T1>
{
	private bool isCompleted;

	private CancellationToken cancellationToken;

	private MyThreadPool myThreadPool;

	private Func<T1> function;

	private T1? result;

	private Exception? exception;

	private ManualResetEvent manualResetEventForContinuations;

	private ManualResetEvent manualResetEventForResult;

	private ManualResetEvent? manualResetEventIsUpperTaskCompleted;

	private List<Action> continuations;

	public MyTask(Func<T1> func, MyThreadPool myThreadPool,
		CancellationToken cancellationToken,
		ManualResetEvent? manualResetEventIsUpperTaskCompleted = null)
	{
		function = func;
		this.myThreadPool = myThreadPool;
		manualResetEventForContinuations = new ManualResetEvent(false);
		manualResetEventForResult = new ManualResetEvent(false);
		this.manualResetEventIsUpperTaskCompleted = manualResetEventIsUpperTaskCompleted;
		IsContinuation = manualResetEventIsUpperTaskCompleted == null ? false :
			true;
		continuations = new List<Action>();
	}

	public bool IsContinuation { get; }

	public bool IsCompleted { get => isCompleted; }

	public T1 Result
	{
		get
		{
			if (cancellationToken.IsCancellationRequested && !isCompleted)
			{
				throw new InvalidOperationException("shutdown was requested" +
					"and task hadn't been completed");
			}
			manualResetEventForResult.WaitOne();
			if (exception != null)
			{
				throw new AggregateException(exception);
			} else
			{
				return result!;
			}
		} 
	}

	public IMyTask<T2> ContinueWith<T2>(Func<T1, T2> func)
	{
		if (!cancellationToken.IsCancellationRequested)
		{
			lock(continuations)
			{
				if (result != null)
				{
					return(myThreadPool.AddTask<T2>(() => func(result),
						manualResetEventForContinuations));
                }
                var task = new MyTask<T2>(() => func(this.Result),
					myThreadPool,
					cancellationToken,
					manualResetEventForContinuations);
                continuations.Add(() => task.Performe());
                return task;
			}
		} else
		{
			throw new InvalidOperationException();
		}
	}

	public void Performe()
	{
		try
		{
			if (manualResetEventIsUpperTaskCompleted != null)
			{
				manualResetEventIsUpperTaskCompleted.WaitOne();
			}
			result = function();
            manualResetEventForResult.Set();
            lock (continuations)
			{
				if (continuations.Count > 0)
				{
					foreach(var continuation in continuations)
					{
						myThreadPool.AddTask(() => continuation, manualResetEventForResult);
					}
				}
			}
			isCompleted = true;
			manualResetEventForContinuations.Set();

		} catch (Exception e)
		{
			exception = e;
		}
	}
}

