namespace FirstHW;

public static class MatrixMultiplier
{
	public static Matrix MultiplyByMultyThreading(Matrix a, Matrix b)
	{
        if (a.GetSize.n != b.GetSize.m)
        {
            throw new ArgumentException("The incorrect matrixes' sizes for " +
                "multiplication");
        }
        else
        {
            var threads = new Thread[a.GetSize.m, b.GetSize.n];
            var result = new Matrix(a.GetSize.m, b.GetSize.n);
            for (var i = 0; i < a.GetSize.m; ++i)
            {
                for (var j = 0; j < b.GetSize.n; ++j)
                {
                    var localI = i;
                    var localJ = j;
                    threads[localI, localJ] = new Thread(() =>
                    {
                        var sum = 0;
                        for (var k = 0; k < a.GetSize.n; ++k)
                        {
                            sum += a.GetElementByIndexes(localI, k) * b.GetElementByIndexes(k, localJ);
                        }
                        result.SetElementByIndexes(localI, localJ, sum);
                    });
                }
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

    public static Matrix MultiplyBySingleThreading(Matrix a, Matrix b)
    {
        if (a.GetSize.n != b.GetSize.m)
        {
            throw new ArgumentException("The incorrect matrixes' sizes for " +
                "multiplication");
        }
        else
        {
            var result = new Matrix(a.GetSize.m, b.GetSize.n);
            for (var i = 0; i < a.GetSize.m; ++i)
            {
                for (var j = 0; j < b.GetSize.n; ++j)
                {
                    var sum = 0;
                    for (var k = 0; k < a.GetSize.n; ++k)
                    {
                        sum += a.GetElementByIndexes(i, k) * b.GetElementByIndexes(k, j);
                    }
                    result.SetElementByIndexes(i, j, sum);
                }
            }
            return result;
        }
    }
}