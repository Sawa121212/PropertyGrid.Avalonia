using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace PropertyGrid.Base
{
    internal static class ReflectionHelper
    {
        /// <summary>
        ///     Check the existence of the specified public instance (i.e. non static) property against
        ///     the type of the specified source object. If the property is not defined by the type,
        ///     a debug assertion will fail. Typically used to validate the parameter of a
        ///     RaisePropertyChanged method.
        /// </summary>
        /// <param name="sourceObject">The object for which the type will be checked.</param>
        /// <param name="propertyName">The name of the property.</param>
        [Conditional("DEBUG")]
        internal static void ValidatePublicPropertyName(object sourceObject, string propertyName)
        {
            if (sourceObject == null)
                throw new ArgumentNullException("sourceObject");

            if (propertyName == null)
                throw new ArgumentNullException("propertyName");

            Debug.Assert(
                sourceObject.GetType().GetProperty(propertyName,
                    BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public) != null,
                string.Format("Public property {0} not found on object of type {1}.", propertyName,
                    sourceObject.GetType().FullName));
        }

        /// <summary>
        ///     Check the existence of the specified instance (i.e. non static) property against
        ///     the type of the specified source object. If the property is not defined by the type,
        ///     a debug assertion will fail. Typically used to validate the parameter of a
        ///     RaisePropertyChanged method.
        /// </summary>
        /// <param name="sourceObject">The object for which the type will be checked.</param>
        /// <param name="propertyName">The name of the property.</param>
        [Conditional("DEBUG")]
        internal static void ValidatePropertyName(object sourceObject, string propertyName)
        {
            if (sourceObject == null)
                throw new ArgumentNullException("sourceObject");

            if (propertyName == null)
                throw new ArgumentNullException("propertyName");

            Debug.Assert(
                sourceObject.GetType().GetProperty(propertyName,
                    BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public |
                    BindingFlags.NonPublic) != null,
                string.Format("Public property {0} not found on object of type {1}.", propertyName,
                    sourceObject.GetType().FullName));
        }

        internal static bool TryGetEnumDescriptionAttributeValue(Enum enumeration, out string description)
        {
            try
            {
                var fieldInfo = enumeration.GetType().GetField(enumeration.ToString());
                var attributes =
                    fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true) as DescriptionAttribute[];
                if (attributes != null && attributes.Length > 0)
                {
                    description = attributes[0].Description;
                    return true;
                }
            }
            catch
            {
            }

            description = string.Empty;
            return false;
        }

        [DebuggerStepThrough]
        internal static string GetPropertyOrFieldName(MemberExpression expression)
        {
            string propertyOrFieldName;
            if (!TryGetPropertyOrFieldName(expression, out propertyOrFieldName))
                throw new InvalidOperationException("Unable to retrieve the property or field name.");

            return propertyOrFieldName;
        }

        [DebuggerStepThrough]
        internal static string GetPropertyOrFieldName<TMember>(Expression<Func<TMember>> expression)
        {
            string propertyOrFieldName;
            if (!TryGetPropertyOrFieldName(expression, out propertyOrFieldName))
                throw new InvalidOperationException("Unable to retrieve the property or field name.");

            return propertyOrFieldName;
        }

        [DebuggerStepThrough]
        internal static bool TryGetPropertyOrFieldName(MemberExpression expression, out string propertyOrFieldName)
        {
            propertyOrFieldName = null;

            if (expression == null)
                return false;

            propertyOrFieldName = expression.Member.Name;

            return true;
        }

        [DebuggerStepThrough]
        internal static bool TryGetPropertyOrFieldName<TMember>(Expression<Func<TMember>> expression,
            out string propertyOrFieldName)
        {
            propertyOrFieldName = null;

            if (expression == null)
                return false;

            return TryGetPropertyOrFieldName(expression.Body as MemberExpression, out propertyOrFieldName);
        }

        public static bool IsPublicInstanceProperty(Type type, string propertyName)
        {
            var flags = BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public;
            return type.GetProperty(propertyName, flags) != null;
        }
    }
}