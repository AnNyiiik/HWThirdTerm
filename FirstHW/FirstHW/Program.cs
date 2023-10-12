using FirstHW;
using ConsoleTables;
using System.Drawing;

int[] sizesOfMatrixes = { 10, 50, 100};

//experiments for matrixes
var table = new ConsoleTable("Value",
    "Number Of Elements", "MultyThread", "SingleThread");

foreach (var size in sizesOfMatrixes)
{
    var a = Matrix.GenerateWithRandomNumbers(size, size);
    var b = Matrix.GenerateWithRandomNumbers(size, size);
    var resultsMultyThreading = Experiments.
        SetExperiment(a, b, new MatrixMultiplierMultyThreading());
    var resultsSingleThreading = Experiments.
        SetExperiment(a, b, new MatrixMultiplierMultyThreading());

    var expectedValueOfMultyThreading = Experiments.
        CountExpectedValue(resultsMultyThreading, 3);
    var expectedValueOfSingleThreading = Experiments.
        CountExpectedValue(resultsSingleThreading, 3);

    var varianceOfMultyThreading = Experiments.CountVariance(resultsMultyThreading,
        3, expectedValueOfMultyThreading);
    var varianceOfSingleThreading = Experiments.CountVariance(resultsSingleThreading,
        3, expectedValueOfSingleThreading);

    table.AddRow("expected value", size.ToString(), expectedValueOfMultyThreading,
        expectedValueOfSingleThreading);
    table.AddRow("variance", size.ToString(), varianceOfMultyThreading, varianceOfSingleThreading);
}
var result = table.ToString();
using var writer = new StreamWriter("ResultsOfExperiments.txt");
writer.Write(result);
Console.Write(result);

//find such data sizes at which the differences in the speed of work are significant
var matrixSize = 2;
var difference = 0.0;
while (Math.Abs(difference) < 100)
{
    var a = Matrix.GenerateWithRandomNumbers(matrixSize, matrixSize);
    var b = Matrix.GenerateWithRandomNumbers(matrixSize, matrixSize);
    var resultsMultyThreading = Experiments.
        SetExperiment(a, b, new MatrixMultiplierMultyThreading());
    var resultsSingleThreading = Experiments.
        SetExperiment(a, b, new MatrixMultiplierMultyThreading());
    difference = Experiments.CountExpectedValue(resultsMultyThreading) -
        Experiments.CountExpectedValue(resultsSingleThreading);
    matrixSize *= 2;
}
Console.WriteLine("The size is {0}", matrixSize * matrixSize);