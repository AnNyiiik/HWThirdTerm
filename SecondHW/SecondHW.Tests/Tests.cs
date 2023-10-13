﻿namespace SecondHW.Tests;

using System;
using SecondHW;

public class Tests
{
    private const int NumberOfMultyThreadTests = 100;
    private const int NumberOfThreads = 10;
    private static int NumberOfSingleThreadExperiments = 100;
    private ManualResetEvent? manualResetEvent;
    private static int delimetr = 0;

    private static IEnumerable<TestCaseData> LazyImpl => new TestCaseData[]
    {
        new TestCaseData(new LazyMultyThreadImpl<double>(() => 1 / delimetr)),
        new TestCaseData(new LazySingleThreadImpl<double>(() => 1 / delimetr)),
    };

    [TestCaseSource(nameof(LazyImpl))]
    public void InvalidSupplierShouldThrowException(ILazy<double> lazy)
    {
        Assert.Throws<Exception>(() => lazy.Get());
    }

    [Test]
    public void SingleThreadTest()
    {
        for (var i = 0; i < NumberOfSingleThreadExperiments; ++i)
        {
            var lazy = new LazyMultyThreadImpl<int>(() => i * i);
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