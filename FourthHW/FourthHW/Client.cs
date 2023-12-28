namespace FourthHW;

using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

public class Client
{
    private string host;
    private int port;

    public Client(int port, string host = "localhost")
    {
        this.port = port;
        this.host = host;
    }

    /// <summary>
    /// Returns the number of entries in the direcory and their
    /// names in format: size (<name: String> <isDir: Boolean>)*\n
    /// If directory doesn't exist throws DirectoryNotFoundException.
    /// </summary>
    /// <param name="path">path to the directory to list entities</param>
    /// <returns></returns>
    public async Task<(int size, List<(string, bool)>?)> List(string path)
    {
        using var client = new TcpClient();
        await client.ConnectAsync(host, port);
        var stream = client.GetStream();
        var writer = new StreamWriter(stream);
        await writer.WriteAsync($"1 {path}\n");
        await writer.FlushAsync();
        var reader = new StreamReader(stream);
        try
        {
            var data = await ReadListResponce(reader);
            return data;
        }
        catch (Exception exception)
        {
            throw Equals(exception.GetType(), typeof(ArgumentException)) ?
                new DirectoryNotFoundException() : new ServerResponseException();
        }
    }

    /// <summary>
    /// Returns the size of the file and its content. If file doesn't exist throws
    /// FileNotFoundException.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="output"></param>
    /// <returns></returns>
    public async Task Get(string path, Stream output)
    {
        using var client = new TcpClient();
        await client.ConnectAsync("localhost", port);
        var stream = client.GetStream();
        var writer = new StreamWriter(stream);
        await writer.WriteAsync($"2 {path}\n");
        await writer.FlushAsync();
        var reader = new StreamReader(stream);
        try
        {
            await ReadGetResponse(stream, output);
            output.Position = 0;
        }
        catch (Exception exception)
        {
            throw Equals(exception.GetType(), typeof(ArgumentException)) ?
                new FileNotFoundException() : new ServerResponseException();
        }
    }

    private async Task<(int, List<(string, bool)>)> ReadListResponce(StreamReader reader)
    {
        var result = new List<(string, bool)>();
        var data = await reader.ReadLineAsync();
        if (data == null)
        {
            throw new InvalidDataException();
        }

        var dataItems = data.Split();
        var isNumber = Int32.TryParse(dataItems[0], out var size);
        if (!isNumber)
        {
            throw new InvalidDataException();
        }
        if (size == -1)
        {
            throw new ArgumentException();
        }
        for (var i = 1; i < dataItems.Length - 1; i += 2)
        {
            var isFile = dataItems[i + 1].Equals("false") ? true : false;
            var isDirectory = dataItems[i + 1].Equals("true") ? true : false;
            if (!isFile && !isDirectory)
            {
                throw new ArgumentException();
            }
            if (isDirectory)
            {
                result.Add((dataItems[i], true));
            }
            else
            {
                result.Add((dataItems[i], false));
            }
        }
        return (size, result);
    }

    private async Task ReadGetResponse(Stream readStream, Stream writeStream)
    {
        var reader = new StreamReader(readStream);
        var sizeString = new StringBuilder();
        var character = new char[0];
        await reader.ReadAsync(character, 0, 1);
        while (character[0] != ' ')
        {
            sizeString.Append(character[0]);
            await reader.ReadAsync(character, 0, 1);
        }

        var size = Int32.Parse(sizeString.ToString());
        if (size == -1)
        {
            throw new ArgumentException();
        }

        var writer = new StreamWriter(writeStream);
        while (!reader.EndOfStream)
        {
            var data = new char[8];
            if (writeStream.Position + 8 > size)
            {
                var bytesRead = await reader.ReadAsync(data, 0, size -
                    (int)writeStream.Position);
                writer.Write(data, 0, bytesRead);
                writer.Flush();
                return;
            }
            else
            {
                var bytesRead = await reader.ReadAsync(data, 0, data.Length);
                writer.Write(data, 0, bytesRead);
            }

            writer.Flush();
        }
    }
}