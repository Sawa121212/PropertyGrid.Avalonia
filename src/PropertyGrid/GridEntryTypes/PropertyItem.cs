using System.Collections;
using System.ComponentModel;
using PropertyGrid.Metadata;
using PropertyGrid.PropertyEditing.Filters;
using PropertyGrid.PropertyTypes;

namespace PropertyGrid.GridEntryTypes
{
    /// <summary>
    /// Defines a wrapper around object property to be used at presentation level.
    /// </summary>
    
    public partial class PropertyItem : GridEntry, IPropertyFilterTarget
    {
        /// <summary>
        /// Применяет фильтр для записи.
        /// </summary>
        /// <param name="filter">The filter.</param>
        public override void ApplyFilter(PropertyFilter filter)
        {
            MatchesFilter = (filter == null) || filter.Match(this);
            OnFilterApplied(filter);
        }

        /// <summary>
        /// Проверяет, соответствует ли запись предикату фильтрации.
        /// </summary>
        /// <param name="predicate">The filtering predicate.</param>
        /// <returns>
        /// 	<c>true</c> if entry matches predicate; otherwise, <c>false</c>.
        /// </returns>
        public override bool MatchesPredicate(PropertyFilterPredicate predicate)
        {
            if (predicate == null)
                return false;
            if (!predicate.Match(DisplayName))
            {
                return (PropertyType != null)
                  ? predicate.Match(PropertyType.Name)
                  : false;
            }
            return true;
        }

        /// <summary>
        /// Создает экземпляр значения свойства.
        /// </summary>
        /// <returns>A new instance of <see cref="PropertyItemValue"/>.</returns>
        protected PropertyItemValue CreatePropertyValueInstance()
        {
            return new PropertyItemValue(this);
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса PropertyItem
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="component">The component property belongs to.</param>
        /// <param name="descriptor">The property descriptor</param>
        public PropertyItem(PropertyGrid owner, object component, PropertyDescriptor descriptor)
          : this(null)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");
            if (component == null)
                throw new ArgumentNullException("component");
            if (descriptor == null)
                throw new ArgumentNullException("descriptor");

            Owner = owner;
            Name = descriptor.Name;
            _component = component;
            _descriptor = descriptor;

            IsBrowsable = descriptor.IsBrowsable;
            IsReadOnly = descriptor.IsReadOnly;
            Description = descriptor.Description;
            _categoryName = descriptor.Category;
            _isLocalizable = descriptor.IsLocalizable;

            _metadata = new AttributesContainer(descriptor.Attributes);
            _descriptor.AddValueChanged(component, ComponentValueChanged);

            FilterApplied += (o, e) =>
            {
                RaisePropertyChanged(IsBrowsableProperty, !IsBrowsable, IsBrowsable);
                RaisePropertyChanged(MatchesFilterProperty, !MatchesFilter, MatchesFilter);
            };
            //BrowsableChanged += (o, e) =>
            //{
                
            //    //RaisePropertyChanged(IsBrowsableProperty, !IsBrowsable, IsBrowsable);
            //    //RaisePropertyChanged(MatchesFilterProperty, !MatchesFilter, MatchesFilter);
            //};

        }

        /// <summary>
        /// Инициализирует новый экземпляр класса PropertyItem 
        /// </summary>
        /// <param name="parentValue">The parent value.</param>
        protected PropertyItem(PropertyItemValue parentValue)
        {
            _parentValue = parentValue;
        }

        private void ComponentValueChanged(object sender, System.EventArgs e)
        {
            RaisePropertyChanged(PropertyValueProperty, null, PropertyValue);
        }

        private void OnValueChanged(object oldValue, object newValue)
        {
            Action<PropertyItem, object, object> handler = ValueChanged;
            if (handler != null)
                handler(this, oldValue, newValue);
        }

        /// <summary>
        /// Очищает значение.
        /// </summary>
        public void ClearValue()
        {
            if (!CanClearValue)
                return;

            var oldValue = GetValue();
            _descriptor.ResetValue(_component);
            OnValueChanged(oldValue, GetValue());
            RaisePropertyChanged(PropertyValueProperty, null, PropertyValue);
        }

