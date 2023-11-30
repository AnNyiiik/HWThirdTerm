using System.Diagnostics;

var stopWatch = new Stopwatch();

Console.WriteLine("Enter path to directory/file");
string? path = Console.ReadLine();
if (!Directory.Exists(path) && !File.Exists(path))
{
    Console.Write("directory not found");
}
try
{
    stopWatch.Start();
    var checkSum = MD5.CheckSumComputer.CreateCheckSum(path!);
    stopWatch.Stop();
    var timeSpanSingleThread = stopWatch.Elapsed;

    Console.WriteLine("Check sum single thread:");
    for (var i = 0; i < checkSum.Length; ++i)
    {
        Console.Write(checkSum[i]);
        Console.Write(' ');
    }
    Console.WriteLine();

    stopWatch.Start();
    checkSum = await MD5.CheckSumComputerAsync.CreateCheckSum(path!);
    stopWatch.Stop();
    var timeSpanMultyThread = stopWatch.Elapsed;
    Console.WriteLine("Check sum multy thread:");
    for (var i = 0; i < checkSum.Length; ++i)
    {
        Console.Write(checkSum[i]);
        Console.Write(' ');
    }
    Console.WriteLine();
    var difference = timeSpanMultyThread.CompareTo(timeSpanSingleThread);
    if (difference < 0)
    {
        Console.WriteLine("multy thread is faster");
    } else if (difference > 0)
    {
        Console.WriteLine("multy thread is slower");
    } else
    {
        Console.WriteLine("the same speed");
    }
}
catch (Exception exception)
{
    Console.WriteLine(exception.Message);
}