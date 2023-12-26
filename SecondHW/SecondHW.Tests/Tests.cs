namespace SecondHW.Tests;

using System;
using SecondHW;

public class Tests
{
    private const int NumberOfMultyThreadTests = 100;
    private const int NumberOfThreads = 10;
    private static int NumberOfSingleThreadExperiments = 100;
    static private ManualResetEvent? manualResetEvent;

    private static IEnumerable<TestCaseData> LazyImplementation => new TestCaseData[]
    {
        new TestCaseData(new LazyMultyThreadImplementation<int>(() => throw new Exception())),
        new TestCaseData(new LazySingleThreadImplementation<int>(() => throw new Exception()))
    };

    private static IEnumerable<TestCaseData> LazyImplementationCheckOnceCreated => new TestCaseData[]
    {
        new TestCaseData(new LazyMultyThreadImplementation<Object>(() => new Object())),
        new TestCaseData(new LazySingleThreadImplementation<Object>(() => new Object()))
    };

    [TestCaseSource(nameof(LazyImplementation))]
    public void InvalidSupplierShouldThrowException(ILazy<int> lazy)
    {
        Assert.Throws<Exception>(() => lazy.Get());
    }

    [TestCaseSource(nameof(LazyImplementationCheckOnceCreated))]
    public void FunctionShouldBeCalledOnce(ILazy<Object> lazy)
    {
        var first = lazy.Get();
        var second = lazy.Get();
        Assert.That(first ==second);
    }

    [Test]
    public void SingleThreadTest()
    {
        for (var i = 0; i < NumberOfSingleThreadExperiments; ++i)
        {
            var lazy = new LazySingleThreadImplementation<int>(() => i * i);
            Assert.That(lazy.Get() == i * i);
        }
    }

    [Test]
    public void MultyThreadTest()
    {
        manualResetEvent = new ManualResetEvent(false);
        var threads = new Thread[NumberOfThreads];

        for (var i = 0; i < NumberOfMultyThreadTests; ++i)
        {
            var localI = i;
            var lazy = new LazyMultyThreadImplementation<int>(() => localI * localI);
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
        manualResetEvent.Reset();
    }
}