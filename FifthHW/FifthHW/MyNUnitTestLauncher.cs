using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace FifthHW;

public class MyNUnitTestLauncher
{
	public class TestClassOutput
	{
		public TestClassOutput()
		{

		}
	}

	public static List<TestClassOutput> RunAllTests(string path)
	{
        var testClassesOutputs = new ConcurrentQueue<TestClassOutput>();
        var assemblies = Directory.EnumerateFiles(path, "*.dll");
        Parallel.ForEach(assemblies,
            (assembly) =>
            {
                var classes = Assembly.LoadFrom(assembly).ExportedTypes
                                   .Where(t => t.IsClass);
                Parallel.ForEach(classes, (classItem) =>
                {
                    var constructorInfo = classItem.GetConstructor(Type.EmptyTypes);
                    var methods = classItem.GetTypeInfo().DeclaredMethods;

                    var beforeClass = ExtractTestAndIncorrectTestElements(methods,
                        MyNUnitTestElement.Types.BeforeClass);
                    var before = ExtractTestAndIncorrectTestElements(methods,
                        MyNUnitTestElement.Types.Before);
                    var after = ExtractTestAndIncorrectTestElements(methods,
                        MyNUnitTestElement.Types.After);
                    var AfterClass = ExtractTestAndIncorrectTestElements(methods,
                        MyNUnitTestElement.Types.AfterClass);

                    foreach (var methodBeforeClass in beforeClass.Item1)
                    {
                        methodBeforeClass.RunMethod();
                    }


                });
        });

        return testClassesOutputs.ToList();
    }

    private static List<IEnumerable<MethodInfo>?>
        ExtractMethodsWithAppropriateAttributes(IEnumerable<MethodInfo>?
        methods)
    {
        var types = new Type[] {typeof(BeforeClassAttribute),
            typeof(BeforeAttribute), typeof(MyTestAttribute),
            typeof(AfterAttribute), typeof(AfterClassAttribute)};
        var suitableMethods = new List<IEnumerable<MethodInfo>?>();

        for (var i = 0; i < types.Length; ++i) 
        {
            suitableMethods[i] = from method in methods
                              where method.IsDefined(types[i])
                              select method;
        }

        return suitableMethods;
    }

    private static (List<MyNUnitTestElement>, List<string>) ExtractTestAndIncorrectTestElements(
        IEnumerable<MethodInfo> methods, MyNUnitTestElement.Types type)
    {
        var testSuitElements = new List<MyNUnitTestElement>();
        var incorrectElements = new List<string>();
        var beforeAndAfterCondition = (MethodInfo methodInfo)
            => methodInfo.GetParameters().Length == 0
            && methodInfo.ReturnType == typeof(void);
        var beforeClassAndAfterClassCondition = (MethodInfo methodInfo)
            => methodInfo.GetParameters().Length == 0
            && methodInfo.ReturnType == typeof(void)
            && methodInfo.IsStatic;

        foreach(var method in methods)
        {
            if (method.GetParameters().Length == 0
            && method.ReturnType == typeof(void))
            {
                if (type == MyNUnitTestElement.Types.After ||
                type == MyNUnitTestElement.Types.Before)
                {
                    testSuitElements.Add(new MyNUnitTestElement(type, method));
                }
                else 
                {
                    if (method.IsStatic)
                    {
                        testSuitElements.Add(new MyNUnitTestElement(type, method));
                    } else
                    {
                        incorrectElements.Add(method.Name);
                    }
                }
            }
            else
            {
                incorrectElements.Add(method.Name);
            }
        }
        return (testSuitElements, incorrectElements);
    }
}

