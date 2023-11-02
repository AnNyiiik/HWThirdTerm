using System.Net.Sockets;

namespace Test1;
public static class ChatClient
{
	public static async Task RunClient(int port, string IP, TextReader reader, TextWriter writer)
	{
		using (var client = new TcpClient(IP, port))
		{
            using var stream = client.GetStream();
            using var serverWriter = new StreamWriter(stream);
            using var serverReader = new StreamReader(stream);

            string? response;
            while (true)
            {
                response = reader.ReadLine();

                await serverWriter.WriteLineAsync(response);
                await serverWriter.FlushAsync();

                if (response == "exit")
                {
                    break;
                }

                await writer.WriteLineAsync($"Sent to server: {response}");
                var recieved = await serverReader.ReadLineAsync();
                await writer.WriteLineAsync($"Received from server: {recieved}");

                if (recieved == "exit")
                {
                    break;
                }
            }
        }
    }
}


