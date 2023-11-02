namespace Test1;
public class Program
{
    public async static Task Main(string[] args)
    {
        Console.WriteLine("enter port to run as server, enter port and IP to run as" +
            "client");
        if (args.Length == 1)
        {
            var isNumber = int.TryParse(args[0], out var port);
            if (!isNumber)
            {
                Console.WriteLine("Invalid data params");
                return;
            }

            await ChatServer.RunServer(port, Console.In, Console.Out);
        } else if (args.Length == 2)
        {
            var isNumber = int.TryParse(args[1], out var port);
            if (!isNumber)
            {
                Console.WriteLine("Invalid data params");
                return;
            }

            await ChatClient.RunClient(port, args[0], Console.In, Console.Out);
        } else
        {
            Console.WriteLine("Invalid number of params");
            return;
        }
    }
}