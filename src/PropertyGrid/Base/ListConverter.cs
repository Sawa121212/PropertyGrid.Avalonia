using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace PropertyGrid.Base
{
    public class ListConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return true;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
                return null;

            var names = value as string;

            var list = new List<object>();
            if (names == null && value != null)
            {
                list.Add(value);
            }
            else
            {
                if (names == null)
                    return null;

                foreach (var name in names.Split(','))
                    list.Add(name.Trim());
            }

            return new ReadOnlyCollection<object>(list);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            if (destinationType != typeof(string))
                throw new InvalidOperationException("Can only convert to string.");


            var strs = (IList) value;

            if (strs == null)
                return null;

            var sb = new StringBuilder();
            var first = true;
            foreach (var o in strs)
            {
                if (o == null)
                    throw new InvalidOperationException("Property names cannot be null.");

                var s = o as string;
                if (s == null)
                    throw new InvalidOperationException("Does not support serialization of non-string property names.");

                if (s.Contains(','))
                    throw new InvalidOperationException("Property names cannot contain commas.");

                if (s.Trim().Length != s.Length)
                    throw new InvalidOperationException(
                        "Property names cannot start or end with whitespace characters.");

                if (!first)
                    sb.Append(", ");
                first = false;

                sb.Append(s);
            }

            return sb.ToString();
        }
    }
}