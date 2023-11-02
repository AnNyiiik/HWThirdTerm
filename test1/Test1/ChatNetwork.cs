using System.Net;
using System.Net.Sockets;

namespace Test1;
public static class ChatNetwork
{
	public static async Task RunClient(int port, string IP, TextReader reader, TextWriter writer)
	{
		using (var client = new TcpClient(IP, port))
		{
            using var stream = client.GetStream();
            using var streamWriter = new StreamWriter(stream);
            using var streamReader = new StreamReader(stream);

            string? response;
            while (true)
            {
                response = reader.ReadLine();

                await streamWriter.WriteLineAsync(response);
                await streamWriter.FlushAsync();

                if (response != null && response.Equals("exit"))
                {
                    break;
                }

                await writer.WriteLineAsync($"Sent to port {port}: {response}");
                var recieved = await streamReader.ReadLineAsync();
                await writer.WriteLineAsync($"Received from port {port}: {recieved}");

                if (recieved != null && recieved.Equals("exit"))
                {
                    break;
                }
            }
        }
    }

    public static async Task RunServer(int port, TextReader reader, TextWriter writer)
    {
        var listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        await writer.WriteLineAsync("Chat is available now");
        var socket = await listener.AcceptSocketAsync();
        await writer.WriteLineAsync("Connected");
        await Task.Run(async () =>
        {
            var stream = new NetworkStream(socket);

            using var streamReader = new StreamReader(stream);
            using var streamWriter = new StreamWriter(stream);

            while (true)
            {
                var recieved = await streamReader.ReadLineAsync();
                if (recieved == "exit")
                {
                    break;
                }

                await writer.WriteLineAsync($"Recieved from client: {recieved}");
                var response = reader.ReadLine();
                await streamWriter.WriteLineAsync(response);
                await streamWriter.FlushAsync();
                await writer.WriteLineAsync($"Sent to client: {response}");

                if (response == "exit")
                {
                    break;
                }
            }
            socket.Close();
        });
        listener.Stop();
    }
}