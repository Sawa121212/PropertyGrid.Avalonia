using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using Avalonia;
using Avalonia.Data;
using PropertyGrid.Metadata;
using PropertyGrid.PropertyTypes;
using PropertyGrid.Utils;

namespace PropertyGrid.GridEntryTypes
{
    [DebuggerDisplay("DisplayName= {DisplayName} Component= {Component}")]
    public partial class PropertyItem
    {
        private readonly PropertyItemValue _parentValue;

        private readonly object _component;
        private object _unwrappedComponent;
        private readonly PropertyDescriptor _descriptor;
        private readonly AttributesContainer _metadata;

        // TODO: Reserved for future implementations.
        /// <summary>
        /// Возвращает родительское значение.
        /// <remarks>This property is reserved for future implementations</remarks>
        /// </summary>
        /// <value>The parent value.</value>
        public PropertyItemValue ParentValue
        {
            get { return _parentValue; }
        }

        public static readonly DirectProperty<PropertyItem, PropertyItemValue> PropertyValueProperty =
                AvaloniaProperty.RegisterDirect<PropertyItem, PropertyItemValue>(
                    nameof(PropertyValue),
                    o => o.PropertyValue/*, defaultBindingMode: Data.BindingMode.TwoWay*/);

        private PropertyItemValue _propertyValue;

        /// <summary>
        /// Получает значение свойства.
        /// </summary>
        /// <value>The property value.</value>
        public PropertyItemValue PropertyValue
        {
            get
            {
                if (_propertyValue == null)
                    PropertyValue = CreatePropertyValueInstance();
                return _propertyValue;
            }
            private set { SetAndRaise(PropertyValueProperty, ref _propertyValue, value); }
        }

        /// <summary>
        /// Получает экземпляр PropertyDescriptor для базового свойства.
        /// </summary>
        public PropertyDescriptor PropertyDescriptor
        {
            get { return _descriptor; }
        }

        public static readonly DirectProperty<PropertyItem, string> DisplayNameProperty =
                AvaloniaProperty.RegisterDirect<PropertyItem, string>(
                    nameof(DisplayName),
                    o => o.DisplayName, (o, v) => o.DisplayName=v,defaultBindingMode: BindingMode.TwoWay);

        private string _displayName;

