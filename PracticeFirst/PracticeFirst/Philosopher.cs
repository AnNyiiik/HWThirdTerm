using System;
using System.Threading;
namespace PracticeFirst;

public class Philosopher
{
	private bool isThinking;
	private bool isEating;
	private bool doneEating;
	private bool doneThinking;
	private int number;

	public bool IsThinking { get => isThinking; }
	public bool IsEating { get => isEating; }
	public bool DoneEating { get => doneEating; }
	public bool DoneThinking { get => doneThinking; }

    public void Eat()
	{
		doneThinking = false;
		doneEating = false;
        Volatile.Write(ref isEating, true);
        Console.WriteLine("Currently the philosopher {0} is eating", number);
		Thread.Sleep(1000);
		isEating = false;
		Volatile.Write(ref doneEating, true);
    }

	public void Right()
	{
		Thread.Sleep(100);
	}

	public void Left()
	{
        Thread.Sleep(100);
    }

    public void Think()
    {
		doneEating = false;
		doneThinking = false;
		Volatile.Write(ref isThinking, true);
        Console.WriteLine("Currently the philosopher {0} is thinking", number);
        Thread.Sleep(1000);
		isThinking = false;
        Volatile.Write(ref doneThinking, true);
    }

    public Philosopher(int number)
	{
		this.number = number;
	}
}

