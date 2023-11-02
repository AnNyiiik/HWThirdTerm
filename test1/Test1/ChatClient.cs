using System;
using System.Net;
using System.Net.Http;
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

            while (true)
            {
                var message = reader.ReadLine();
                await serverWriter.WriteLineAsync(message);
                await serverWriter.FlushAsync();

                if (message == "exit")
                {
                    break;
                }

                await writer.WriteLineAsync($"Sent: {message}");
                var data = await serverReader.ReadLineAsync();
                await writer.WriteLineAsync($"Received: {data}");
                if (data == "exit")
                {
                    break;
                }
            }
        }

    }
}


