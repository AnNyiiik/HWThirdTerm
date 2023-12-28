namespace ThirdHW;

public class MyTask<T1> : IMyTask<T1>
{
    private Func<T1> function;
    private T1? result;
    private bool isResultReady;

    private List<Action> continuations;

    private ManualResetEvent accessToResult;
    private ManualResetEvent? isUpperTaskCompleted;
    private ManualResetEvent manualResetEventForContinuations;

    private Exception? exception;
    private MyThreadPool myThreadPool;

    /// <summary>
    /// Initializes a new instance of the <see cref="MyTask{TResult}"/> class.
    /// </summary>
    /// <param name="function">Task function.</param>
    /// <param name="myThreadPool">ThreadPool that will execute this task.</param>
    /// <param name="isUpperTaskCompleted">Parental task ManualResetEvent (for continuation task).</param>
    public MyTask(Func<T1> function, MyThreadPool myThreadPool,
        ManualResetEvent? manualResetEventForContinuations = null)
    {
        this.function = function;
        this.continuations = new List<Action>();
        this.accessToResult = new ManualResetEvent(false);
        this.myThreadPool = myThreadPool;
        this.isUpperTaskCompleted = manualResetEventForContinuations;
        this.manualResetEventForContinuations = new ManualResetEvent(false);
    }

    /// <summary>
    /// Returns true value if task has been already calculated.
    /// </summary>
    public bool IsCompleted => this.isResultReady;

    /// <summary>
    /// Returns the task result.
    /// </summary>
    public T1 Result
    {
        get
        {
            if (this.myThreadPool.IsTerminated && !this.isResultReady)
            {
                throw new InvalidOperationException("Hasn't been started when the Shutdown was requested.");
            }

            this.accessToResult.WaitOne();
            if (this.exception != null)
            {
                throw new AggregateException(this.exception);
            }

            return this.result!;
        }
    }

    /// <summary>
    /// Creates a new task which operates with the result of this task.
    /// </summary>
    /// <typeparam name="T2">Value type for the continuation result.</typeparam>
    /// <param name="func">Function for creating a new task.</param>
    /// <returns>Task with new return value type of the function.</returns>
    public IMyTask<T2> ContinueWith<T2>(Func<T1, T2> func)
    {
        lock (this.continuations)
        {
            if (this.result != null)
            {
                return this.myThreadPool.AddTask(() => func(this.Result), this.manualResetEventForContinuations);
            }

            var continuation = new MyTask<T2>(
                () => func(this.Result),
                this.myThreadPool,
                this.manualResetEventForContinuations);
            this.continuations.Add(() => continuation.Performe());
            return continuation;
        }
    }

    /// <summary>
    /// Calculates task result.
    /// </summary>
    public void Performe()
    {
        try
        {
            if (isUpperTaskCompleted != null)
            {
                this.isUpperTaskCompleted!.WaitOne();
            }

            this.result = this.function();

            lock (this.continuations)
            {
                if (this.continuations.Count > 0)
                {
                    foreach (var continuation in this.continuations)
                    {
                        this.myThreadPool.AddTask(() => continuation, this.isUpperTaskCompleted);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            this.exception = ex;
        }

        this.isResultReady = true;
        this.accessToResult.Set();
        this.manualResetEventForContinuations.Set();
    }
}