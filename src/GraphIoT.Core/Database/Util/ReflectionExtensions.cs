using System;
using System.Linq.Expressions;
using System.Reflection;

namespace PhilipDaubmeier.GraphIoT.Core.Database.Util
{
    internal static class ReflectionExtensions
    {
        internal static MethodInfo GetMethodInfo<T1, T2, T3, Tresult>(Expression<Func<T1, T2, T3, Tresult>> expression)
        {
            return (expression.Body as MethodCallExpression)?.Method;
        }

        internal static bool IsNullableType(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }

        internal static bool IsSubclassOfGeneric(this Type toCheck, Type generic)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                    return true;
                toCheck = toCheck.BaseType;
            }
            return false;
        }

        internal static void SetPropertyValue(this PropertyInfo propertyInfo, object inputObject, object propertyVal)
        {
            var type = inputObject.GetType();
            var propertyType = propertyInfo.PropertyType;

            // is it a nullable type?
            var targetType = propertyType.IsNullableType() ? Nullable.GetUnderlyingType(propertyType) : propertyType;

            if (targetType == typeof(byte[]))
                propertyVal = Convert.FromBase64String((string)propertyVal);

            if (propertyVal != null && targetType != propertyVal.GetType())
            {
                // is there an implicit conversion operator?
                var converter = targetType.GetMethod("op_Implicit", new[] { propertyVal.GetType() });
                if (converter != null)
                    propertyVal = converter.Invoke(null, new[] { propertyVal });

                // is there a constructor for conversion?
                var constructor = targetType.GetConstructor(new[] { propertyVal.GetType() });
                if (constructor != null)
                    propertyVal = Activator.CreateInstance(targetType, propertyVal);
            }

            propertyVal = Convert.ChangeType(propertyVal, targetType);
            propertyInfo.SetValue(inputObject, propertyVal, null);
        }
    }
}