using System;
namespace FifthHW;

public class ClassTestElements
{
    public ClassTestElements(List<MyNUnitTestElement>? beforeClass,
                             List<MyNUnitTestElement>? before,
                             List<MyNUnitTestElement>? afterClass,
                             List<MyNUnitTestElement>? after,
                             List<MyNUnitTest>? tests,
                             object? obj = null)
	{
        this.beforeClass = beforeClass;
        this.afterClass = afterClass;
        this.before = before;
        this.after = after;
        this.tests = tests;
        testClassOutput = new TestClassOutput();
        this.obj = obj;
	}

    public List<MyNUnitTestElement>? beforeClass { get; private set; }
    public List<MyNUnitTestElement>? afterClass { get; private set; }
    public List<MyNUnitTestElement>? before { get; private set; }
    public List<MyNUnitTestElement>? after { get; private set; }
    public List<MyNUnitTest>? tests { get; private set; }
    public TestClassOutput testClassOutput { get; private set; }
    public object? obj { get; private set; }

    public class TestClassOutput
    {
        public List<MyNUnitTest.TestOutput> testsOutput { get; private set; }
        public TestClassOutput()
        {
            testsOutput = new List<MyNUnitTest.TestOutput>();
        }

        public void AddTestOutput(MyNUnitTest.TestOutput output)
        {
            testsOutput.Add(output);
        }
    }

    public void RunTestClass()
    {
        if (tests != null)
        {
            Parallel.ForEach(tests, (test) =>
            {
                if (beforeClass != null)
                {
                    foreach (var beforeElement in beforeClass)
                    {
                        beforeElement.RunMethod();
                    }
                }

                if (before != null)
                {
                    foreach (var before in before)
                    {
                        before.RunMethod(obj);
                    }
                }

                testClassOutput.AddTestOutput(test.PerformeTest());

                if (after != null)
                {
                    foreach (var after in after)
                    {
                        after.RunMethod(obj);
                    }
                }

                if (afterClass != null)
                {
                    foreach (var afterClass in afterClass)
                    {
                        afterClass.RunMethod();
                    }
                }
            });
        }
    }
}