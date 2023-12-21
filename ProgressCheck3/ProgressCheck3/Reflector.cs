using System.Reflection;
using System.Text;
public class Reflector
{
    /// <summary>
    /// Creates a file named <class name>.cs with class code.
    /// </summary>
    /// <param name="someClass">type of which class file should be extracted</param>
    /// <param name="dirToWrite">directory path to write extracted file</param>
    public void PrintStructure(Type someClass, string dirToWrite)
    {
        string className = someClass.Name;
        string fileName = className + ".cs";

        using (StreamWriter writer = new StreamWriter(dirToWrite + fileName))
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"namespace {someClass.Namespace}");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine($"    {GetVisibility(someClass)} " +
                $"class {className}{GetGenericParameters(someClass)}");
            stringBuilder.AppendLine("    {");

            var fields = someClass.GetFields();
            foreach(var field in fields)
            {
                stringBuilder.AppendLine($"        {GetVisibility(field)} " +
                    $"{GetFieldType(field)} {field.Name};");
            }
            var methods = someClass.GetMethods();
            foreach(var method in methods)
            {
                stringBuilder.AppendLine($"        {GetVisibility(method)} " +
                    $"{GetReturnType(method)} {method.Name}" +
                    $"{GetGenericParameters(method)}" +
                    $"({GetMethodParameters(method)});");
            }
            var nestedClasses = someClass.GetNestedTypes();
            foreach(var nestedClass in nestedClasses)
            {
                stringBuilder.AppendLine($"        {GetVisibility(nestedClass)} " +
                    $"class {nestedClass.Name}" +
                    $"{GetGenericParameters(nestedClass)}");
                stringBuilder.AppendLine("        {");
                stringBuilder.AppendLine("        }");
            }

            var interfaces = someClass.GetInterfaces();

            foreach(var interfaceItem in interfaces)
            {
                stringBuilder.AppendLine($"        {GetVisibility(interfaceItem)} " +
                    $"interface {interfaceItem.Name}" +
                    $"{GetGenericParameters(interfaceItem)}");
                stringBuilder.AppendLine("        {");
                stringBuilder.AppendLine("        }");
            }
            stringBuilder.AppendLine("    }");
            stringBuilder.AppendLine("}");

            writer.Write(stringBuilder.ToString());
        }
    }

    /// <summary>
    /// Print dirrent fields and methods of a given types.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    public string DiffClasses(Type a, Type b)
    {
        var result = new StringBuilder();
        result.AppendLine("Fields:");
        result.Append(GetTypesDifferences(a.GetFields(), b.GetFields()));
        result.AppendLine("Methods:");
        result.Append(GetTypesDifferences(a.GetMethods(), b.GetMethods()));
        return result.ToString();
    }

    private string GetTypesDifferences(MemberInfo[] a, MemberInfo[] b)
    {
        var result = new StringBuilder();
        var differences = a.Except(b).Union(b.Except(a));
        foreach (var difference in differences)
        {
            result.AppendLine(difference.Name);
        }
        return result.ToString();
    }

    private string GetVisibility(MemberInfo member)
    {
        if (member is Type type)
        {
            if (type.IsNested)
            {
                return GetVisibilityOfTypeAttributes(type.Attributes);
            }
            else
            {
                return GetVisibilityOfTypeAttributes(type.Attributes) + (type.IsInterface ? " interface" : " class");
            }
        }
        else if (member is FieldInfo field)
        {
            return GetVisibilityOfFieldAttributes(field.Attributes);
        }
        else if (member is MethodInfo)
        {
            var method = member as MethodInfo;
            var attr = method?.Attributes;
            return  (method.IsStatic ? " static" : "") + (method.IsAbstract ? " abstract" : "");
        }
        return "";
    }

    private string GetVisibilityOfTypeAttributes(TypeAttributes attributes)
    {
        switch (attributes & TypeAttributes.VisibilityMask)
        {
            case TypeAttributes.Public:
                return "public";
            case TypeAttributes.NestedPublic:
                return "public";
            case TypeAttributes.NestedPrivate:
                return "private";
            case TypeAttributes.NestedFamily:
                return "protected";
            case TypeAttributes.NestedAssembly:
                return "internal";
            case TypeAttributes.NestedFamORAssem:
                return "protected internal";
            default:
                return "";
        }
    }
    private string GetVisibilityOfFieldAttributes(FieldAttributes attributes)
    {
        switch (attributes & FieldAttributes.FieldAccessMask)
        {
            case FieldAttributes.Public:
                return "public";
            case FieldAttributes.Private:
                return "private";
            case FieldAttributes.Family:
                return "protected";
            case FieldAttributes.Assembly:
                return "internal";
            case FieldAttributes.FamORAssem:
                return "protected internal";
            default:
                return "";
        }
    }

    private string GetFieldType(FieldInfo field)
    {
        return field.FieldType.Name;
    }

    private string GetReturnType(MethodInfo method)
    {
        return method.ReturnType.Name;
    }

    private string GetGenericParameters(Type type)
    {
        if (type.GetGenericArguments().Length > 0)
        {
            var genericArgs = type.GetGenericArguments().Select(a => a.Name).ToArray();
            return $"<{string.Join(", ", genericArgs)}>";
        }
        return "";
    }
    private string GetGenericParameters(MethodInfo method)
    {
        if (method.GetGenericArguments().Length > 0)
        {
            var genericArgs = method.GetGenericArguments().Select(a => a.Name).ToArray();
            return $"<{string.Join(", ", genericArgs)}>";
        }
        return "";
    }
    private string GetMethodParameters(MethodInfo method)
    {
        var parameters = method.GetParameters();
        string[] parameterDeclarations = new string[parameters.Length];
        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            parameterDeclarations[i] = $"{parameter.ParameterType.Name} " +
                $"{parameter.Name}";
        }
        return string.Join(", ", parameterDeclarations);
    }
}