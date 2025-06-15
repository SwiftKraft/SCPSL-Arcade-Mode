using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SwiftUHC.Utils.Extensions
{
    public static class ReflectionExtensions
    {
        public static List<Type> GetAllNonAbstractSubclasses<T>()
        {
            var baseType = typeof(T);

            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly =>
                {
                    // Avoid ReflectionTypeLoadException by handling bad assemblies
                    try
                    {
                        return assembly.GetTypes();
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        return ex.Types.Where(t => t != null);
                    }
                })
                .Where(t => t != null
                            && t.IsClass
                            && !t.IsAbstract
                            && baseType.IsAssignableFrom(t)
                            && t != baseType) // exclude the base type itself
                .ToList();
        }
    }
}
