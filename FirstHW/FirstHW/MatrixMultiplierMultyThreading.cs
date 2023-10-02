namespace FirstHW;

/// <summary>
/// This class created to multiply matrixes using multy-threading.
/// </summary>
public class MatrixMultiplierMultyThreading : IMatrixMultiplier
{
	public Matrix Multiply(Matrix firstMatrix, Matrix secondMatrix)
    {
        if (firstMatrix.GetSize.n != secondMatrix.GetSize.m)
        {
            throw new ArgumentException("The incorrect matrixes' sizes for " +
                "multiplication");
        }
        else
        {
            var numberOfThreads = Environment.ProcessorCount;
            var threads = new Thread[numberOfThreads];
            var chunkSize = (firstMatrix.GetSize.m / numberOfThreads) + 1;
            var result = new Matrix(firstMatrix.GetSize.m, secondMatrix.GetSize.n);
            for (var i = 0; i < numberOfThreads; ++i)
            {
                var localI = i;
                threads[i] = new Thread(() =>
                {
                    for (var j = localI * chunkSize; j < (localI + 1) * chunkSize
                    && j < firstMatrix.GetSize.m; ++j)
                    {
                        for (var k = 0; k < secondMatrix.GetSize.n; ++k)
                        {
                            var sum = 0;
                            for (var t = 0; t < firstMatrix.GetSize.n; ++t)
                            {
                                sum += firstMatrix.GetElementByIndexes(j, t)
                                    * secondMatrix.GetElementByIndexes(t, k);
                            }
                            result.SetElementByIndexes(j, k, sum);
                        }
                    }
                });
            }
            foreach (var thread in threads)
            {
                thread.Start();
            }
            foreach (var thread in threads)
            {
                thread.Join();
            }
            return result;
        }
    }
}