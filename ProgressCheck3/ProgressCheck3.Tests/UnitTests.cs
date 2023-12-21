using System.CodeDom.Compiler;
using System.Reflection;
using Microsoft.CSharp;
namespace ProgressCheck3.Tests;

class UnitTests
{
    [Test]
    public void Test()
    {
        Reflector reflector = new Reflector();
        Type someClassType = typeof(Class);
        reflector.PrintStructure(someClassType);
        string fileName = someClassType.Name + ".cs";
        string outputName = someClassType.Name + ".dll";
        var codeProvider = new CSharpCodeProvider();
        var compilerParameters = new CompilerParameters();
        compilerParameters.GenerateExecutable = false;
        compilerParameters.OutputAssembly = outputName;
        var compilerResults = codeProvider.CompileAssemblyFromFile(compilerParameters, fileName);
        Assembly assembly = Assembly.LoadFrom($"{someClassType.Name}.dll");
        Type loadedClassType = assembly
            .GetType($"{someClassType.Namespace}.{someClassType.Name}");
        var differences = reflector.CountDifferences(someClassType, loadedClassType);
        Assert.That(differences, Is.EqualTo(0));
    }
}