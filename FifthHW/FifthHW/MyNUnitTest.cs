using System.Reflection;
namespace FifthHW;

public class MyNUnitTest
{

	private readonly bool isIgnored;
	private readonly string whyIsIgnored;
	private readonly Type? typeOfExpectedException;
	private readonly MethodInfo methodInfo;
	public readonly Object classObject;

	public MyNUnitTest(MethodInfo methodInfo, Object obj)
	{
		this.methodInfo = methodInfo;
		var attribute = methodInfo.GetCustomAttribute(typeof(MyTestAttribute));
		isIgnored = ((MyTestAttribute)attribute!).ReasonForIgnore != null;
        whyIsIgnored = isIgnored ? ((MyTestAttribute)attribute!).
			ReasonForIgnore! : string.Empty;
        typeOfExpectedException = ((MyTestAttribute)attribute!)
			.TypeOfExpectedException;
        this.classObject = obj;
    }

	public enum TestStatuses
	{
		failed,
		passed,
		ignored
	}

	public TestOutput PerformeTest()
	{
		if (isIgnored)
		{
			var testOutput = new TestOutput(TestStatuses.ignored,
				whyIsIgnored, methodInfo.Name);
		}

        try
        {
            methodInfo.Invoke(classObject, new object[] { });
            return new TestOutput(TestStatuses.passed, String.Empty,
				methodInfo.Name);
        }
        catch (Exception exception)
        {
            var exceptionType = exception.InnerException!.GetType();
            if (typeOfExpectedException != null &&
				typeOfExpectedException == exceptionType)
            {
                return new TestOutput(TestStatuses.passed, String.Empty,
					methodInfo.Name);
            }
            else
            {
                return new TestOutput(TestStatuses.failed,
					$"caught {exceptionType} exception, but " +
					$"{typeOfExpectedException} was expected",
                    methodInfo.Name);
            }
        }
    }
} 