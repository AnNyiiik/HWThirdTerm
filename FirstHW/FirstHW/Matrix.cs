namespace FirstHW;

/// <summary>
/// This class creates matrix and gives an opportunity to change/get its elements.
/// </summary>
public class Matrix
{
    private int[,] MatrixItem;

    private (int height, int width) Size;

    public (int m, int n) GetSize { get => Size; } 

    public Matrix(int m, int n)
    {
        MatrixItem = new int[m, n];
        Size.height = m;
        Size.width = n;
    }

    private static Random random = new Random();

    /// <summary>
    /// Generates matrix
    /// </summary>
    /// <param name="m">rows</param>
    /// <param name="n">columns</param>
    /// <returns>matrix</returns>
	public static Matrix GenerateMatrixWithRandomNumbers(int m, int n)
    {
        var matrix = new Matrix(m, n);
        for (var i = 0; i < m; ++i)
        {
            for (var j = 0; j < n; ++j)
            {
                matrix.SetElementByIndexes(i, j, random.Next(-100, 100));
            }
        }
        return matrix;
    }



    public void SetElementByIndexes(int i, int j, int value)
    {
        if (i < 0 || j < 0)
        {
            throw new ArgumentException("incorrect matrix's size parametrs: " +
                "shold be positive integers");
        }
        MatrixItem[i, j] = value;
    }

    public int GetElementByIndexes(int i, int j)
    {
        if (i < 0 || i >= Size.height || j < 0 || j >= Size.width)
        {
            throw new ArgumentException("incorrect indexes");
        }
        return MatrixItem[i, j];
    }

    public void PrintMatrix()
    {
        for (var i = 0; i < Size.height; ++i)
        {
            for (var j = 0; i < Size.height; ++i)
            {
                Console.Write(MatrixItem[i, j] + " ");
            }
            Console.Write("\n");
        }
    }
}