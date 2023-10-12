namespace FirstHW;

/// <summary>
/// This class is created to write a matrix to file.
/// </summary>
public static class MatrixWriter
{
    public static void WriteMatrixToFile(string filePath, Matrix matrix)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("file doesn't exist");
        }

        using var writer = new StreamWriter(filePath);
        string line = matrix.GetSize.height.ToString() + " " + matrix.GetSize.width.ToString() + "\n";
        writer.Write(line);
        for (var i = 0; i < matrix.GetSize.height; ++i)
        {
            for (var j = 0; j < matrix.GetSize.width; ++j)
            {
                writer.Write(matrix.GetElementByIndexes(i, j).ToString() + " ");
            }
            writer.Write("\n");
        }
    }
}