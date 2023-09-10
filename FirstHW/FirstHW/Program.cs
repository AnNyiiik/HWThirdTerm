using FirstHW;

(Matrix a, Matrix b) = MatrixReader.ReadMatrixesFromFile("/Users/annnikolaeff" +
    "/HWThirdTerm/FirstHW/FirstHW/Matrixes.txt");
var result = MatrixMultiplier.MultiplyBySingleThreading(a, b);
MatrixWriter.WriteMatrixToFile("/Users/annnikolaeff/HWThirdTerm/FirstHW" +
    "/FirstHW/Result.cs", result);