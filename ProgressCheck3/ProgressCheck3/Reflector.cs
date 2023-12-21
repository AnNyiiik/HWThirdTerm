using System.Reflection;
using System.Text;

namespace ProgressCheck3;

public class Reflector
{
    
    public void PrintStructure(Type someClass, string? dirToWrite = null)
    {
        var className = someClass.Name;
        var fileName = className + ".cs";
        using (var writer = new StreamWriter(dirToWrite + fileName))
        {
            PrintClass(writer, someClass);
        }
    }

    private void PrintClass(StreamWriter writer, Type someClass)
    {

        writer.WriteLine($"public class {someClass.Name}");
        writer.WriteLine("{");
        PrintFields(writer, someClass);
        PrintMethods(writer, someClass);

        var nestedClasses = someClass.GetNestedTypes(BindingFlags.Public
        | BindingFlags.NonPublic);
        foreach (var nestedClass in nestedClasses)
        {
            PrintClass(writer, nestedClass);
        }

        PrintInterfaces(writer, someClass);
        writer.WriteLine("}");
    }

    private void PrintFields(StreamWriter writer, Type someClass)
    {
        var fields = someClass.GetFields(BindingFlags.Public | BindingFlags.
            NonPublic | BindingFlags.Instance | BindingFlags.Static);

        foreach (var field in fields)
        {
            var modifiers = GetModifiers(field);
            var fieldType = GetFieldType(field.FieldType);
            var fieldName = field.Name;
            writer.WriteLine($"{modifiers} {fieldType} {fieldName};");
        }
        writer.WriteLine();
    }

    private void PrintMethods(StreamWriter writer, Type someClass)
    {
        var methods = someClass.GetMethods(BindingFlags.Public |
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        foreach(var method in methods)
        {
            var modifiers = GetModifiers(method);
            var returnType = GetFieldType(method.ReturnType);
            var methodName = method.Name;
            var parameters = string.Join(", ", method.GetParameters().
                Select(p => $"{GetFieldType(p.ParameterType)} {p.Name}"));
            writer.WriteLine($"{modifiers} {returnType} {methodName}({parameters});");
        }
        writer.WriteLine();
    }

    private void PrintInterfaces(StreamWriter writer, Type someClass)
    {
        var interfaces = someClass.GetInterfaces();
        foreach(var interfaceItem in interfaces)
        {
            writer.WriteLine($"public interface {interfaceItem.Name}");
            writer.WriteLine("{");
            PrintMethods(writer, interfaceItem);
            writer.WriteLine("}");
        }
        writer.WriteLine();
    }

    private string GetModifiers(MemberInfo member)
    {
        var modifiers = "";
        if (member is FieldInfo)
        {
            var item = member as FieldInfo;
            if (item.IsPublic)
                modifiers += "public ";
            if (item.IsPrivate)
                modifiers += "private ";
            if (item.IsFamily)
            modifiers += "protected ";
            if (item.IsStatic)
                modifiers += "static ";
        }  else
        {
            var item = member as MethodInfo;
            if (item.IsPublic)
                modifiers += "public ";
            if (item.IsPrivate)
                modifiers += "private ";
            if (item.IsFamily)
                modifiers += "protected ";
            if (item.IsStatic)
                modifiers += "static ";
        }
        return modifiers.Trim();
    }

    private string GetFieldType(Type fieldType)
    {
        var typeName = fieldType.Name;
        if (fieldType.IsGenericType)
        {
            typeName = typeName.Substring(0, typeName.IndexOf("`"));
            typeName += "<";
            typeName += string.Join(", ", fieldType.GetGenericArguments()
                .Select(arg => GetFieldType(arg)));
            typeName += ">";
        }
        return typeName;
    }

    public int CountDifferences(Type a, Type b)
    {
        return (Compare(a, b, typeof(FieldInfo)).Length +
            Compare(a, b, typeof(MethodInfo)).Length);
    }

    public string DiffClasses(Type a, Type b)
    {
        var result = new StringBuilder();
        result.AppendLine("Fields:");
        result.AppendLine(Compare(a, b, typeof(FieldInfo)));
        result.AppendLine();
        result.AppendLine("Methods:");
        result.AppendLine(Compare(a, b, typeof(MethodInfo)));
        return result.ToString();
    }

    private string FindDifference(MemberInfo[] itemFirst, MemberInfo[] itemSecond)
    {
        var differenceResult = new StringBuilder();
        var difference = itemFirst.Except(itemSecond).Union(itemSecond
            .Except(itemFirst));
        foreach (var diff in difference)
        {
            differenceResult.AppendLine(diff.Name);
        }
        return differenceResult.ToString();
    }

    private string Compare(Type a, Type b, Type typeToCompare)
    {
        var bindFlags = BindingFlags.Public
            | BindingFlags.NonPublic | BindingFlags.Instance
            | BindingFlags.Static;
        if (typeToCompare == typeof(FieldInfo))
        {
            return FindDifference(a.GetFields(bindFlags), b.GetFields(bindFlags));
        } else if (typeToCompare == typeof(PropertyInfo))
        {
            return FindDifference(a.GetProperties(bindFlags),
                b.GetProperties(bindFlags));
            
        }
        return FindDifference(a.GetMethods(bindFlags), b.GetMethods(bindFlags));
    }
}