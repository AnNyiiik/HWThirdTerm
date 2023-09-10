using System;
namespace FirstHW;

public static class MatrixWriter
{
    public static void WriteMatrixToFile(string filePath, Matrix matrix)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("file doesn't exist");
        }

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            string line = matrix.GetSize.m.ToString() + " " + matrix.GetSize.n.ToString() + "\n";
            writer.Write(line);
            for (var i = 0; i < matrix.GetSize.m; ++i)
            {
                for (var j = 0; j < matrix.GetSize.n; ++j)
                {
                    writer.Write(matrix.GetElementByIndexes(i, j).ToString() + " ");
                }
                writer.Write("\n");
            }
        }
    }
}

