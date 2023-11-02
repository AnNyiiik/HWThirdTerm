using System.Threading;
using PracticeFirst;
// See https://aka.ms/new-console-template for more information
var threads = new Thread[5];
var availableForks = new Object[5];
var synchronizationObject = new Object();
var philosophers = new Philosopher[5];

for (var i = 0; i < 5; ++i)
{
    availableForks[i] = new Object();
}

for (var i = 0; i < 5; ++i)
{
    philosophers[i] = new Philosopher(i);
}

for (var i = 0; i < 5; ++i)
{
    var localI = i;
    threads[localI] = new Thread(() =>
    {
        while (philosophers[localI].NumberOfEating < 5)
        {
            philosophers[localI].Think();
            lock (availableForks[localI])
            {
                lock (availableForks[(localI + 1) % 5])
                {
                    philosophers[localI].Eat();
                }
            }
        }
    });
}

for (var i = 0; i < 5; ++i)
{
    threads[i].Start();
}

for (var i = 0; i < 5; ++i)
{
    threads[i].Join();
}

Console.WriteLine("Hello world");