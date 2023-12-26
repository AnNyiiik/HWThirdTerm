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

	public MyTask(Func<T1> function, MyThreadPool myThreadPool,
		CancellationToken cancellationToken,
		ManualResetEvent? manualResetEventIsUpperTaskCompleted = null)
	{
	    this.function = function;
		this.cancellationToken = cancellationToken;
	    this.myThreadPool = myThreadPool;
	    this.manualResetEventForContinuations = new ManualResetEvent(false);
	    this.manualResetEventForResult = new ManualResetEvent(false);
	    this.manualResetEventIsUpperTaskCompleted = manualResetEventIsUpperTaskCompleted;
	    this.IsContinuation = manualResetEventIsUpperTaskCompleted == null ? false :
		    true;
	    this.continuations = new List<Action>();
		this.IsCompleted = isCompleted;
	}

	public bool IsContinuation { get; }

	public bool IsCompleted { get; private set; }

    /// <summary>
    /// Returns the result of a task if it is ready.
    /// If it is not completed yet and shoutdown isn't requested,
    /// makes thread to wait until the result is ready. Throws
	/// InvalidOperationException if shoutdown was requested.
    /// </summary>
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
			}
		    else
			{
			    return result!;
			}
		} 
	}

	/// <summary>
	/// Add a continuation to the task.
	/// </summary>
	/// <typeparam name="T2"></typeparam>
	/// <param name="func"></param>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException"></exception>
	public IMyTask<T2> ContinueWith<T2>(Func<T1, T2> func)
	{
	    if (!cancellationToken.IsCancellationRequested)
		{
		    lock(continuations)
			{
				lock(myThreadPool.Tasks)
				{
                    if (result != null)
                    {
                        return (myThreadPool.AddTask<T2>(() => func(result),
                            manualResetEventForContinuations));
                    }
                    var task = new MyTask<T2>(() => func(this.Result),
                        myThreadPool,
                        cancellationToken,
                        manualResetEventForContinuations);
                    continuations.Add(() => task.Performe());
                    return task;
                }
			}
		}
		else
		{
		    throw new InvalidOperationException();
		}
	}

	/// <summary>
	/// Performe the task.
	/// </summary>
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
						lock(myThreadPool.Tasks)
						{
                            myThreadPool.AddTask(() => continuation, manualResetEventForResult);
                        }
					}
				}
			}
		    isCompleted = true;
		    manualResetEventForContinuations.Set();

		}
	    catch (Exception e)
		{
			exception = e;
		}
	}
}