namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// The path expression visitor.
    /// </summary>
    internal static class PropertyPathParser
    {
        private static readonly ConcurrentDictionary<LambdaExpression, IReadOnlyList<PropertyInfo>> Cache = new ConcurrentDictionary<LambdaExpression, IReadOnlyList<PropertyInfo>>(PropertyPathComparer.Default);

        internal static IReadOnlyList<PropertyInfo> GetPath<TSource, TResult>(Expression<Func<TSource, TResult>> propertyPath)
        {
            return Cache.GetOrAdd(propertyPath, Create);
        }

        internal static IReadOnlyList<PropertyInfo> GetPath<T>(Expression<Func<T>> propertyPath)
        {
            return Cache.GetOrAdd(propertyPath, Create);
        }

        private static IReadOnlyList<PropertyInfo> Create(LambdaExpression expression)
        {
            var path = new List<PropertyInfo>();
            var member = expression.GetRootProperty();
            while (member != null)
            {
                path.Add(member.Property());
                member = member.GetPreviousProperty();
            }

            path.Reverse();
            return path;
        }

        private static PropertyInfo Property(this MemberExpression member)
        {
            var property = (PropertyInfo)member.Member;
            var type = member.Expression.Type;
            if (property.ReflectedType == type)
            {
                return property;
            }

            property = type.GetProperty(member.Member.Name);
            if (property != null)
            {
                return property;
            }

            return new InterfaceProperty((PropertyInfo)member.Member, type);
        }

        private class InterfaceProperty : PropertyInfo
        {
            private readonly PropertyInfo property;
            private readonly Type reflectedType;

            public InterfaceProperty(PropertyInfo property, Type reflectedType)
            {
                this.property = property;
                this.reflectedType = reflectedType;
            }

            /// <inheritdoc />
            public override string Name => this.property.Name;

            /// <inheritdoc />
            public override Type DeclaringType => this.property.DeclaringType;

            /// <inheritdoc />
            public override Type ReflectedType => this.reflectedType;

            /// <inheritdoc />
            public override Type PropertyType => this.property.PropertyType;

            /// <inheritdoc />
            public override PropertyAttributes Attributes => this.property.Attributes;

            /// <inheritdoc />
            public override bool CanRead => this.property.CanRead;

            /// <inheritdoc />
            public override bool CanWrite => this.property.CanWrite;

            /// <inheritdoc />
            public override object[] GetCustomAttributes(bool inherit) => this.property.GetCustomAttributes(inherit);

            /// <inheritdoc />
            public override bool IsDefined(Type attributeType, bool inherit) => this.property.IsDefined(attributeType, inherit);

            /// <inheritdoc />
            public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
            {
                return this.property.GetValue(obj, invokeAttr, binder, index, culture);
            }

            /// <inheritdoc />
            public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
            {
                this.property.SetValue(obj, value, invokeAttr, binder, index, culture);
            }

            /// <inheritdoc />
            public override MethodInfo[] GetAccessors(bool nonPublic) => this.property.GetAccessors(nonPublic);

            /// <inheritdoc />
            public override MethodInfo GetGetMethod(bool nonPublic) => this.property.GetGetMethod(nonPublic);

            /// <inheritdoc />
            public override MethodInfo GetSetMethod(bool nonPublic) => this.property.GetSetMethod(nonPublic);

            /// <inheritdoc />
            public override ParameterInfo[] GetIndexParameters() => this.property.GetIndexParameters();

            /// <inheritdoc />
            public override object[] GetCustomAttributes(Type attributeType, bool inherit) => this.property.GetCustomAttributes(attributeType, inherit);

            /// <inheritdoc />
            public override bool Equals(object obj)
            {
                if (obj is null)
                {
                    return false;
                }

                if (ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj.GetType() != this.GetType())
                {
                    return false;
                }

                return this.Equals((InterfaceProperty)obj);
            }

            /// <inheritdoc />
            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = base.GetHashCode();
                    hashCode = (hashCode * 397) ^ this.property.GetHashCode();
                    hashCode = (hashCode * 397) ^ this.reflectedType.GetHashCode();
                    return hashCode;
                }
            }

            protected bool Equals(InterfaceProperty other)
            {
                return base.Equals(other) && this.property.Equals(other.property) && this.reflectedType.Equals(other.reflectedType);
            }
        }
    }
}
