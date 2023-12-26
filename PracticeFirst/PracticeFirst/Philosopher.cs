using System;
using System.Threading;
namespace PracticeFirst;

public class Philosopher
{
	private int number;

	private int numberOfEating = 0;

	public int NumberOfEating { get => numberOfEating; }

	private static Random random = new Random();

	public void Eat()
	{
		Console.WriteLine("Currently the philosopher {0} is eating", number);
		Thread.Sleep(random.Next(1000));
		++numberOfEating;
	}

    public void Think()
    {
        Console.WriteLine("Currently the philosopher {0} is thinking", number);
        Thread.Sleep(random.Next(1000));
    }

    public Philosopher(int number)
	{
		this.number = number;
	}
}