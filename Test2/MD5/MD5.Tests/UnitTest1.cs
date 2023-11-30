namespace MD5.Tests;

public class Tests
{
    [Test]
    public void TestCaseComputeDirectoryHash()
    {
        var hash1 = CheckSumComputer.CreateCheckSum("TestDirFirst");
        var hash2 = CheckSumComputer.CreateCheckSum("TestDirSecond");
        Assert.That(hash1, Is.Not.EqualTo(hash2));
    }

    [Test]
    public void TestCaseComputeDirectoryHashAsync()
    {
        var hash1 = CheckSumComputerAsync.CreateCheckSum("TestDirFirst");
        var hash2 = CheckSumComputerAsync.CreateCheckSum("TestDirSecond");
        Assert.That(hash1, Is.Not.EqualTo(hash2));
    }

    [Test]
    public void TestCaseComputeFileHash()
    {
        var hash1 = CheckSumComputer.CreateCheckSum("TestDirFirst/TextFile1.txt");
        var hash2 = CheckSumComputer.CreateCheckSum("TestDirSecond/TextFile.txt");
        Assert.That(hash1, Is.Not.EqualTo(hash2));
    }

    [Test]
    public void TestCaseComputeFileHashAsync()
    {
        var hash1 = CheckSumComputerAsync.
            CreateCheckSum("TestDirFirst/TextFile1.txt");
        var hash2 = CheckSumComputerAsync.
            CreateCheckSum("TestDirSecond/TextFile.txt");
        Assert.That(hash1.Result, Is.Not.EqualTo(hash2.Result));
    }

    [Test]
    public static void TestCaseDirNotFound()
    {
        Assert.Throws<Exception>(() =>
            CheckSumComputer.CreateCheckSum("TextFile3.txt"));
    }
}