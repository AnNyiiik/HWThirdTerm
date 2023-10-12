namespace FirstHW;

/// <summary>
/// This class creates matrix and gives an opportunity to change/get its elements.
/// </summary>
public class Matrix
{
    private int[,] MatrixItem;

    private (int height, int width) Size;

    public (int height, int width) GetSize { get; } 

    public Matrix(int height, int width)
    {
        MatrixItem = new int[height, width];
        Size.height = height;
        Size.width = width;
        GetSize = Size;
    }

    private static Random random = new Random();

    /// <summary>
    /// Generates matrix
    /// </summary>
    /// <param name="height">rows</param>
    /// <param name="width">columns</param>
    /// <returns>matrix</returns>
	public static Matrix GenerateWithRandomNumbers(int height, int width)
    {
        var matrix = new Matrix(height, width);
        for (var i = 0; i < height; ++i)
        {
            for (var j = 0; j < width; ++j)
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

    public void Print()
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