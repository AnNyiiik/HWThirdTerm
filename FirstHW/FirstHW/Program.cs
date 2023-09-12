using FirstHW;
using System.Diagnostics;

//experiment for 10 * 10 matrixes
Console.WriteLine("Set an experiment with 10*10 matrixes");
var a = Experiments.GenerateMatrix(10, 10);
var b = Experiments.GenerateMatrix(10, 10);
var resultsMultyThreading = Experiments.
    SetExperiment(a, b, new MatrixMultiplierMultyThreading());
var resultsSingleThreading = Experiments.
    SetExperiment(a, b, new MatrixMultiplierMultyThreading());
Console.WriteLine("The expected value of multythread-method's results is {0} milliseconds",
    Experiments.CountExpectedValue(resultsMultyThreading));
Console.WriteLine("The expected value of singlethread-method's results is {0} milliseconds",
    Experiments.CountExpectedValue(resultsSingleThreading));

// experiment for 100 * 100 matrixes
Console.WriteLine("Set an experiment with 100*100 matrixes");
a = Experiments.GenerateMatrix(100, 100);
b = Experiments.GenerateMatrix(100, 100);
resultsMultyThreading = Experiments.
    SetExperiment(a, b, new MatrixMultiplierMultyThreading());
resultsSingleThreading = Experiments.
    SetExperiment(a, b, new MatrixMultiplierMultyThreading());
Console.WriteLine("The expected value of multythread-method's results is {0} milliseconds",
    Experiments.CountExpectedValue(resultsMultyThreading));
Console.WriteLine("The expected value of singlethread-method's results is {0} milliseconds",
    Experiments.CountExpectedValue(resultsSingleThreading));

// experiment for 1000 * 1000 matrixes
Console.WriteLine("Set an experiment with 1000*1000 matrixes");
a = Experiments.GenerateMatrix(1000, 1000);
b = Experiments.GenerateMatrix(1000, 1000);
resultsMultyThreading = Experiments.
    SetExperiment(a, b, new MatrixMultiplierMultyThreading());
resultsSingleThreading = Experiments.
    SetExperiment(a, b, new MatrixMultiplierMultyThreading());
Console.WriteLine("The expected value of multythread-method's results is {0} milliseconds",
    Experiments.CountExpectedValue(resultsMultyThreading));
Console.WriteLine("The expected value of singlethread-method's results is {0} milliseconds",
    Experiments.CountExpectedValue(resultsSingleThreading));