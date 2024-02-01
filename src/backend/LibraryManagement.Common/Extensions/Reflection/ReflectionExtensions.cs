
using System.Reflection;

namespace LibraryManagement.Common.Extensions.Reflection
{
    public static class ReflectionExtensions
    {
        public static IEnumerable<Type> GetTypesFromAssembly(Type type, Type implementedType, string @namespace)
        {
            var types = GetTypesFromAssembly(type, @namespace);
            return types.Where(x => implementedType.IsAssignableFrom(x) && x.IsPublic).ToList();
        }

        public static IEnumerable<Type> GetTypesFromAssembly(Type type, string @namespace)
        {
            var assembly = Assembly.GetAssembly(type);
            if (assembly == null)
            {
                throw new InvalidOperationException($"Could not found the assembly of specified type {type}");
            }

            var types = assembly.GetTypes().Where(t =>
                t.Namespace != null &&
                t.Namespace.StartsWith(@namespace) &&
                t.DeclaringType == null &&
                !t.IsAbstract
            ).ToList();

            return types;
        }
    }
}
