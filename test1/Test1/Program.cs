namespace Test1;
public class Program
{
    public async static Task Main(string[] args)
    {
        if (args.Length == 1)
        {
            var isNumber = int.TryParse(args[0], out var port);
            if (!isNumber)
            {
                Console.WriteLine("Invalid data params");
                return;
            }

            await ChatNetwork.RunServer(port, Console.In, Console.Out);
        }
        else if (args.Length == 2)
        {
            var isNumber = int.TryParse(args[1], out var port);
            if (!isNumber)
            {
                Console.WriteLine("Invalid data params");
                return;
            }

            await ChatNetwork.RunClient(port, args[0], Console.In, Console.Out);
        }
        else
        {
            Console.WriteLine("Invalid number of params");
            return;
        }
    }
}