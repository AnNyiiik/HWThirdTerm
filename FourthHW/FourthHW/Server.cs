namespace FourthHW;

public class Server
{
	
	private bool[] ports;
	private string rootDirectoryName;

	public string PathToRootDirectory { get; }

	public Server(int numberOfPorts, string pathToRootDirectory)
	{
		ports = new bool[numberOfPorts];
		this.rootDirectoryName = pathToRootDirectory;
		PathToRootDirectory = pathToRootDirectory;
	}

    private bool isDirectory(string path)
	{
		return Directory.Exists(Directory.GetDirectoryRoot(rootDirectoryName)[1..] + rootDirectoryName + path) ||
		Directory.Exists(Directory.GetDirectoryRoot(path)[1..] + path);
    }
        
    public int GetPort(int port)
    {
        if (port > ports.Length || port < 0)
        {
            throw new ArgumentException();
        } else if (ports[port])
        {
            return -1;
        }

        ports[port] = true;
        return port;
    }

    public void ReturnPort(int i)
    {
        if (i > ports.Length || i < 0)
        {
            throw new ArgumentException();
        }

        ports[i] = false;
    }


    public (int, List<(string, bool)>?) List(string path)
	{
		if (!isDirectory(path))
		{
			return (-1, null);
		}

		var directories = Directory.Exists(Directory.
			GetDirectoryRoot(path)[1..] + path)
            ? Directory.GetDirectories(Directory.
			GetDirectoryRoot(path)[1..] + path)
            : Directory.GetDirectories(Directory.
			GetDirectoryRoot(rootDirectoryName)[1..]
			+ rootDirectoryName + path);

        var files = Directory.Exists(Directory.
            GetDirectoryRoot(path)[1..] + path)
            ? Directory.GetFiles(Directory.
            GetDirectoryRoot(path)[1..] + path)
            : Directory.GetFiles(Directory.
            GetDirectoryRoot(rootDirectoryName)[1..]
            + rootDirectoryName + path);

		var list = new List<(string, bool)>();
		for (var i = 0; i < directories.Length; ++i)
		{
			list.Add((directories[i], true));
		}
        for (var i = 0; i < files.Length; ++i)
        {
            if (files[i].Contains('~'))
            {
                continue;
            }
            list.Add((files[i], false));
        }
        var size = list.Count;
		return (size, list);
    }

    public (long, byte[]?) Get(string path)
    {
        if (isDirectory(path))
        {
            return (-1, null);
        }

        if (!File.Exists(Directory.GetDirectoryRoot(rootDirectoryName)[1..]
            + rootDirectoryName + path) &&
            !File.Exists(Directory.GetDirectoryRoot(path)[1..] + path))
        {
            return (-1, null);
        }

        path = File.Exists(Directory.GetDirectoryRoot(rootDirectoryName)[1..]
            + rootDirectoryName + path) ? Directory.
            GetDirectoryRoot(rootDirectoryName)[1..] + rootDirectoryName + path
            : Directory.GetDirectoryRoot(path)[1..] + path;

        var bytes = File.ReadAllBytesAsync(path).Result;
        var length = new FileInfo(path).Length;
        return (length, bytes);
    }
}