using FirstHW;

namespace MatrixMultiplication.Tests;

public class Tests
{
    private static IEnumerable<TestCaseData> TestingMultiplicationData
         => new TestCaseData[]
         {
             new TestCaseData(new MatrixMultiplierMultyThreading(), "..//..//.." +
                 "//SimpleCaseFirstMatrix.txt", "..//..//..//SimpleCaseSecond" +
                 "Matrix.txt", "..//..//..//SimpleCaseCorrectResult.txt"),
             new TestCaseData(new MatrixMultiplierSingleThreading(), "..//..//.." +
                 "//SimpleCaseFirstMatrix.txt", "..//..//..//SimpleCaseSecond" +
                 "Matrix.txt", "..//..//..//SimpleCaseCorrectResult.txt"),
             new TestCaseData(new MatrixMultiplierMultyThreading(), "..//..//.." +
                 "//ComplexCaseFirstMatrix.txt", "..//..//..//ComplexCaseSecond" +
                 "Matrix.txt", "..//..//..//ComplexCaseCorrectResult.txt"),
             new TestCaseData(new MatrixMultiplierSingleThreading(), "..//..//.." +
                 "//ComplexCaseFirstMatrix.txt", "..//..//..//ComplexCaseSecond" +
                 "Matrix.txt", "..//..//..//ComplexCaseCorrectResult.txt")
         };

    private static IEnumerable<TestCaseData> MatrixMultipliers
         => new TestCaseData[]
         {
             new TestCaseData(new MatrixMultiplierMultyThreading()),
             new TestCaseData(new MatrixMultiplierSingleThreading())
         };

    [TestCaseSource(nameof(MatrixMultipliers))]
    public void TestCaseIncompatibleMatrixesSizesSholdThrowArgumentException(IMatrixMultiplier matrixMultiplier)
    {
        var a = new Matrix(3, 5);
        var b = new Matrix(6, 2);
        Assert.Throws<ArgumentException>(() => matrixMultiplier.Multiply(a, b));
    }

    [Test]
    public void TestCaseNotExistingFileShouldThrowFileNotFoundException()
    {
        Assert.Throws<FileNotFoundException>(() =>
        MatrixReader.ReadMatrixesFromFile("..//..//..//NotExistingFile.txt"));
        Assert.Throws<FileNotFoundException>(() =>
        MatrixWriter.WriteMatrixToFile("..//..//..//NotExistingFile.txt", new Matrix(2, 2)));
    }

    [TestCaseSource(nameof(TestingMultiplicationData))]
    public void TestCaseMultiplication(IMatrixMultiplier matrixMultiplier,
        string pathToFirstMatrix, string pathToSecondMatrix, string pathToAnswer)
    {
        var a = MatrixReader.ReadMatrixesFromFile(pathToFirstMatrix);
        var b = MatrixReader.ReadMatrixesFromFile(pathToSecondMatrix);
        var result = matrixMultiplier.Multiply(a, b);
        using (StreamReader reader = File.OpenText(pathToAnswer))
        {
            var line = reader.ReadLine()?.Split();
            if (line != null)
            {
                Assert.True(Int32.Parse(line[0]) == result.GetSize.m &&
                Int32.Parse(line[1]) == result.GetSize.n);
                var n = result.GetSize.n;
                var m = result.GetSize.m;
                for (var i = 0; i < m; ++i)
                {
                    line = reader.ReadLine()?.Split();
                    for (var j = 0; j < n; ++j)
                    {
                        if (line != null)
                        {
                            Assert.That(Int32.Parse(line[j]), Is.EqualTo(result.GetElementByIndexes(i, j)));
                        }
                        else { Assert.True(false); }
                    }
                }
            }
            else { Assert.True(false); }
        }
    }
}