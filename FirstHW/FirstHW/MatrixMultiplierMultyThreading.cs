namespace FirstHW;

/// <summary>
/// This class created to multiply matrixes using multy-threading.
/// </summary>
public class MatrixMultiplierMultyThreading : IMatrixMultiplier
{
	public Matrix Multiply(Matrix a, Matrix b)
    {
        if (a.GetSize.n != b.GetSize.m)
        {
            throw new ArgumentException("The incorrect matrixes' sizes for " +
                "multiplication");
        }
        else
        {
            var threads = new Thread[a.GetSize.m];
            var result = new Matrix(a.GetSize.m, b.GetSize.n);
            for (var i = 0; i < a.GetSize.m; ++i)
            {
                var localI = i;
                threads[localI] = new Thread(() =>
                {
                    for (var t = 0; t < b.GetSize.n; ++t)
                    {
                        var sum = 0;
                        for (var k = 0; k < a.GetSize.n; ++k)
                        {
                            sum += a.GetElementByIndexes(localI, k) * b.GetElementByIndexes(k, t);
                        }
                        result.SetElementByIndexes(localI, t, sum);
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