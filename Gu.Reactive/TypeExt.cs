namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Helpers for <see cref="Type"/>.
    /// </summary>
    public static class TypeExt
    {
        private static readonly Dictionary<Type, string> Map = new Dictionary<Type, string>
        {
            { typeof(bool), "bool" },
            { typeof(byte), "byte" },
            { typeof(char), "char" },
            { typeof(decimal), "decimal" },
            { typeof(double), "double" },
            { typeof(float), "float" },
            { typeof(int), "int" },
            { typeof(long), "long" },
            { typeof(object), "object" },
            { typeof(sbyte), "sbyte" },
            { typeof(short), "short" },
            { typeof(string), "string" },
            { typeof(uint), "uint" },
            { typeof(ulong), "ulong" },
        };

        /// <summary>
        /// Returns nicely formatted type names for generic types.
        /// </summary>
        /// <param name="type">The <see cref="Type"/>.</param>
        /// <returns>A string representation of the type.</returns>
        public static string PrettyName(this Type type)
        {
            if (Map.TryGetValue(type, out var mapped))
            {
                return mapped;
            }

            return type switch
            {
                { IsGenericType: true } => Generic(),
                { IsArray: true } => type.GetElementType()!.PrettyName() + "[]",
                { DeclaringType: { } declaringType } => $"{declaringType.PrettyName()}.{type.Name}",
                _ => type.Name,
            };

            string Generic()
            {
                var arguments = string.Join(", ", type.GenericTypeArguments.Select(PrettyName));

                if (type.DeclaringType != null)
                {
                    return $"{type.DeclaringType.PrettyName()}.{type.Name.Split('`').First()}<{arguments}>";
                }

                return $"{type.Name.Split('`').First()}<{arguments}>";
            }
        }

        /// <summary>
        /// Check if a type is Nullable`1.
        /// </summary>
        /// <param name="type">The <see cref="Type"/>.</param>
        /// <returns>True if <paramref name="type"/> is <see cref="Nullable{T}"/>.</returns>
        public static bool IsNullable(this Type type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}
