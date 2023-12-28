using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace FourthHW.Tests;

public class Tests
{
    private Server server;
    private Client client;

    private List<(string, bool)> correctAnswerList;
    private byte[] correctResultGet;

    private static string pathForGet = "../../../TestDirectory/TestFile1.txt";
    private static string pathForList = "./../../../TestDirectory";

    private static string testListResponse = "2 ../FourthHW.Tests/TestDirectory/" +
        "TestDirectory true" +
    " ../FourthHW.Tests/TestDirectory/TestFile1.txt false " +
        "../FourthHW.Tests/TestDirectory/TestFile2.txt false\n";

    private static byte[] fileBytes = File
        .ReadAllBytes(pathForGet);
    private static string testGetResponse = $"{fileBytes.Length} " +
        $"{System.Text.Encoding.Default.GetString(fileBytes)}\n";
    private CancellationTokenSource tokenSource;

    [SetUp]
    public void Setup()
    {
        tokenSource = new CancellationTokenSource();
        //server = new Server();
        //client = new Client(8888);
        correctAnswerList = new List<(string, bool)>
        {
            ("./../../../TestDirectory/TestDirectory", true),
            ("./../../../TestDirectory/TestFile1.txt", false),
            ("./../../../TestDirectory/TestFile2.txt", false)
        };
        correctResultGet = File.
            ReadAllBytes(pathForGet);
    }

    [Test]
    public void ServerTestList()
    {
        
    }

    [Test]
    public async Task ClientTestList()
    {
        client = new Client(8888);
        await Task.Run(async () => await ServerWork(8888, tokenSource.Token));
        var result = await client.List(pathForList);
        tokenSource.Cancel();
        Assert.That(result.size, Is.EqualTo(correctAnswerList.Count));
        Assert.That(result.Item2, Is.EqualTo(correctAnswerList));
    }

    [Test]
    public void ServerTestGet()
    {
        
    }

    [Test]
    public async Task ClientTestGet()
    {
        client = new Client(8889);
        await Task.Run(async () => await ServerWork(8889, tokenSource.Token));
        using var resultStream = new MemoryStream();
        await client.Get(pathForGet, resultStream);
        resultStream.Position = 0;
        var expectedResult = System.Text.Encoding.Default
            .GetString(correctResultGet);
        var streamReader = new StreamReader(resultStream);
        var result = await streamReader.ReadToEndAsync();
        tokenSource.Cancel();
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    public async Task MultipleRequestsTest()
    {
        
    } 

    [Test]
    public async Task CheckArgumentExceptionTest()
    {
        client = new Client(8888);
        await Task.Run(async () => await ServerWork(8888, tokenSource.Token));
        using var resultStream = new MemoryStream();
        Assert.Throws<ArgumentException>(() => client?
            .Get("NotExistingFilePath", resultStream));
        Assert.Throws<ArgumentException>(() => client?
            .List("NotExistingDirectoryPath"));
    }

    private string Response(string command)
    {
        if (command == $"1 {pathForList}")
        {
            return testListResponse;
        }
        else if (command == $"2 {pathForGet}")
        {
            return testGetResponse;
        }
        else
        {
            return "-1";
        }
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
                var response = data == null ? "-1\n" : Response(data);
                var writer = new StreamWriter(stream);
                await writer.WriteAsync(response);
                await writer.FlushAsync();
                socket.Close();
            });
        }
    }

    
}