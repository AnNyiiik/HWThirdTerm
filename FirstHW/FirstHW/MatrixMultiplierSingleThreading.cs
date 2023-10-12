namespace FirstHW;

/// <summary>
/// This class is created to multiply matrixes using single-threading.
/// </summary>
public class MatrixMultiplierSingleThreading : IMatrixMultiplier
{
	public Matrix Multiply(Matrix a, Matrix b)
	{
        if (a.GetSize.width != b.GetSize.height)
        {
            throw new ArgumentException("The incorrect matrixes' sizes for " +
                "multiplication");
        }
        else
        {
            var result = new Matrix(a.GetSize.height, b.GetSize.width);
            for (var i = 0; i < a.GetSize.height; ++i)
            {
                for (var j = 0; j < b.GetSize.width; ++j)
                {
                    var sum = 0;
                    for (var k = 0; k < a.GetSize.width; ++k)
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