using System.Collections.Concurrent;
using System.Reflection;

namespace FifthHW;

public class MyNUnitTestLauncher
{

    private static ConcurrentBag<TestClassOutput> testClassOutputs =
        new ConcurrentBag<TestClassOutput>();

    /// <summary>
    /// launches all the tests in the given assemply .dll path, performes all before and after elements.
    /// </summary>
    /// <param name="assembly">Path to assemply to test</param>
    /// <returns>cref="TestClassOutput"</returns>
    public static List<TestClassOutput> RunAllTests(string assembly)
	{
        var classes = Assembly.LoadFrom(assembly).ExportedTypes
                            .Where(t => t.IsClass);
        Parallel.ForEach(classes, (classItem) =>
        {
            var constructorInfo = classItem.GetConstructor(Type.EmptyTypes);
            var methods = classItem.GetTypeInfo().DeclaredMethods;

            var beforeClass = ExtractTestAndIncorrectTestElements(methods,
                typeof(BeforeClassAttribute));
            var before = ExtractTestAndIncorrectTestElements(methods,
                typeof(BeforeAttribute));
            var after = ExtractTestAndIncorrectTestElements(methods,
                typeof(AfterAttribute));
            var afterClass = ExtractTestAndIncorrectTestElements(methods,
                typeof(AfterClassAttribute));
            var tests = ExtractTestsAndIncorrectTestNames(methods,
                constructorInfo);

            foreach (var methodBeforeClass in beforeClass.Item1)
            {
                methodBeforeClass.RunMethod(null);
            }

            var testResults = new ConcurrentBag<TestOutput>();
            Parallel.ForEach(tests.Item1, (test) =>
            {
                foreach(var beforeMethod in before.Item1)
                {
                    beforeMethod.RunMethod(test.classObject);
                }
                testResults.Add(test.PerformeTest());
                foreach(var afterMethod in after.Item1)
                {
                    afterMethod.RunMethod(test.classObject);
                }
                foreach(var afterClassMethod in afterClass.Item1)
                {
                    afterClassMethod.RunMethod(null);
                }
            });

            var incorrectTestElementsNames = new List<string>();
            incorrectTestElementsNames.AddRange(beforeClass.Item2);
            incorrectTestElementsNames.AddRange(before.Item2);
            incorrectTestElementsNames.AddRange(after.Item2);
            incorrectTestElementsNames.AddRange(afterClass.Item2);
            var testClassOutput = new TestClassOutput(classItem.Name,
                incorrectTestElementsNames, tests.Item2,
                testResults.ToList());
            testClassOutputs.Add(testClassOutput);
        });
        

        return testClassOutputs.ToList();
    }

    private static List<MethodInfo>GetElementsWithAttribute(IEnumerable<MethodInfo> methods,
        Type type)
    {
        var methodsWithAppropriateType = new List<MethodInfo>();
        foreach (var method in methods)
        {
            if (method.IsDefined(type))
            {
                methodsWithAppropriateType.Add(method);
            }
        }
        return methodsWithAppropriateType;
    }

    private static (List<MyNUnitTest>, List<string>)
        ExtractTestsAndIncorrectTestNames(IEnumerable<MethodInfo> methods,
        ConstructorInfo? constructorInfo)
    {
        var testsMethods = GetElementsWithAttribute(methods, typeof(MyTestAttribute));
        var tests = new List<MyNUnitTest>();
        var incorrectTestNames = new List<string>();

        foreach(var method in testsMethods)
        {
            if (method.GetParameters().Length > 0 || method.ReturnType
                != typeof(void))
            {
                incorrectTestNames.Add(method.Name);
            } else
            {
                var test = new MyNUnitTest(method, constructorInfo!
                    .Invoke(new object[] {}));
                tests.Add(test);
            }
        }
        return (tests, incorrectTestNames);
    }

    private static (List<MyNUnitTestElement>, List<string>) ExtractTestAndIncorrectTestElements(
        IEnumerable<MethodInfo> methods, Type type)
    {
        var testElements = new List<MyNUnitTestElement>();
        var incorrectTestElementsNames = new List<string>();

        var methodsWithAppropriateType = GetElementsWithAttribute(methods, type);

        foreach(var method in methodsWithAppropriateType)
        {

            if (method.GetParameters().Length == 0
            && method.ReturnType == typeof(void))
            {
                if (type == typeof(AfterAttribute) ||
                type == typeof(BeforeAttribute))
                {
                    testElements.Add(new MyNUnitTestElement(type, method));
                }
                else 
                {
                    if (method.IsStatic && (type == typeof(AfterClassAttribute) ||
                        type == typeof(BeforeClassAttribute)))
                    {
                        testElements.Add(new MyNUnitTestElement(type, method));
                    } else
                    {
                        incorrectTestElementsNames.Add(method.Name);
                    }
                }
            }
            else
            {
                incorrectTestElementsNames.Add(method.Name);
            }
        }
        return (testElements, incorrectTestElementsNames);
    }

    private static int CountTetsByStatus(List<TestOutput> testClassOutput,
        MyNUnitTest.TestStatuses status)
    {
        var count = 0;
        foreach(var testOutput in testClassOutput)
        {
            if (testOutput.Status == status)
            {
                ++count;
            }
        }
        return count;
    }

    /// <summary>
    /// Print all the execution data.
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="testClassesOutputs"></param>
    public static void WriteTestExecutionResults(TextWriter writer,
        List<TestClassOutput> testClassesOutputs)
    {
        foreach (var testClassOutput in testClassesOutputs)
        {
            writer.WriteLine($"Class: {testClassOutput.ClassName}");
            writer.WriteLine($"Passed: {CountTetsByStatus(testClassOutput
                .TestResults, MyNUnitTest.TestStatuses.passed)}");
            writer.WriteLine($"Failed: {CountTetsByStatus(testClassOutput
                .TestResults, MyNUnitTest.TestStatuses.failed)}");
            writer.WriteLine($"Ignored: {CountTetsByStatus(testClassOutput
                .TestResults, MyNUnitTest.TestStatuses.ignored)}");
            writer.WriteLine("Test results:");
            foreach (var testOutput in testClassOutput.TestResults)
            {
                writer.WriteLine($"Test {testOutput.Name} executed with status " +
                    $"{testOutput.Name}. Message: {testOutput.Message}");
            }
            writer.WriteLine("Incorrect test methods, verify the number of " +
                "parameters and return value:");
            foreach(var name in testClassOutput.IncorrectTestNames)
            {
                writer.WriteLine(name);
            }
            writer.WriteLine("Incorrect before and after methods, verify the number of " +
                "parameters and return value:");
            foreach (var name in testClassOutput.IncorrectTestElementsNames)
            {
                writer.WriteLine(name);
            }
        }
    }
}