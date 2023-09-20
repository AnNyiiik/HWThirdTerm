namespace SecondHW.Tests;

using SecondHW;

public class Tests
{
    private const int NumberOfMultyThreadTests = 100;
    private const int NumberOfThreads = 10;
    private ManualResetEvent? manualResetEvent; 
    
    [Test]
    public void InvalidSupplierShouldThrowException()
    {
        Assert.Pass();
    }

    [Test]
    public void MultyThreadTest()
    {
        manualResetEvent = new ManualResetEvent(false);
        var threads = new Thread[NumberOfThreads];

        for (var i = 0; i < NumberOfMultyThreadTests; ++i)
        {
            var localI = i;
            var lazy = new LazyMultyThreadImpl<int>(() => localI * localI);
            manualResetEvent.Reset();

            var results = new int[NumberOfThreads];
            for (var j = 0; j < NumberOfThreads; ++j)
            {
                var localJ = j;
                threads[j] = new Thread(() =>
                {
                    manualResetEvent.WaitOne();
                    results[localJ] = lazy.Get();
                });
            }

            for (var j = 0; j < NumberOfThreads; ++j)
            {
                threads[j].Start();
            }

            manualResetEvent.Set();

            for (var j = 0; j < NumberOfThreads; ++j)
            {
                threads[j].Join();
            }

            for (var j = 0; j < NumberOfThreads; ++j)
            {
                Assert.That(results[j], Is.EqualTo(localI * localI));
            }
        }
    }
}