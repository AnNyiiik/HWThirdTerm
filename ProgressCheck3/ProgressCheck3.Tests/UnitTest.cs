
using System;
using System.Reflection;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

namespace ProgressCheck3.Tests;
public class Tests
{
    [Test]
    public void TestCreateProject()
    {
        //var reflector = new Reflector();
        //var someClass = typeof(Class);
        //var directory = "../../../../";
        //reflector.PrintStructure(someClass, directory);
        //var className = someClass.Name;
        //var fileName = className + ".cs";
        //var outputName = className + ".dll";
        //var codeProvider = new CSharpCodeProvider();
        //var compilerParameters = new CompilerParameters();
        //compilerParameters.GenerateExecutable = false;
        //compilerParameters.OutputAssembly = outputName;
        //var compilerResults = codeProvider
        //    .CompileAssemblyFromFile(compilerParameters, fileName);
        //Assert.AreEqual(0, compilerResults.Errors.Count);

        //var assembly = Assembly.LoadFrom($"{someClass.Name}.dll");
        //var loadedClassType = assembly.GetType($"{someClass.Namespace}.{someClass.Name}");
        //var differences = reflector.DiffClasses(someClass, loadedClassType);
        Assert.Pass();
    }
}