namespace FirstHW;

public static class MatrixReader
{
    public static (Matrix firstMatrix, Matrix secondMatrix) ReadMatrixesFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("file doesn't exist");
        }

        using (StreamReader reader = File.OpenText(filePath))
        {
            var getNumbersFromString = (string line) =>
            {
                string[] items = line.Split();
                int[] numbers = new int[items.Length];
                for (var i = 0; i < items.Length; ++i)
                {
                    bool isNumber = Int32.TryParse(items[i], out numbers[i]);
                    if (!isNumber)
                    {
                        throw new ArgumentException("incorrect format: " +
                            "should be a number");
                    }
                }
                return numbers;
            };

            string? line = reader.ReadLine();

            if (line == null || line.Length == 0)
            {
                throw new ArgumentException("incorrect format");
            }

            int[] matrixSize = getNumbersFromString(line);
            if (matrixSize.Length != 4 || matrixSize[0] <= 0 ||
                 matrixSize[1] <= 0 || matrixSize[2] <= 0 || matrixSize[3] <= 0)
            {
                throw new ArgumentException("incorrect format: matrixes's sizes " +
                    "should be represented by 4 positive integers");
            }
            (Matrix firstMatrix, Matrix secondMatrix) = (new Matrix(matrixSize[0],
                matrixSize[1]), new Matrix(matrixSize[2], matrixSize[3]));
            Matrix[] matrixes = { firstMatrix, secondMatrix };
            foreach (var matrix in matrixes)
            {
                int row = 0;
                (int m, int n) = (matrix.GetSize.m, matrix.GetSize.n);
                while ((line = reader.ReadLine()) != null)
                {
                    int[] numbers = getNumbersFromString(line);
                    if (numbers.Length != n)
                    {
                        throw new ArgumentException("incorrect format: matrix's "
                            + "size and matrix's data don't coincide");
                    }
                    for (var i = 0; i < n; ++i)
                    {
                        matrix.SetElementByIndexes(row, i, numbers[i]);
                    }
                    ++row;
                    if (row == m)
                    {
                        break;
                    }
                }
            }

            return (firstMatrix, secondMatrix);

        }

    }
}