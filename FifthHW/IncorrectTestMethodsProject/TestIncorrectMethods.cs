namespace IncorrectTestMethodsProject;
using FifthHW;

public class TestIncorrectMethods
{
    [MyTest]
    public void TestsMethodFailed()
    {
        throw new Exception();
    }

    [MyTest]
    public void TestMethodPassed()
    {

    }

    [MyTest]
    public int TestNotVoidReturnValue()
    {
        return 0;
    }

    [MyTest]
    public void TestWithInputParameters(int parameter)
    {

    }

    [BeforeClass]
    public void BeforeClassNotStatic()
    {

    }

    [AfterClass]
    public void AfterClassNotStatic()
    {

    }

    [BeforeClass]
    public static void BeforeClassWithInputParameters(int parameter)
    {

    }

    [AfterClass]
    public static void AfterClassWithInputParameters(int parameter)
    {

    }

    [BeforeClass]
    public static int BeforeClassNotVoidReturnValue()
    {
        return 0;
    }

    [AfterClass]
    public static int AfterClassNotVoidReturnValue()
    {
        return 0;
    }

    [Before]
    public int BeforeNotVoidReturnValue()
    {
        return 0;
    }

    [After]
    public int AfterNotVoidReturnValue()
    {
        return 0;
    }

    [Before]
    public void BeforeWithInputParameters(int parameter)
    {
        
    }

    [After]
    public void AfterWithInputParameters(int parameter)
    {
        
    }
}