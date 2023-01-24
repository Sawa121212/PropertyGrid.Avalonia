using System.ComponentModel;
using System.Globalization;
using Avalonia.Data.Converters;

namespace PropertyGrid.Converters
{
    public class SelectedObjectConverter : IValueConverter
    {
        private const string ValidParameterMessage =
            @"parameter must be one of the following strings: 'Type', 'TypeName', 'SelectedObjectName'";

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
                throw new ArgumentNullException("parameter");

            if (!(parameter is string))
                throw new ArgumentException(ValidParameterMessage);

            if (CompareParam(parameter, "Type"))
                return ConvertToType(value, culture);
            if (CompareParam(parameter, "TypeName"))
                return ConvertToTypeName(value, culture);
            if (CompareParam(parameter, "SelectedObjectName"))
                return ConvertToSelectedObjectName(value, culture);
            throw new ArgumentException(ValidParameterMessage);
        }

        private bool CompareParam(object parameter, string parameterValue)
        {
            return string.Compare((string) parameter, parameterValue, StringComparison.OrdinalIgnoreCase) == 0;
        }

        private object ConvertToType(object value, CultureInfo culture)
        {
            return value?.GetType();
        }

        private object ConvertToTypeName(object value, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            var newType = value.GetType();

            //ICustomTypeProvider is only available in .net 4.5 and over. Use reflection so the .net 4.0 and .net 3.5 still works.
            if (newType.GetInterface("ICustomTypeProvider", true) != null)
            {
                var methodInfo = newType.GetMethod("GetCustomType");
                if (methodInfo != null) newType = methodInfo.Invoke(value, null) as Type;
            }

            var displayNameAttribute =
                newType.GetCustomAttributes(false).OfType<DisplayNameAttribute>().FirstOrDefault();

            return displayNameAttribute == null
                ? newType.Name
                : displayNameAttribute.DisplayName;
        }

        private object ConvertToSelectedObjectName(object value, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            var newType = value.GetType();
            var properties = newType.GetProperties();
            foreach (var property in properties)
                if (property.Name == "Name")
                    return property.GetValue(value, null);

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
}