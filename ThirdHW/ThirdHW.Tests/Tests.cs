namespace ThirdHW.Tests;

public class Tests
{
    private MyThreadPool myThreadPool;
    private ManualResetEvent manualResetEvent;
    private int threadSize = 8;

    [SetUp]
    public void Setup()
    {
        myThreadPool = new MyThreadPool(threadSize);
        manualResetEvent = new ManualResetEvent(false);
    }

    [Test]
    public void AreAllTheThreadsWorkingTest()
    {
        for (var i = 0; i < threadSize; ++i)
        {
            myThreadPool.AddTask(() =>
            {
                manualResetEvent.WaitOne();
                return 0;
            });
        }
        Thread.Sleep(1000);
        Assert.That(myThreadPool.WorkingThreads, Is.EqualTo(threadSize));
        manualResetEvent.Set();
        Thread.Sleep(100);
        Assert.That(myThreadPool.WorkingThreads, Is.EqualTo(0));
    }

    [Test]
    public void CheckFullLoadCase()
    {
        var tasks = new IMyTask<int>[threadSize * 5];
        var correctResults = new int[threadSize * 5];
        for (var i = 0; i < tasks.Length; ++i)
        {
            var localI = i;
            tasks[i] = myThreadPool.AddTask<int>(() =>
            {
                return localI * localI;
            });
            correctResults[i] = i * i;
        }
        Thread.Sleep(1000);
        for (var i = 0; i < tasks.Length; ++i)
        {
            Assert.That(correctResults[i], Is.EqualTo(tasks[i].Result));
        }
    }

    [Test]
    public void ContinuationsCheck()
    {
        var tasks = new IMyTask<int>[threadSize];
        var continuations = new IMyTask<int>[threadSize];
        var correctResults = new int[threadSize];
        for (var i = 0; i < tasks.Length; ++i)
        {
            var localI = i;
            tasks[i] = myThreadPool.AddTask<int>(() =>
            {
                manualResetEvent.WaitOne();
                return localI;
            });
            continuations[i] = tasks[i].ContinueWith<int>(x => x + 1);
            correctResults[i] = i + 1;
        }
        manualResetEvent.Set();
        Thread.Sleep(100);
        for (var i = 0; i < tasks.Length; ++i)
        {
            Assert.That(correctResults[i], Is.EqualTo(continuations[i].Result));
        }
    }

    [Test]
    public void ShutDownCheck()
    {
        var tasks = new IMyTask<int>[threadSize];
        var iterations = Int32.MaxValue;
        for (var i = 0; i < tasks.Length; ++i)
        {
            tasks[i] = myThreadPool.AddTask<int>(() =>
            {
                var sum = 0;
                for (var i = 0; i < iterations; ++i)
                {
                    ++sum;
                }
                return 0;
            });
        }
        myThreadPool.ShutDown();
        Assert.That(myThreadPool.WorkingThreads, Is.EqualTo(0));
        Assert.Throws<InvalidOperationException>(() => myThreadPool
            .AddTask<int>(() => 0));
    }

    [Test]
    public void ResultCheck()
    {

    }

    [Test]
    public void IsCompletedCheck()
    {

    }

    [Test]
    public void ConcurrentAccessCheck()
    {

    }

    [Test]
    public void MultipleContinuationCheck()
    {

    }
}