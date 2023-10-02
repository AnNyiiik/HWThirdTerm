namespace FirstHW;
using System.Diagnostics;

/// <summary>
/// Class is designed to set experiments with matrix multiplication.
/// </summary>
public static class Experiments
{
    private const int N = 100;
    /// <summary>
    /// Counts an expected value of set of experiment's results.
    /// </summary>
    /// <param name="resultsOfExperiments"></param>
    /// <param name="precision"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
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

    /// <summary>
    /// Counts a variance of set of experiment's results.
    /// </summary>
    /// <param name="resultsOfExperiments"></param>
    /// <param name="precision"></param>
    /// <param name="expectedValue"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Set an experiment with matrix multiplication.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="multiplier"></param>
    /// <returns></returns>
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