using System.Threading;
using ThirdHW;
namespace ThirdHW.Tests;

public class Tests
{
    private MyThreadPool myThreadPool;
    private ManualResetEvent manualResetEvent;
    private const int threadSize = 8;
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
            })
        }
        Thread.Sleep(100);
        Assert.AreEqual(threadSize, myThreadPool.WorkingThreads);
    }
}
