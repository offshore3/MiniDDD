using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniDDD.Extensions
{
    public static class ExtensionLib
    {
        private static readonly Dictionary<string, Type> _typeMap = new Dictionary<string, Type>();

        ///<summary>Finds the class that the string represents within any loaded assembly. Calling with "MyNameSpace.MyObject" would return the same type as typeof(MyNameSpace.MyObject) etc.</summary>
        public static Type AsType(this string valueType)
        {
            lock (_typeMap)
            {
                Type type;
                if (_typeMap.TryGetValue(valueType, out type))
                {
                    return type;
                }

                var types = AppDomain.CurrentDomain.GetAssemblies()
                    .Select(assembly => assembly.GetType(valueType))
                    .Where(t => t != null)
                    .ToArray();
                if (types!=null && !types.Any())
                {
                    throw new Exception("Not matched type:"+valueType);
                }

                if (types.Count() > 1)
                {
                    throw new Exception("Mutliple types:"+valueType);
                }

                type = types.Single();
                _typeMap.Add(valueType, types.Single());
                return type;
            }
        }
    }
}
