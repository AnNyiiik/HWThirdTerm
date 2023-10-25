namespace FourthHW.Tests;

public class Tests
{
    private Server server;
    private Client client;
    private List<(string, bool)> correctAnswerList;
    private int correctSizeList;
    private byte[] correctResultGet;
    private int correctSizeGet;
    private string pathForGet = "./../../../TestDirectory/TestFile1.txt";
    private string pathToTestDirectory = "./../../../TestDirectory";
    [SetUp]
    public void Setup()
    {
        server = new Server(21, Directory.GetCurrentDirectory());
        client = new Client();
        correctAnswerList = new List<(string, bool)>
        {
            ("./../../../TestDirectory/TestDirectory", true),
            ("./../../../TestDirectory/TestFile1.txt", false),
            ("./../../../TestDirectory/TestFile2.txt", false)
        };
        correctSizeList = correctAnswerList.Count;
        correctResultGet = File.
            ReadAllBytes(pathForGet);
        correctSizeGet = correctResultGet.Length;
    }

    [Test]
    public void ServerTestList()
    {
        var result = server.List(pathToTestDirectory);
        Assert.AreEqual(result.Item1, correctSizeList);
        Assert.NotNull(result.Item2);
        //foreach(var element in correctAnswerList)
        //if (result.Item2 != null)
        //{
        //    Assert.True(result.Item2.Contains(element));
        //}
    }

    [Test]
    public void ClientTestList()
    {
        var result = client.List(server, 0, pathToTestDirectory);
        Assert.AreEqual(result.Item1, correctSizeList);
        Assert.NotNull(result.Item2);
        //foreach (var element in correctAnswerList)
        //if (result.Item2 != null)
        //{
        //    Assert.True(result.Item2.Contains(element));
        //}
    }

    [Test]
    public void ServerTestGet()
    {
        var result = server.Get(pathForGet);
        Assert.That(result.Item1 == correctSizeGet);
        Assert.NotNull(result.Item2);
        Assert.AreEqual(correctResultGet, result.Item2);
    }

    [Test]
    public void ClientTestGet()
    {
        var result = client.Get(server, 0, pathForGet);
        Assert.That(result.Item1 == correctSizeGet);
        Assert.NotNull(result.Item2);
        Assert.AreEqual(correctResultGet, result.Item2);
    }
}