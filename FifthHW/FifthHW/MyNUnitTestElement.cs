using System;
using System.Reflection;

namespace FifthHW
{
	public class MyNUnitTestElement
	{
		public enum Types
		{
			Before,
			After,
			BeforeClass,
			AfterClass
		}

		public MyNUnitTestElement(Types type, MethodInfo method,
			object? obj = null)
		{
			this.type = type;
			this.method = method;
			this.obj = obj;
		}

        public Types type { get; private set; }

		public MethodInfo method { get; private set; }

		private readonly object? obj;

		public void RunMethod(object? obj = null)
		{
			try
			{
				if (this.type == Types.BeforeClass || this.type ==
					Types.BeforeClass)
				{
                    method.Invoke(null, new object[] { });
					return;
                }
                method.Invoke(obj, new object[] { });
            } catch 
			{
				return;
			}
		}
    }
}