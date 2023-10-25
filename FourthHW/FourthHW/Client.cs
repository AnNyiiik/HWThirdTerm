namespace FourthHW;

public class Client
{
    private Server? currentServer;
    private int port;

    public bool GetServer(Server server, int port)
    {
        var returnedPort = server.GetPort(port);
        if (returnedPort != -1)
        {
            currentServer = server;
            this.port = port;
        }
        return returnedPort != -1;
    }

    public void LeaveServer()
    {
        currentServer?.ReturnPort(port);
        currentServer = null;
    }

    public (int size, List<(string, bool)>?) List(Server server, int port, string path)
    {
        if (currentServer == null)
        {
            currentServer = server;
            this.port = port;
        }
        if (server != currentServer && port != this.port)
        {
            LeaveServer();
            GetServer(server, port);
        }

        if (currentServer != null && port != -1)
        {
            return currentServer.List(path);
        } else
        {
            throw new ArgumentException();
        }
    }

    public (long size, byte[]? bytes) Get(Server server, int port, string path)
    {
        if (currentServer == null)
        {
            currentServer = server;
            this.port = port;
        }
        if (server != currentServer && port != this.port)
        {
            LeaveServer();
            GetServer(server, port);
        }

        if (currentServer != null && port != -1)
        {
            return currentServer.Get(path);
        }
        else
        {
            throw new ArgumentException();
        }
    }
}