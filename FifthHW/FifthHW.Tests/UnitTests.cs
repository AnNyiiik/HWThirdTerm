namespace FifthHW.Tests;

public class Tests
{
    private static string pathToIncorrectTestMethodsDll =
        "../../../../IncorrectTestMethodsProject/bin/Debug/net7.0/IncorrectTestMethodsProject.dll";

    private static string pathToTestProjectDll =
        "../../../../TestProject/bin/Debug/net7.0/TestProject.dll";

    [Test]
    public void IncorrectTestMethodsTest()
    {
        var output = MyNUnitTestLauncher.RunAllTests(pathToIncorrectTestMethodsDll);
        var expected = new List<TestClassOutput>();
        string[] incorrectTestNames = { "TestNotVoidReturnValue",
            "TestWithInputParameters" };
        string[] incorrectTestElementsNames = { "BeforeClassNotStatic",
        "AfterClassNotStatic", "BeforeClassWithInputParameters",
        "AfterClassWithInputParameters", "BeforeClassNotVoidReturnValue",
        "AfterClassNotVoidReturnValue", "BeforeNotVoidReturnValue",
        "AfterNotVoidReturnValue", "BeforeWithInputParameters",
        "AfterWithInputParameters"};
        var testOutputs = new List<TestOutput>();
        testOutputs.Add(new TestOutput(MyNUnitTest.TestStatuses.failed, null,
            "TestsMethodFailed"));
        testOutputs.Add(new TestOutput(MyNUnitTest.TestStatuses.passed, null,
            "TestMethodPassed"));
        expected.Add(new TestClassOutput("TestIncorrectMethods",
            incorrectTestElementsNames.ToList(), incorrectTestNames.ToList(),
            testOutputs));
        Assert.That(expected.Count, Is.EqualTo(output.Count));
        for (var i = 0; i < expected.Count; ++i) {
            Assert.True(AreEqualTestClassOutputs(expected[i], output[i])); 
        }
    }

    [Test]
    public void CasualTestCase()
    {
        var output = MyNUnitTestLauncher.RunAllTests(pathToTestProjectDll);
        var expected = new List<TestClassOutput>();
        var incorrectTestNames = new List<String>();
        var incorrectTestElementsNames = new List<String>();
        var testOutputs = new List<TestOutput>();
        testOutputs.Add(new TestOutput(MyNUnitTest.TestStatuses.ignored, null,
            "IgnoredTestWithException"));
        testOutputs.Add(new TestOutput(MyNUnitTest.TestStatuses.ignored, null,
            "IgnoredTest"));
        testOutputs.Add(new TestOutput(MyNUnitTest.TestStatuses.passed, null,
            "ExceptionPassedTest"));
        testOutputs.Add(new TestOutput(MyNUnitTest.TestStatuses.failed, null,
            "ExceptionFailedTest"));
        testOutputs.Add(new TestOutput(MyNUnitTest.TestStatuses.passed, null,
            "PassedTest"));
        testOutputs.Add(new TestOutput(MyNUnitTest.TestStatuses.failed, null,
            "FailedTest"));
        expected.Add(new TestClassOutput("TestMethods",
            incorrectTestElementsNames, incorrectTestNames,
            testOutputs));
        Assert.That(expected.Count, Is.EqualTo(output.Count));
        for (var i = 0; i < expected.Count; ++i)
        {
            Assert.True(AreEqualTestClassOutputs(expected[i], output[i]));
        }
    }

    private static bool AreEqualTestOutputs(TestOutput expected,
        TestOutput actual)
    {
        return (string.Compare(expected.Name, actual.Name) == 0) &&
            (expected.Status == expected.Status);
    }

    private static bool AreEqualTestClassOutputs(TestClassOutput expected,
        TestClassOutput actual)
    {
        if (expected.ClassName != actual.ClassName)
        {
            return false;
        }
        if (expected.IncorrectTestElementsNames.Count !=
            actual.IncorrectTestElementsNames.Count)
        {
            return false;
        }
        if (expected.IncorrectTestNames.Count != actual.IncorrectTestNames.Count)
        {
            return false;
        }
        if (expected.TestResults.Count != actual.TestResults.Count)
        {
            return false;
        }
        var compare = new Comparison<TestOutput>((first, second) =>
            String.Compare(first.Name, second.Name));
        expected.TestResults.Sort(compare);
        actual.TestResults.Sort(compare);
        for(var i = 0; i < expected.TestResults.Count; ++i)
        {
            if (!AreEqualTestOutputs(expected.TestResults[i],
                actual.TestResults[i]))
            {
                return false;
            }
        }
        var stringComparator = new Comparison<String>((first, second) =>
            String.Compare(first, second));
        expected.IncorrectTestNames.Sort(stringComparator);
        actual.IncorrectTestNames.Sort(stringComparator);
        for (var i = 0; i < expected.IncorrectTestNames.Count; ++i)
        {
            if (stringComparator(expected.IncorrectTestNames[i],
                actual.IncorrectTestNames[i]) != 0)
            {
                return false;
            }
        }
        expected.IncorrectTestElementsNames.Sort(stringComparator);
        actual.IncorrectTestElementsNames.Sort(stringComparator);
        for (var i = 0; i < expected.IncorrectTestNames.Count; ++i)
        {
            if (stringComparator(expected.IncorrectTestElementsNames[i],
                actual.IncorrectTestElementsNames[i]) != 0)
            {
                return false;
            }
        }
        return true;
    }
}