namespace Test1.Tests;

public class Tests
{
    [Test]
    public void CasualCaseTest()
    {
        var serverText = File.OpenRead("server.txt");
        var clientText = File.OpenRead("client.txt");
        var output = File.OpenWrite("result.txt");
        var serverReader = new StreamReader(serverText);
        var serverWriter = new StreamWriter(output);
        Task.Run(() => ChatNetwork.RunServer(8082, serverReader, serverWriter));
        var clientReader = new StreamReader(clientText);
        var clientWriter = new StreamWriter(output);
        var result = ChatNetwork.RunClient(8082, "localhost", clientReader, clientWriter);
        result.Wait();
    }
}
