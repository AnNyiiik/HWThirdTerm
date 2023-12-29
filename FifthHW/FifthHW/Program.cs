using System.Reflection;
using FifthHW;

if (args.Length != 1)
{
    Console.WriteLine("invalid number of input parameters");
} else
{
    try
    {
        var assemblies = Directory.EnumerateFiles(args[0], "*.dll");

        Parallel.ForEach(assemblies,
                (assembly) =>
                {
                    var results = MyNUnitTestLauncher.RunAllTests(assembly);
                    MyNUnitTestLauncher.WriteTestExecutionResults(Console.Out, results);
                });
    }
    catch (Exception e)
    {
        Console.Write(e.Message);
    }
}