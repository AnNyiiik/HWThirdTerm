using System.Threading;
using PracticeFirst;
// See https://aka.ms/new-console-template for more information
var threads = new Thread[5];
var availableForks = new int[10];
var synchronizationObject = new Object();
var philosophers = new Philosopher[5];

for (var i = 0; i < 10; ++i)
{
    availableForks[i] = 1;
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
        lock (synchronizationObject)
        {
            if (Volatile.Read(ref availableForks[localI * 2]) == 1 &&
            availableForks[localI * 2 + 1] == 1 && philosophers[localI].DoneThinking)
            {
                philosophers[localI].Left();
                Volatile.Write(ref availableForks[localI * 2], 0);
                philosophers[localI].Right();
                Volatile.Write(ref availableForks[localI * 2 + 1], 0);
                philosophers[localI].Eat();
            }
            else if (philosophers[localI].DoneEating)
            {
                philosophers[localI].Left();
                Volatile.Write(ref availableForks[localI * 2], 1);
                philosophers[localI].Right();
                Volatile.Write(ref availableForks[localI * 2 + 1], 1);
                philosophers[localI].Think();
            };
        }
    });
}

Console.WriteLine("Hello world");

for (var i = 0; i < 5; ++i)
{
    threads[i].Start();
}

for (var i = 0; i < 5; ++i)
{
    threads[i].Join();
}