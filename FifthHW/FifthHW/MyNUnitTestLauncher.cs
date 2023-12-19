using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace FifthHW;

public static class MyNUnitTestLauncher
{
	

	public static List<ClassTestElements.TestClassOutput> RunAllTests(string path)
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
                    var constructor = classItem.GetConstructor(Type.EmptyTypes);
                    var methods = classItem.GetTypeInfo().DeclaredMethods;

                    var suitableMethods = ExtractMethodsWithAppropriateAttributes(methods);

                    var beforeClass = ExtractTestAndIncorrectTestElements(suitableMethods[0],
                        MyNUnitTestElement.Types.BeforeClass);
                    var before = ExtractTestAndIncorrectTestElements(suitableMethods[1],
                        MyNUnitTestElement.Types.Before);
                    var after = ExtractTestAndIncorrectTestElements(suitableMethods[2],
                        MyNUnitTestElement.Types.After);
                    var afterClass = ExtractTestAndIncorrectTestElements(suitableMethods[3],
                        MyNUnitTestElement.Types.AfterClass);
                    var tests = ExtractTestAndIncorrectTest(suitableMethods[4], constructor);

                    var allTestMethods = new ClassTestElements(beforeClass.Item1,
                                                               before.Item1,
                                                               afterClass.Item1,
                                                               after.Item1,
                                                               tests.Item1);
                    allTestMethods.RunTestClass();

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

    private static (List<MyNUnitTest>?, List<string>?) ExtractTestAndIncorrectTest(
        IEnumerable<MethodInfo>? methods, ConstructorInfo? constructor)
    {
        if (methods == null)
        {
            return (null, null);
        }
        var tests = new List<MyNUnitTest>();
        var incorrectTests = new List<string>();
        foreach (var method in methods)
        {
            if (method.GetParameters().Length == 0
            && method.ReturnType == typeof(void))
            {
                var obj = constructor!.Invoke(new object[] { });
                tests.Add(new MyNUnitTest(method, obj));
            } else
            {
                incorrectTests.Add(method.Name);
            }
        }
        return (tests, incorrectTests);
    }

    private static (List<MyNUnitTestElement>?, List<string>?) ExtractTestAndIncorrectTestElements(
        IEnumerable<MethodInfo>? methods, MyNUnitTestElement.Types type)
    {
        if (methods == null)
        {
            return (null, null);
        }
        var testElements = new List<MyNUnitTestElement>();
        var incorrectElements = new List<string>();

        foreach(var method in methods)
        {
            if (method.GetParameters().Length == 0
            && method.ReturnType == typeof(void))
            {

                if (type == MyNUnitTestElement.Types.After ||
                type == MyNUnitTestElement.Types.Before)
                {
                    testElements.Add(new MyNUnitTestElement(type, method));
                }
                else 
                {
                    if (method.IsStatic)
                    {
                        testElements.Add(new MyNUnitTestElement(type, method));
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
        return (testElements, incorrectElements);
    }
}