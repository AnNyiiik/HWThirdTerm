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
    TimeSpan ts = stopWatch.Elapsed;

    for (var i = 0; i < checkSum.Length; ++i)
    {
        Console.Write(checkSum[i]);
        Console.Write(' ');
    }

    stopWatch.Start();
    checkSum = await MD5.CheckSumComputerAsync.CreateCheckSum(path!);
    stopWatch.Stop();
    ts = stopWatch.Elapsed;

    for (var i = 0; i < checkSum.Length; ++i)
    {
        Console.Write(checkSum[i]);
        Console.Write(' ');
    }
}
catch (Exception exception)
{
    Console.WriteLine(exception.Message);
}