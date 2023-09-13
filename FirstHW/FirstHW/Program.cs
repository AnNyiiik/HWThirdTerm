using FirstHW;
using ConsoleTables;
using System.Drawing;

int[] sizesOfMatrixes = { 10, 50, 100, 1000 };

//experiments for matrixes
var table = new ConsoleTable("Value",
    "Number Of Elements", "MultyThread", "SingleThread");

foreach (var size in sizesOfMatrixes)
{
    Console.WriteLine("Set an experiment with {0}*{0} matrixes", size);

    var a = Experiments.GenerateMatrix(size, size);
    var b = Experiments.GenerateMatrix(size, size);
    var resultsMultyThreading = Experiments.
        SetExperiment(a, b, new MatrixMultiplierMultyThreading());
    var resultsSingleThreading = Experiments.
        SetExperiment(a, b, new MatrixMultiplierMultyThreading());

    var expectedValueOfMultyThreading = Experiments.
        CountExpectedValue(resultsMultyThreading, 3);
    var expectedValueOfSingleThreading = Experiments.
        CountExpectedValue(resultsSingleThreading, 3);

    Console.WriteLine("The expected value of multythread-method's results is " +
        "{0} milliseconds", expectedValueOfMultyThreading);
    Console.WriteLine("The expected value of singlethread-method's results is {0} " +
        "milliseconds", expectedValueOfSingleThreading);

    var varianceOfMultyThreading = Experiments.CountVariance(resultsMultyThreading,
        3, expectedValueOfMultyThreading);
    var varianceOfSingleThreading = Experiments.CountVariance(resultsSingleThreading,
        3, expectedValueOfSingleThreading);

    table.AddRow("expected value", size.ToString(), expectedValueOfMultyThreading,
        expectedValueOfSingleThreading);
    table.AddRow("variance", size.ToString(), varianceOfMultyThreading, varianceOfSingleThreading);

    Console.WriteLine("The variance of multythread-method's results is " +
        "{0} milliseconds", varianceOfMultyThreading);
    Console.WriteLine("The variance of singlethread-method's results is " +
        "{0} milliseconds", varianceOfSingleThreading);
}
table.Write();

//find such data sizes at which the differences in the speed of work are significant
var matrixSize = 2;
var difference = 0.0;
while (Math.Abs(difference) < 100)
{
    var a = Experiments.GenerateMatrix(matrixSize, matrixSize);
    var b = Experiments.GenerateMatrix(matrixSize, matrixSize);
    var resultsMultyThreading = Experiments.
        SetExperiment(a, b, new MatrixMultiplierMultyThreading());
    var resultsSingleThreading = Experiments.
        SetExperiment(a, b, new MatrixMultiplierMultyThreading());
    difference = Experiments.CountExpectedValue(resultsMultyThreading) -
        Experiments.CountExpectedValue(resultsSingleThreading);
    matrixSize *= 2;
}
Console.WriteLine("The size is {0}", matrixSize * matrixSize);