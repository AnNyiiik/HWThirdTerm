namespace FirstHW;

/// <summary>
/// This class created to multiply matrixes using single-threading.
/// </summary>
public class MatrixMultiplierSingleThreading : IMatrixMultiplier
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
            var result = new Matrix(a.GetSize.m, b.GetSize.n);
            for (var i = 0; i < a.GetSize.m; ++i)
            {
                for (var j = 0; j < b.GetSize.n; ++j)
                {
                    var sum = 0;
                    for (var k = 0; k < a.GetSize.n; ++k)
                    {
                        sum += a.GetElementByIndexes(i, k) *
                            b.GetElementByIndexes(k, j);
                    }
                    result.SetElementByIndexes(i, j, sum);
                }
            }
            return result;
        }
    }
}