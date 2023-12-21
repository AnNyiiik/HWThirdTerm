
using System;
using System.Reflection;
using System.CodeDom;
using Microsoft.CSharp;

namespace ProgressCheck3.Tests;
public class Tests
{
    [Test]
    public void TestCreateProject()
    {
        var reflector = new Reflector();
        var someClass = typeof(Class);
        var directory = "../../../../";
        reflector.PrintStructure(someClass, directory);
        // Компилируем и собираем проект в библиотеку
        var className = someClass.Name;
        var fileName = className + ".cs";
        string outputName = className + ".dll";
        var codeProvider = new Microsoft.CSharp.;
        CompilerParameters compilerParameters = new CompilerParameters();
        compilerParameters.GenerateExecutable = false;
        compilerParameters.OutputAssembly = outputName;
        CompilerResults compilerResults = codeProvider.CompileAssemblyFromFile(compilerParameters, fileName);
        if (compilerResults.Errors.Count > 0)
        {
            Console.WriteLine("Errors while compiling the project:");
            foreach (CompilerError error in compilerResults.Errors)
            {
                Console.WriteLine($"Line {error.Line}: {error.ErrorText}");
            }
        }
        else
        {
            Console.WriteLine("Project compiled successfully");
        }
        // Загружаем собранную библиотеку
        Assembly assembly = Assembly.LoadFrom($"{someClassType.Name}.dll");
        // Получаем тип загруженного класса
        Type loadedClassType = assembly.GetType($"{someClassType.Namespace}.{someClassType.Name}");
        // Проверяем различия в полях и методах между исходным и загруженным классами
        reflector.DiffClasses(someClassType, loadedClassType);
        Assert.Pass();
    }
}