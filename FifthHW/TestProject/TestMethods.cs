namespace TestProjectFirst;
using FifthHW.MyNUnitAttributes;


public class TestMethods
{
    private static int flagFirst = 0;
    private static int flagSecond = 1;

    [Before]
    public void BeforeMethod()
    {
        ++flagFirst;
    }

    [After]
    public void AfterMthod()
    {
        flagSecond += 2;
    }

    [MyTest("reason for ignore", typeof(InvalidDataException))]
    public void IgnoredTestWithException()
    {
        throw new InvalidDataException();
    }

    [MyTest("reason for ignore")]
    public void IgnoredTest()
    {
        
    }

    [MyTest(null, typeof(InvalidDataException))]
    public void ExceptionPassedTest()
    {
        throw new InvalidDataException();
    }

    [MyTest(null, typeof(InvalidDataException))]
    public void ExceptionFailedTest()
    {
        throw new ArgumentNullException();
    }

    [MyTest]
    public void PassedTest()
    {
        if (flagFirst != flagSecond)
        {
            throw new Exception();
        }
    }

    [MyTest]
    public void FailedTest()
    {
        if (flagFirst != flagSecond)
        {
            throw new Exception();
        }
    }
}

