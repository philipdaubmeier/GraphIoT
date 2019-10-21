using System;
using System.Linq.Expressions;
using System.Reflection;

namespace PhilipDaubmeier.GraphIoT.Core.Database.Util
{
    internal static class ReflectionExtensions
    {
        internal static MethodInfo GetMethodInfo<T1, T2, T3, Tresult>(Expression<Func<T1, T2, T3, Tresult>> expression)
        {
            var methodExpr = (expression.Body as MethodCallExpression);
            if (methodExpr is null)
                throw new ArgumentException("The given expression body was not of type MethodCallExpression", nameof(expression));

            return methodExpr.Method;
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
                if (toCheck.BaseType is null)
                    break;
                toCheck = toCheck.BaseType;
            }
            return false;
        }

        internal static void SetPropertyValue(this PropertyInfo propertyInfo, object inputObject, object? propertyVal)
        {
            var propertyType = propertyInfo.PropertyType;

            // is it a nullable type?
            Type targetType = propertyType.IsNullableType() ? Nullable.GetUnderlyingType(propertyType) ?? propertyType : propertyType;

            if (targetType == typeof(byte[]) && propertyVal != null)
                propertyVal = Convert.FromBase64String((string)propertyVal);

            if (propertyVal != null && targetType != null && targetType != propertyVal.GetType())
            {
                // is there an implicit conversion operator?
                var converter = targetType.GetMethod("op_Implicit", new[] { propertyVal.GetType() });
                if (converter != null)
                {
                    object? implicitPropertyVal = converter.Invoke(null, new[] { propertyVal });
                    if (implicitPropertyVal != null)
                        propertyVal = implicitPropertyVal;
                }
                
                // is there a constructor for conversion?
                var constructor = targetType.GetConstructor(new[] { propertyVal.GetType() });
                if (constructor != null)
                {
                    object? constructedPropertyVal = Activator.CreateInstance(targetType, propertyVal);
                    if (constructedPropertyVal != null)
                        propertyVal = constructedPropertyVal;
                }
            }

            if (targetType != null)
            {
                object? changeTypePropertyVal = Convert.ChangeType(propertyVal, targetType);
                if (changeTypePropertyVal != null)
                    propertyVal = changeTypePropertyVal;
            }

            propertyInfo.SetValue(inputObject, propertyVal, null);
        }
    }
}