namespace FirstHW;
using System.Diagnostics;

public static class Experiments
{
    private const int N = 100;

	public static Matrix GenerateMatrix(int m, int n)
	{
		var matrix = new Matrix(m, n);
        var random = new Random();
		for (var i = 0; i < m; ++i)
		{
            for (var j = 0; j < n; ++j)
            {
                matrix.SetElementByIndexes(i, j, random.Next(-100, 100));
            }
        }
        return matrix;
    }

	public static double CountExpectedValue(double[] resultsOfExperiments, int precision = 9)
    { 
        if (resultsOfExperiments.Length <= 0)
        {
            throw new ArgumentException();
        }
        var sum = 0.0;
        for (var i = 0; i < resultsOfExperiments.Length; ++i)
        {
            if (resultsOfExperiments[i] < 0)
            {
                throw new ArgumentException();
            }
            sum += resultsOfExperiments[i];
        }
        return Math.Round(sum / resultsOfExperiments.Length, precision);
    }

    public static double CountVariance(double[] resultsOfExperiments, int precision = 9, double? expectedValue = null)
    {
        var sum = 0.0;
        if (expectedValue == null)
        {
            expectedValue = CountExpectedValue(resultsOfExperiments);
        } 
        for (var i = 0; i < resultsOfExperiments.Length; ++i)
        {
            sum += Math.Pow(resultsOfExperiments[i] - (double)expectedValue, 2);
        }
        sum = sum / (resultsOfExperiments.Length * (resultsOfExperiments.Length - 1));
        return Math.Round(Math.Sqrt(sum), precision);
    }

    public static double[] SetExperiment(Matrix a, Matrix b, IMatrixMultiplier multiplier)
    {
        Stopwatch stopwatch = new Stopwatch();
        var results = new double[N];
        for (var i = 0; i < N; ++i)
        {
            stopwatch.Start();
            multiplier.Multiply(a, b);
            stopwatch.Stop();
            results[i] = stopwatch.ElapsedMilliseconds;
        }
        return results;
    }
}