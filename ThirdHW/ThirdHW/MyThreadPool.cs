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
    private ConcurrentQueue<Action> tasks;
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
        this.tasks = new ConcurrentQueue<Action>();
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

    /// <summary>
    /// Returns a number of threads, which calculates task at the current moment.
    /// </summary>
    public int WorkingThreads { get => workingThreads; }

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

                    lock (tasks)
                    {
                        if (tasks.Count > 0)
                        {
                            areAnyTasksInQueue.Set();
                        }
                        else
                        {
                            areAnyTasksInQueue.Reset();
                        }
                    }

                    if (!tasks.IsEmpty)
                    {
                        var isAvailable = tasks.TryDequeue(out var action);
                        if (isAvailable && action != null)
                        {
                            Interlocked.Increment(ref workingThreads);
                            isWorking[localI] = true;
                            action.Invoke();
                            isWorking[localI] = false;
                            Interlocked.Decrement(ref workingThreads);
                            action = null;
                        }
                    }

                    lock (tasks)
                    {
                        if (tasks.Count > 0)
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
            lock (this.tasks)
            {
                this.tasks.Enqueue(() => myTask.Performe());
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
}