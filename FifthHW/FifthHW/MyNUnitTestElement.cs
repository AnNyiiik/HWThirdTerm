using System.Reflection;

namespace FifthHW
{
	public class MyNUnitTestElement
	{
		
		public MyNUnitTestElement(Type type, MethodInfo method)
		{
			this.type = type;
			this.method = method;
		}

        public Type type { get; private set; }

		public MethodInfo method { get; private set; }

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
}