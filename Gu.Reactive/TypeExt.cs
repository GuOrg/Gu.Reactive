namespace Gu.Reactive
{
    using System;
    using System.Linq;

    public static class TypeExt
    {
        /// <summary>
        /// Returns nicely formatted type names for generic types.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string PrettyName(this Type type)
        {
            if (type == typeof(long))
            {
                return "long";
            }

            if (type == typeof(double))
            {
                return "double";
            }

            if (type == typeof(int))
            {
                return "int";
            }

            if (type == typeof(string))
            {
                return "string";
            }

            if (type == typeof(bool))
            {
                return "bool";
            }

            if (type.IsGenericType)
            {
                var arguments = string.Join(", ", type.GenericTypeArguments.Select(PrettyName));
                return string.Format("{0}<{1}>", type.Name.Split('`').First(), arguments);
            }

            return type.Name;
        }
    }
}
