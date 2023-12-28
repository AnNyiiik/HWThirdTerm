using System.Net;
using System.Net.Sockets;
using System.Text;
namespace FourthHW;

public class Server
{
    private int port;
    private CancellationToken cancellationToken;

    public Server(int port, CancellationToken token)
    {
        this.port = port;
        this.cancellationToken = token;
    }

    public async Task LaunchServer()
    {
        var listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        var serverTasks = new List<Task>();
        while (!this.cancellationToken.IsCancellationRequested)
        {
            var socket = await listener.AcceptSocketAsync(this.cancellationToken);
            var task = Task.Run(async () =>
            {
                var stream = new NetworkStream(socket);
                var reader = new StreamReader(stream);
                var data = await reader.ReadLineAsync();
                var response = (data == null || (data[0] != '1'
                    && data[0] != '2')) ? "-1\n" : Response(data[0],
                    data.Remove(0, 2));
                var writer = new StreamWriter(stream);
                await writer.WriteAsync(response);
                await writer.FlushAsync();
                socket.Close();
            });
            serverTasks.Add(task);
        }

        await Task.WhenAll(serverTasks.ToArray());
    }


    private string Response(char option, string path)
    {
        return (option == '1') ? ListResponse(path) : GetResponse(path);
    }


    private string ListResponse(string path)
    {
        if (!Directory.Exists(path))
        {
            return "-1";
        }

        var directories = Directory.GetDirectories(path);
        var files = Directory.GetFiles(path);
        var size = 0;
        var response = new StringBuilder();

        for (var i = 0; i < directories.Length; ++i)
        {
            response.Append(directories[i].ToString() + " true");
            size++;
        }
        for (var i = 0; i < files.Length; ++i)
        {
            if (files[i].Contains('~'))
            {
                continue;
            }
            response.Append(files[i].ToString() + " false");
            size++;
        }
        return size.ToString() + response + "\n";
    }

    private string GetResponse(string path)
    {
        if (!File.Exists(path))
        {
            return "-1";
        }

        var bytes = File.ReadAllBytesAsync(path).Result;
        var length = new FileInfo(path).Length;
        return length.ToString() + " " + Encoding.Default.GetString(bytes) + "\n";
    }
}