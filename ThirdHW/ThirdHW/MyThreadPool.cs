using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ThirdHW;

public class MyThreadPool
{
    private int numberOfThreads;
    private Thread[] threads;
    private CancellationTokenSource cancellationTokenSource;
    private WaitHandle[] waitHandles;
    private ManualResetEvent isThereAnyTasksInQueue;
    private AutoResetEvent newTaskIsAwaiting;
    private Object synchronizationObject;
    private int workingThreads;

    public MyThreadPool(int numberOfThreads)
    {
        this.numberOfThreads = numberOfThreads;
        this.threads = new Thread[numberOfThreads];
        this.cancellationTokenSource = new CancellationTokenSource();
        this.waitHandles = new WaitHandle[2];
        this.isThereAnyTasksInQueue = new ManualResetEvent(false);
        this.newTaskIsAwaiting = new AutoResetEvent(false);
        this.waitHandles[0] = isThereAnyTasksInQueue;
        this.waitHandles[1] = newTaskIsAwaiting;
        this.synchronizationObject = new Object();
        this.IsTerminated = false;
        this.Tasks = new ConcurrentQueue<Action>();
        Start();
    }

    public ConcurrentQueue<Action> Tasks { get; private set; }

    public int WorkingThreads { get => workingThreads; }

    public bool IsTerminated { get; private set; }

    /// <summary>
    /// Activates the thread-pool work.
    /// </summary>
    private void Start()
    {
        for (var i = 0; i < numberOfThreads; ++i)
        {
            threads[i] = new Thread(() =>
            {
                while (!cancellationTokenSource.IsCancellationRequested)
                {
                    if (cancellationTokenSource.IsCancellationRequested)
                    {
                        break;
                    }

                    WaitHandle.WaitAny(waitHandles);

                    lock (Tasks)
                    {
                        if (Tasks.Count > 0)
                        {
                            isThereAnyTasksInQueue.Set();
                        }
                        else
                        {
                            isThereAnyTasksInQueue.Reset();
                        }
                    }

                    if (Tasks.Count > 0)
                    {
                        var isavailable = Tasks.TryDequeue(out var action);
                        if (isavailable && action != null)
                        {
                            Interlocked.Increment(ref workingThreads);
                            action?.Invoke();
                            Interlocked.Decrement(ref workingThreads);
                        }
                    }

                    lock (Tasks)
                    {
                        if (Tasks.Count > 0)
                        {
                            isThereAnyTasksInQueue.Set();
                        }
                        else
                        {
                            isThereAnyTasksInQueue.Reset();
                        }
                    }
                }
             });
            threads[i].Start();
        }
    }

    /// <summary>
    /// Makes all the threads in the pool to complete their work and stop working.
    /// </summary>
    public void ShutDown()
    {
        if (!IsTerminated)
        {
            lock(Tasks)
            {
                cancellationTokenSource.Cancel();
            foreach (var thread in threads)
            {
                thread.Join();
            }
            IsTerminated = true;
            }
        }
    }

    /// <summary>
    /// Add task to the thread pool. 
    /// </summary>
    /// <typeparam name="T1">Type of a return value</typeparam>
    /// <param name="function">Action to performe</param>
    /// <param name="manualResetEventIsUpperTaskCompleted">Reset event to blok continuation task performance till the upper is ready</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public IMyTask<T1> AddTask<T1>(Func<T1> function, ManualResetEvent? manualResetEventIsUpperTaskCompleted = null)
    {
        if (!cancellationTokenSource.IsCancellationRequested)
        {
            lock(Tasks)
            {
                lock (synchronizationObject)
                {
                    MyTask<T1> newTask;
                    newTask = new MyTask<T1>(function, this,
                        cancellationTokenSource.Token,
                        manualResetEventIsUpperTaskCompleted);
                    Tasks.Enqueue(() => newTask.Performe());
                    newTaskIsAwaiting.Set();
                    return newTask;
                }
            }
        }
        else
        {
            throw new InvalidOperationException();
        }
        
    }
}