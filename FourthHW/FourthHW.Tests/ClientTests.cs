namespace FourthHW.Tests;

using System.Net.Sockets;
using System.Net;
using FourthHW;

public class ClientTests
{
    private CancellationTokenSource cancellationTokenSource;

    private static string pathForList = "../FourthHW.Tests/TestDirectory";
    private static string listResponse = "2 ../FourthHW.Tests/TestDirectory/TestDirectory true" +
    " ../FourthHW.Tests/TestDirectory/TestFile1.txt false" + 
    " ../FourthHW.Tests/TestDirectory/TestFile2.txt false\n";

    private static string pathForGet = "../../../TestDirectory/TestFile1.txt";
    private static byte[] fileBytes = File.ReadAllBytes("../../../TestDirectory/TestFile1.txt");
    private static string testResponse = $"{fileBytes.Length} " +
        $"{System.Text.Encoding.Default.GetString(fileBytes)}\n";

    private static string notExistingPath = "../FourthHW.Tests/TestDirectory/NotExistedDirectory";

    private static List<(string, bool)> correctResultList;

    [SetUp]
    public void Setup()
    {
        cancellationTokenSource = new CancellationTokenSource();
        correctResultList = new List<(string, bool)>()
        {
            ("../FourthHW.Tests/TestDirectory/TestDirectory", true),
            ("../FourthHW.Tests/TestDirectory/TestFile1.txt", false),
            ("../FourthHW.Tests/TestDirectory/TestFile2.txt", false)
        };
        
    }

    [Test]
    public async Task ListTest()
    {
        var client = new Client(8888);
        await Task.Run(async () => await ServerWork(8888, cancellationTokenSource.Token));
        var result = await client.List(pathForList);
        cancellationTokenSource.Cancel();
        Assert.That(result.Item2!.Count, Is.EqualTo(correctResultList.Count)); 
        var comparison = new Comparison<(string, bool)>((itemFirst, itemSecond)
            => itemFirst.Item1.CompareTo(itemSecond.Item1));
        correctResultList.Sort(comparison);
        result.Item2!.Sort(comparison);
        for (var i = 0; i < correctResultList.Count; ++i)
        {
            Assert.That(comparison(correctResultList[i], result.Item2[i]), Is.EqualTo(0));
            Assert.That(correctResultList[i].Item2 == result.Item2[i].Item2);
        }
    }

    [Test]
    public async Task GetTest()
    {
        var client = new Client(8889);
        await Task.Run(async () => await ServerWork(8889, cancellationTokenSource.Token));
        using var resultStream = new MemoryStream();
        await client.Get(pathForGet, resultStream);
        resultStream.Position = 0;
        var expectedResult = System.Text.Encoding.Default.GetString(fileBytes);
        var streamReader = new StreamReader(resultStream);
        var result = await streamReader.ReadToEndAsync();
        cancellationTokenSource.Cancel();
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    public void IvalidPathTest()
    {
        var client = new Client(8890);
        Task.Run(async () => await ServerWork(8890, cancellationTokenSource.Token));
        using var result = new MemoryStream();
        Assert.ThrowsAsync<DirectoryNotFoundException>(() => client.List(notExistingPath));
    }

    private async Task ServerWork(int port, CancellationToken cancellationToken)
    {
        var listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        while (!cancellationToken.IsCancellationRequested)
        {
            var socket = await listener.AcceptSocketAsync();
            var task = Task.Run(async () =>
            {
                var stream = new NetworkStream(socket);
                var reader = new StreamReader(stream);
                var data = await reader.ReadLineAsync();
                var response = data == null ? "-1\n" : ServerResponse(data);
                var writer = new StreamWriter(stream);
                await writer.WriteAsync(response);
                await writer.FlushAsync();
                socket.Close();
            });
        }
    }

    private static string ServerResponse(string request)
    {
        if (request == $"1 {pathForList}")
        {
            return listResponse;
        }
        else if (request == $"2 {pathForGet}")
        {
            return testResponse;
        }
        else
        {
            return "-1";
        }
    }
}