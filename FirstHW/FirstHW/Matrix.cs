namespace FirstHW;

public class Matrix
{
    int[,] MatrixItem;

    private (int m, int n) Size;

    public (int m, int n) GetSize
    {
        get { return (Size.m, Size.n); }
    }

    public Matrix(int m, int n)
    {
        MatrixItem = new int[m, n];
        Size.m = m;
        Size.n = n;
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
        if (i < 0 || i >= Size.m || j < 0 || j >= Size.n)
        {
            throw new ArgumentException("incorrect indexes");
        }
        return MatrixItem[i, j];
    }

    public void PrintMatrix()
    {
        for (var i = 0; i < Size.m; ++i)
        {
            for (var j = 0; i < Size.n; ++i)
            {
                Console.Write(MatrixItem[i, j] + " ");
            }
            Console.Write("\n");
        }
    }
}