        /// <summary>
        /// Возвращает отображаемое имя свойства.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The display name for the property.
        /// </returns>
        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(_displayName))
                    DisplayName = GetDisplayName();
                return _displayName;
            }

            set
            {
                if (_displayName == value)
                    return;
                SetAndRaise(DisplayNameProperty, ref _displayName, value);
            }
        }

        private readonly string _categoryName;

        /// <summary>
        /// Получает имя категории, в которой находится это свойство.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The name of the category that this property resides in.
        /// </returns>
        public string CategoryName
        {
            get { return _categoryName; }
        }

        public static readonly DirectProperty<PropertyItem, string> DescriptionProperty =
                AvaloniaProperty.RegisterDirect<PropertyItem, string>(
                    nameof(Description),
                    o => o.Description, (o, v) => o.Description = v, defaultBindingMode: BindingMode.TwoWay);

        private string _description;

        /// <summary>
        /// Получает описание инкапсулированного свойства.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The description of the encapsulated property.
        /// </returns>
        public string Description
        {
            get { return _description; }
            set
            {
                if (_description == value)
                    return;
                SetAndRaise(DescriptionProperty, ref _description, value);
            }
        }

        /// <summary>
        /// Возвращает значение, указывающее, является ли инкапсулированное свойство расширенным свойством.
        /// </summary>
        /// <returns>true if the encapsulated property is an advanced property; otherwise, false.</returns>
        // TODO: move intilialization to ctor
        public bool IsAdvanced
        {
            get
            {
                var browsable = (EditorBrowsableAttribute)Attributes[typeof(EditorBrowsableAttribute)];
                return browsable != null && browsable.State == EditorBrowsableState.Advanced;
            }
        }

        private readonly bool _isLocalizable;

        /// <summary>
        /// Возвращает значение, указывающее, является ли инкапсулируемое свойство локализуемым.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is localizable; otherwise, <c>false</c>.
        /// </value>
        public bool IsLocalizable
        {
            get { return _isLocalizable; }
        }

        public static readonly DirectProperty<PropertyItem, bool> IsReadOnlyProperty =
                AvaloniaProperty.RegisterDirect<PropertyItem, bool>(
                    nameof(IsReadOnly),
                    o => o.IsReadOnly, (o, v) => o.IsReadOnly=v, unsetValue: false, defaultBindingMode: BindingMode.TwoWay);

        private bool _isReadOnly;

        /// <summary>
        /// Возвращает значение, указывающее, доступно ли инкапсулированное свойство только для чтения.
        /// </summary>
        /// <value></value>
        /// <returns>true if the encapsulated property is read-only; otherwise, false.
        /// </returns>
        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set
            {
                if (_isReadOnly == value)
                    return;

                SetAndRaise(IsReadOnlyProperty, ref _isReadOnly, value);
            }
        }

        /// <summary>
        /// Возвращает тип инкапсулированного свойства.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The type of the encapsulated property.
        /// </returns>
        public virtual Type PropertyType
        {
            get
            {
                if (_descriptor == null)
                    return null;
                return _descriptor.PropertyType;
            }
        }

        /// <summary>
        /// Возвращает стандартные значения, которые поддерживает инкапсулированное свойство.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// A <see cref="T:System.Collections.ICollection"/> of standard values that the encapsulated property supports.
        /// </returns>
        public ICollection StandardValues
        {
            get
            {
                if (Converter.GetStandardValuesSupported())
                    return Converter.GetStandardValues();

                return new ArrayList(0);
            }
        }

        /// <summary>
        /// Возвращает компонент, которому принадлежит свойство.
        /// </summary>
        /// <value>The component.</value>
        public object Component
        {
            get { return _component; }
        }

        /// <summary>
        /// Возвращает компонент, которому принадлежит свойство.
        /// </summary>
        /// <remarks>
        /// Это свойство возвращает реальный развернутый компонент, даже если используется пользовательский дескриптор типа.
        /// </remarks>
        public object UnwrappedComponent
        {
            get { return _unwrappedComponent ?? (_unwrappedComponent = ObjectServices.GetUnwrappedObject(_component)); }
        }

        /// <summary>
        /// Получает всплывающую подсказку.
        /// </summary>
        /// <value>The tool tip.</value>
        public object ToolTip
        {
            get
            {
                var attribute = GetAttribute<DescriptionAttribute>();
                return (attribute != null && !string.IsNullOrEmpty(attribute.Description))
                  ? attribute.Description
                  : DisplayName;
            }
        }

        /// <summary>
        /// Возвращает пользовательские атрибуты, привязанные к свойству.
        /// </summary>
        /// <value>The attributes.</value>
        public virtual AttributeCollection Attributes
        {
            get
            {
                if (_descriptor == null)
                    return null;
                return _descriptor.Attributes;
            }
        }

        /// <summary>
        /// Возвращает контейнер пользовательских атрибутов.
        /// </summary>
        /// <value>The custom attributes container.</value>
        public AttributesContainer Metadata
        {
            get { return _metadata; }
        }

        /// <summary>
        /// Получает конвертер.
        /// </summary>
        /// <value>The converter.</value>
        public TypeConverter Converter
        {
            get { return ObjectServices.GetPropertyConverter(_descriptor); }
        }

        /// <summary>
        /// Получает значение, указывающее, может ли этот экземпляр очистить значение.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can clear value; otherwise, <c>false</c>.
        /// </value>
        public bool CanClearValue
        {
            get { return _descriptor.CanResetValue(_component); }
        }

        // TODO: support this (UI should also react on it)
        /// <summary>
        /// Возвращает значение, указывающее, является ли данный экземпляр значением по умолчанию.
        /// <remarks>Это свойство зарезервировано для будущих реализаций.</remarks>
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is default value; otherwise, <c>false</c>.
        /// </value>
        public bool IsDefaultValue
        {
            get { return true; }
        }

        /// <summary>
        /// Получает значение, указывающее, является ли этот экземпляр коллекцией.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is collection; otherwise, <c>false</c>.
        /// </value>
        public bool IsCollection
        {
            get { return typeof(IList).IsAssignableFrom(PropertyType); }
        }

        /// <summary>
        /// Возникает при изменении значения свойства.
        /// </summary>
        public event Action<PropertyItem, object, object> ValueChanged;
    }
}