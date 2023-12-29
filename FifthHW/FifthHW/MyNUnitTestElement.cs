using System.Reflection;

namespace FifthHW;

/// <summary>
/// Test element item (before/after/before (after) class)
/// </summary>
public class MyNUnitTestElement
{
	
	public MyNUnitTestElement(Type type, MethodInfo method)
	{
		this.type = type;
		this.method = method;
	}

        public Type type { get; private set; }

	public MethodInfo method { get; private set; }

	/// <summary>
	/// Executes test elemnt method.
	/// </summary>
	/// <param name="classObject">Object to which method belongs if it's not static.</param>
	public void RunMethod(object? classObject)
	{
		try
		{
			if (type == typeof(BeforeClassAttribute) || type ==
				typeof(BeforeAttribute))
			{
                    method.Invoke(null, new object[] { });
				return;
                }
                method.Invoke(classObject, new object[] { });
            } catch 
		{
			return;
		}
	}
}