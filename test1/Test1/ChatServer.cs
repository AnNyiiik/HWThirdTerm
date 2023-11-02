using System.Net;
using System.Net.Sockets;

namespace Test1;

public static class ChatServer
{
    public static async Task RunServer(int port, TextReader userReader, TextWriter userWriter)
    {
        var listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        await userWriter.WriteLineAsync("Chat is available now");
        var socket = await listener.AcceptSocketAsync();
        await userWriter.WriteLineAsync("Connected");
        using var stream = new NetworkStream(socket);

        using var reader = new StreamReader(stream);
        using var writer = new StreamWriter(stream);

        while (true)
        {
            var recieved = await reader.ReadLineAsync();
            if (recieved == "exit")
            {
                break;
            }

            await userWriter.WriteLineAsync($"Recieved from client: {recieved}");
            var response = userReader.ReadLine();
            await writer.WriteLineAsync(response);
            await writer.FlushAsync();
            await userWriter.WriteLineAsync($"Sent to client: {response}");

            if (response == "exit")
            {
                break;
            }
        }

        socket.Close();
    }
}

