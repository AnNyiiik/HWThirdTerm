using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ThirdHW;

public class MyThreadPool
{
    private int numberOfThreads = 8;

    private Thread[] threads;

    private ConcurrentQueue<Action> tasks;

    private CancellationTokenSource cancellationTokenSource;

    private WaitHandle[] waitHandles;

    private ManualResetEvent isThereAnyTasksInQueue;

    private AutoResetEvent newTaskIsAwaiting;

    private Object synchronizationObject;

    private int workingThreads;

    public int WorkingThreads { get => workingThreads; }

    public bool IsTerminated { get; private set; }

    public MyThreadPool(int numberOfThreads)
    {
        this.numberOfThreads = numberOfThreads;
        threads = new Thread[numberOfThreads];
        tasks = new ConcurrentQueue<Action>();
        cancellationTokenSource = new CancellationTokenSource();
        waitHandles = new WaitHandle[2];
        isThereAnyTasksInQueue = new ManualResetEvent(false);
        newTaskIsAwaiting = new AutoResetEvent(false);
        waitHandles[0] = isThereAnyTasksInQueue;
        waitHandles[1] = newTaskIsAwaiting;
        synchronizationObject = new Object();
        IsTerminated = false;
        Start();
    }

    private void Start()
    {
        for (var i = 0; i < numberOfThreads; ++i)
        {
            threads[i] = new Thread(() =>
            {
                while (!cancellationTokenSource.IsCancellationRequested)
                {
                    WaitHandle.WaitAny(waitHandles);

                    lock (tasks)
                    {
                        if (tasks.Count > 0)
                        {
                            isThereAnyTasksInQueue.Set();
                        }
                        else
                        {
                            isThereAnyTasksInQueue.Reset();
                        }
                    }

                    if (tasks.Count > 0)
                    {
                        var isavailable = tasks.TryDequeue(out var action);
                        if (isavailable)
                        {
                            Interlocked.Increment(ref workingThreads);
                            action?.Invoke();
                            Interlocked.Decrement(ref workingThreads);
                        }
                    }

                    lock (tasks)
                    {
                        if (tasks.Count > 0)
                        {
                            isThereAnyTasksInQueue.Set();
                        }
                        else
                        {
                            isThereAnyTasksInQueue.Reset();
                        }
                    }
                }
            }
            );

            threads[i].Start();
        }
    }

    public void ShutDown()
    {
        if (!IsTerminated)
        {
            cancellationTokenSource.Cancel();
            foreach (var thread in threads)
            {
                thread.Join();
            }
            IsTerminated = true;
        }
    }

    public IMyTask<T1> AddTask<T1>(Func<T1> function, ManualResetEvent? manualResetEventIsUpperTaskCompleted = null)
    {
        if (!cancellationTokenSource.IsCancellationRequested)
        {
            lock (synchronizationObject)
            {
                MyTask<T1> newTask;
                newTask = new MyTask<T1>(function, this,
                    cancellationTokenSource.Token,
                    manualResetEventIsUpperTaskCompleted);
                tasks.Enqueue(() => newTask.Performe());
                newTaskIsAwaiting.Set();
                return newTask;
            }
        } else
        {
            throw new InvalidOperationException();
        }
        
    }
}