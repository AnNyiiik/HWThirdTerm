namespace ThirdHW;

using System;
using System.Collections.Concurrent;
using System.Threading;

/// <summary>
/// Class which enables to perform several tasks parallely.
/// </summary>
public class MyThreadPool
{
    private int maxTimeForCompleteJointPerThread;
    private Thread[] threads;
    private CancellationTokenSource cancellationTokenSource;
    private AutoResetEvent newTaskIsAwaiting;
    private ManualResetEvent areAnyTasksInQueue;
    private WaitHandle[] waitHandlers;
    private object synchronizationObject;
    private int workingThreads;
    private volatile bool[] isWorking;

    /// <summary>
    /// Instantiates a new instance of MyThreadPool class.
    /// </summary>
    public MyThreadPool(int numberOfThreads, int maxTimeForCompleteJointPerThread = 1000)
    {
        if (numberOfThreads <= 0)
        {
            throw new InvalidDataException();
        }

        this.isWorking = new bool[numberOfThreads];
        this.Tasks = new ConcurrentQueue<Action>();
        this.threads = new Thread[numberOfThreads];
        this.cancellationTokenSource = new CancellationTokenSource();
        this.newTaskIsAwaiting = new AutoResetEvent(false);
        this.areAnyTasksInQueue = new ManualResetEvent(false);
        this.waitHandlers = new WaitHandle[2]{this.newTaskIsAwaiting,
            this.areAnyTasksInQueue};
        this.maxTimeForCompleteJointPerThread = maxTimeForCompleteJointPerThread;
        this.synchronizationObject = new object();
        Start();
    }

    public ConcurrentQueue<Action> Tasks { get; private set; }

    /// <summary>
    /// Returns a number of threads, which calculates task at the current moment.
    /// </summary>
    public int WorkingThreads { get; private set; }

    /// <summary>
    /// Indicates if the thread pool is active or not (ShutDown was requested).
    /// </summary>
    public bool IsTerminated { get; private set; }

    private void Start()
    {
        for (var i = 0; i < threads.Length; ++i)
        {
            var localI = i;
            threads[i] = new Thread(() =>
            {
                while (!cancellationTokenSource.IsCancellationRequested)
                {
                    WaitHandle.WaitAny(waitHandlers);
                    if (cancellationTokenSource.IsCancellationRequested)
                    {
                        break;
                    }

                    lock (Tasks)
                    {
                        if (Tasks.Count > 0)
                        {
                            areAnyTasksInQueue.Set();
                        }
                        else
                        {
                            areAnyTasksInQueue.Reset();
                        }
                    }

                    if (!Tasks.IsEmpty)
                    {
                        var isAvailable = Tasks.TryDequeue(out var action);
                        if (isAvailable && action != null)
                        {
                            Interlocked.Increment(ref workingThreads);
                            WorkingThreads = workingThreads;
                            isWorking[localI] = true;
                            action.Invoke();
                            isWorking[localI] = false;
                            Interlocked.Decrement(ref workingThreads);
                            WorkingThreads = workingThreads;
                            action = null;
                        }
                    }

                    lock (Tasks)
                    {
                        if (Tasks.Count > 0)
                        {
                            areAnyTasksInQueue.Set();
                        }
                        else
                        {
                            areAnyTasksInQueue.Reset();
                        }
                    }
                }
            });
            threads[i].Start();
        }
    }

    /// <summary>
    /// Add new task item to the queue of tasks.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="function"></param>
    /// <param name="isUpperTaskCompleted"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public IMyTask<T1> AddTask<T1>(Func<T1> function, ManualResetEvent? isUpperTaskCompleted = null)
    {
        lock (this.synchronizationObject)
        {
            if (this.cancellationTokenSource.IsCancellationRequested)
            {
                throw new InvalidOperationException("Shutdown has been requested");
            }

            var myTask = isUpperTaskCompleted == null ? new MyTask<T1>(function, this)
                : new MyTask<T1>(function, this, isUpperTaskCompleted);
            lock (this.Tasks)
            {
                this.Tasks.Enqueue(() => myTask.Performe());
            }

            this.newTaskIsAwaiting.Set();
            return myTask;
        }
    }

    /// <summary>
    /// Terminates the work of thread pool and makes threads to performe already started tasks.
    /// </summary>
    /// <exception cref="EternalTaskException"></exception>
    public void ShutDown()
    {
        lock (this.synchronizationObject)
        {
            this.cancellationTokenSource.Cancel();
            this.areAnyTasksInQueue.Set();
            for (var i = 0; i < this.threads.Length; ++i)
            {
                this.threads[i].Join(this.maxTimeForCompleteJointPerThread);
                if (isWorking[i])
                {
                    throw new EternalTaskException("The task hasn't been finished" +
                        $" for {this.maxTimeForCompleteJointPerThread} milliseconds");
                }
            }

            this.IsTerminated = true;
        }
    }

    private class MyTask<T1> : IMyTask<T1>
    {
        private Func<T1> function;
        private T1? result;
        private bool isResultReady;
        private MyThreadPool threadPool;

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
            this.threadPool = myThreadPool;
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
            lock (threadPool.Tasks)
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

                lock (threadPool.Tasks)
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
}