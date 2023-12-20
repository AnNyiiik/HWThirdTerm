using FifthHW;

if (args.Length != 1)
{
    Console.WriteLine("invalid number of input parameters");
}

try
{
    var results = MyNUnitTestLauncher.RunAllTests(args[0]);
    MyNUnitTestLauncher.WriteTestExecutionResults(Console.Out, results);
} catch (Exception e)
{
    Console.Write(e.Message);
}