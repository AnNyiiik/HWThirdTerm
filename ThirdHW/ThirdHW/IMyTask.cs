namespace ThirdHW;

public interface IMyTask<T1>
{
    public bool IsCompleted { get; }

    public T1 Result { get; }

    public IMyTask<T2> ContinueWith<T2>(Func<T1, T2> func);
}

