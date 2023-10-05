using System.Collections.Concurrent;
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
                return newTask;
            }
        } else
        {
            throw new InvalidOperationException();
        }
        
    }

    private class MyThread
    {
        private MyThreadPool myThreadPool;

        private Thread thread;

        private Action? action;

        private bool isAvaible;

        public bool IsAvaible { get; set; }

        public MyThread(MyThreadPool myThreadPool, CancellationToken cancellationToken)
        {
            this.myThreadPool = myThreadPool;
            this.thread = new Thread(() => Start(cancellationToken));
            isAvaible = true;
            IsAvaible = isAvaible;
            thread.Start();
        }

        public void Start(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                WaitHandle.WaitAny(myThreadPool.waitHandles);
                if (isAvaible && cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                lock (myThreadPool.tasks)
                {
                    if (myThreadPool.tasks.Count > 0)
                    {
                        myThreadPool.isThereAnyTasksInQueue.Set();
                    } else
                    {
                        myThreadPool.isThereAnyTasksInQueue.Reset();
                    }
                }

                if (myThreadPool.tasks.Count > 0)
                {
                    var isTaskAvailable = myThreadPool.tasks.
                        TryDequeue(out action);
                    if (isAvaible && action != null)
                    {
                        isAvaible = false;
                        action();
                        isAvaible = true;
                        action = null;
                    }
                }

                lock (myThreadPool.tasks)
                {
                    if (myThreadPool.tasks.Count > 0)
                    {
                        myThreadPool.isThereAnyTasksInQueue.Set();
                    }
                    else
                    {
                        myThreadPool.isThereAnyTasksInQueue.Reset();
                    }
                }
            }
        }
    }
}