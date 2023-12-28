using FourthHW;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace FourthHW.Tests;

public class ServerTests
{
    private CancellationTokenSource cancellationTokenSource;

    private static string testGetRequest = "2 ../../../TestDirectory/TestFile1.txt";
    private static byte[] textFileBytes = File.ReadAllBytes("../../../TestDirectory/TestFile1.txt");
    private static string testGetExpectedResponse = $"{Encoding.Default.GetString(textFileBytes)}\n";

    private static string incorrectTestListRequest = "1 ../../../TestDirectory/TestFile1";
    private static string expectedIncorrectTestListResponse = "-1";

    [SetUp]
    public void Setup()
    {
        cancellationTokenSource = new CancellationTokenSource();
    }

    [Test]
    public async Task GetTest()
    {
        var server = new Server(8118, cancellationTokenSource.Token);
        await Task.Run(async () => await server.LaunchServer());
        var result = await ClientWork(8118, testGetRequest);
        cancellationTokenSource.Cancel();
        Assert.That(result.Remove(result.Length - 2) == testGetExpectedResponse.Remove(testGetExpectedResponse.Length - 1));
    }

    [Test]
    public async Task IncorrectPathTest()
    {
        var server = new Server(8113, cancellationTokenSource.Token);
        await Task.Run(async () => await server.LaunchServer());
        var result = await ClientWork(8113, incorrectTestListRequest);
        cancellationTokenSource.Cancel();
        Assert.That(result == expectedIncorrectTestListResponse);
    }

    [Test]
    public void MultipleClientsTest()
    {
        var server = new Server(8114, cancellationTokenSource.Token);
        Task.Run(async () => await server.LaunchServer());
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        Task.WaitAll(
            ClientWork(8114, "1 ../../../TestDirectory/TestDirectory", 3000),
            ClientWork(8114, "1 ../../../TestDirectory/TestDirectory", 3000),
            ClientWork(8114, "1 ../../../TestDirectory/TestDirectory", 3000),
            ClientWork(8114, "1 ../../../TestDirectory/TestDirectory", 3000));
        stopWatch.Stop();
        cancellationTokenSource.Cancel();
        Assert.That(stopWatch.ElapsedMilliseconds, Is.LessThan(12000));
    }

    private static async Task<string> ClientWork(int port, string request, int delay = 0)
    {
        await Task.Delay(delay);
        using var client = new TcpClient();
        await client.ConnectAsync("localhost", port);
        var stream = client.GetStream();
        var writer = new StreamWriter(stream);
        await writer.WriteAsync($"{request}\n");
        await writer.FlushAsync();
        var reader = new StreamReader(stream);
        var data = string.Empty;
        if (request[0] == '1')
        {
            data = await reader.ReadLineAsync();
        }
        else
        {
            data = await ReadGetResponse(reader);
        }

        return data!;
    }

    private static async Task<string> ReadGetResponse(StreamReader streamReader)
    {
        var currentSymbol = new char[1];
        await streamReader.ReadAsync(currentSymbol, 0, 1);
        var data = new StringBuilder();
        while (currentSymbol[0] != ' ' && currentSymbol[0] != '\n')
        {
            data.Append(currentSymbol);
            await streamReader.ReadAsync(currentSymbol, 0, 1);
        }

        var dataLength = int.Parse(data.ToString());
        if (dataLength == -1)
        {
            throw new ArgumentException();
        }

        var fileData = new char[dataLength];
        await streamReader.ReadAsync(fileData, 0, fileData.Length);
        return new string(fileData);
    }
}