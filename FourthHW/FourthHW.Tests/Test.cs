namespace FourthHW.Tests;

public class Tests
{
    private Server server;
    private Client client;
    [SetUp]
    public void Setup()
    {
        server = new Server(21, Directory.GetCurrentDirectory());
        client = new Client();
    }

    [Test]
    public void ClientTestList()
    {
        var result = server.List("../../../../TestDirectory");
        Assert.That(result.Item1 == 3);
        var correctAnswer = new List<(string, bool)>();
        correctAnswer.Add(("TestDirectory", true));
        correctAnswer.Add(("TestFile1.txt", false));
        correctAnswer.Add(("TestFile2.txt", false));
        Assert.NotNull(result.Item2);
        if (result.Item2 != null)
        {
            Assert.Equals(result.Item2, correctAnswer);
        }
    }

    [Test]
    public void ServerTest()
    {

    }
}