        /// <summary>
        /// Получает значение.
        /// </summary>
        /// <returns>Property value</returns>
        public object GetValue()
        {
            if (_descriptor == null)
                return null;
            var target = GetViaCustomTypeDescriptor(_component, _descriptor);
            return _descriptor.GetValue(target);
        }

        private void SetValueCore(object value)
        {
            if (_descriptor == null)
                return;

            // Проверьте, проходит ли базовое свойство зависимости проверку
            if (!IsValidAvaloniaPropertyValue(_descriptor, value))
            {
                RaisePropertyChanged(PropertyValueProperty, null, PropertyValue);
                return;
            }

            var target = GetViaCustomTypeDescriptor(_component, _descriptor);

            if (target != null)
                _descriptor.SetValue(target, value);
        }

        /// <summary>
        /// Устанавливает значение.
        /// </summary>
        /// <param name="value">The value.</param>
        public void SetValue(object value)
        {
            // Check whether the property is not readonly
            if (IsReadOnly)
                return;

            var oldValue = GetValue();
            try
            {
                if (value != null && value.Equals(oldValue))
                    return;

                if (PropertyType == typeof(object) ||
                  value == null && PropertyType.IsClass ||
                  value != null && PropertyType.IsAssignableFrom(value.GetType()) || 
                  value != null && value is ICollection)
                {
                    SetValueCore(value);
                }
                else
                {
                    var convertedValue = Converter.ConvertFrom(value);
                    SetValueCore(convertedValue);
                }
                OnValueChanged(oldValue, GetValue());
            }
            catch
            {
                // TODO: Provide error feedback!
            }
            RaisePropertyChanged(PropertyValueProperty, null, PropertyValue);
        }

        /// <summary>
        /// Освобождает неуправляемые и - необязательно - управляемые ресурсы
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    _descriptor.RemoveValueChanged(_component, ComponentValueChanged);
                }
                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// Возвращает атрибут, привязанный к свойству.
        /// </summary>
        /// <typeparam name="T">Attribute type to look for</typeparam>
        /// <returns>Attribute bound to property or null.</returns>
        public virtual T GetAttribute<T>() where T : Attribute
        {
            if (Attributes == null)
                return null;
            return Attributes[typeof(T)] as T;
        }

        private static object GetViaCustomTypeDescriptor(object obj, PropertyDescriptor descriptor)
        {
            var customTypeDescriptor = obj as ICustomTypeDescriptor;
            return customTypeDescriptor != null ? customTypeDescriptor.GetPropertyOwner(descriptor) : obj;
        }

        /// <summary>
        /// Проверяет указанное значение.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// 	<c>true</c> if value can be applied for the property; otherwise, <c>false</c>.
        /// </returns>
        public bool Validate(object value)
        {
            return IsValidAvaloniaPropertyValue(_descriptor, value);
        }

        private bool IsValidAvaloniaPropertyValue(PropertyDescriptor descriptor, object value)
        {
            bool result = true;

            var desciptor= TypeDescriptor.GetProperties(this).OfType<PropertyDescriptor>().
                FirstOrDefault(x => x.Name == descriptor.Name && x.PropertyType == descriptor.PropertyType);

            if(descriptor!=null)
            {
              return descriptor.Converter.IsValid(value);
            }

            return result;
        }

        private string GetDisplayName()
        {
            // TODO: decide what to be returned in the worst case (no descriptor)
            if (_descriptor == null)
                return null;

            // Try getting Parenthesize attribute
            var attr = GetAttribute<ParenthesizePropertyNameAttribute>();

            // if property needs parenthesizing then apply parenthesis to resulting display name
            return (attr != null && attr.NeedParenthesis)
              ? "(" + _descriptor.DisplayName + ")"
              : _descriptor.DisplayName;
        }

        //public void SetPropertySouce(object source)
        //{
        //  if (source == null) throw new ArgumentNullException("source");

        //  this.component = source;

        //  if (_Value != null)
        //  {
        //    _Value = null;
        //    OnPropertyChanged("PropertyValue");
        //  }
        //}
    }
}