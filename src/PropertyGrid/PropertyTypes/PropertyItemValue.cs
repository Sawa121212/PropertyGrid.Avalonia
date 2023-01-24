using System.Collections;
using System.ComponentModel;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Common.Core;
using Prism.Commands;
using Prism.Mvvm;
using PropertyGrid.CollectionControl;
using PropertyGrid.Editors;
using PropertyGrid.GridEntryTypes;
using PropertyGrid.PropertyEditing;
using PropertyGrid.Utils;

namespace PropertyGrid.PropertyTypes
{
    /// <summary>
    /// Provides a wrapper around property value to be used at presentation level.
    /// </summary>
    public partial class PropertyItemValue : AvaloniaObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyItemValue"/>.
        /// </summary>
        /// <param name="property">The property.</param>
        public PropertyItemValue(PropertyItem property)
        {
            if (property == null)
                throw new ArgumentNullException("property");
            ParentProperty = property;

            HasSubProperties = property.Converter.GetPropertiesSupported();

            if (HasSubProperties)
            {
                object value = property.GetValue();

                PropertyDescriptorCollection descriptors = property.Converter.GetProperties(value);
                foreach (PropertyDescriptor d in descriptors)
                {
                    SubProperties.Add(new PropertyItem(property.Owner, value, d));
                    // TODO: Move to PropertyData as a public property
                    NotifyParentPropertyAttribute notifyParent =
                        d.Attributes[KnownTypes.Attributes.NotifyParentPropertyAttribute] as
                            NotifyParentPropertyAttribute;
                    if (notifyParent != null && notifyParent.NotifyParent)
                    {
                        d.AddValueChanged(value, NotifySubPropertyChanged);
                    }
                }
            }


            ButtonCommand = new DelegateCommand<object>(async (e) => await OnButtonClickShowProperties(e));

            ParentProperty.PropertyChanged += ParentPropertyChanged;
        }

        private async Task OnButtonClickShowProperties(object obj)
        {
            if (obj is IList collection)
            {
                var result =
                    await ShowDialog<CollectionControlView, IList, IList>(new CollectionControlView(), collection);
                if (result != null && !result.Equals(obj))
                {
                    collection = result;
                }
            }
        }

        //public async Task<bool?> ShowDialog(Window window)
        private async Task<TResult> ShowDialog<TWindow, TResult, TParam>(Window view, TParam param,
            bool canMinimize = false)
            where TWindow : Window, IViewWithResult<TResult>
        {
            if (!(view.DataContext is BindableBase viewModel)) // ObservableViewModelBase
                throw new InvalidOperationException("ViewModel must implement BindableBase");

            if (!(view.DataContext is IInitializable<TParam> init))
                throw new InvalidOperationException("ViewModel must implement IInitializable<TParam>");
            init.Initialize(param);

            if (view.DataContext is IInitializable initializable)
                initializable.Initialize();

            // Set Owner
            if (Application.Current is
                {
                    ApplicationLifetime: IClassicDesktopStyleApplicationLifetime
                    desktopStyleApplicationLifetime
                })
            {
                var shell = desktopStyleApplicationLifetime.MainWindow;
                FindActiveOwner(shell, view);
                shell.Activate();

                await view.ShowDialog<TResult>(shell);
                var result = view as IViewWithResult<TResult>;
                if (result != null)
                {
                    var e = result.Result;
                    return e != null ? e : default(TResult);
                }
            }

            return default(TResult);
        }

        /// <summary>
        /// Рекурсивный поиск активного окна и установка его как Owner.
        /// </summary>
        /// <param name="possibleOwner"></param>
        /// <param name="childWindow"></param>
        /// <returns></returns>
        private bool FindActiveOwner(Window possibleOwner, Window childWindow)
        {
            if (possibleOwner != null)
            {
                if (possibleOwner.IsActive) //Если possibleOwner активный
                {
                    if (possibleOwner.IsVisible)
                    {
                        //childWindow.Owner = possibleOwner;
                        return true;
                    }
                }

                //Поиск активного дочернего окна.
                foreach (Window ownedWindow in possibleOwner.OwnedWindows)
                {
                    if (FindActiveOwner(ownedWindow, childWindow))
                        return true;
                }

                // Если нет активных дочерних окон, то пусть будет главное окно.
                if (possibleOwner.IsVisible)
                {
                    //childWindow.Owner = possibleOwner;
                    return true;
                }
            }

            return false;
        }

        private void ParentPropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property.Name == nameof(PropertyItem.PropertyValue))
                NotifyRootValueChanged();

            if (e.Property.Name == nameof(PropertyItem.IsReadOnly))
            {
                RaisePropertyChanged(PropertyItem.IsReadOnlyProperty,
                    !ParentProperty.IsReadOnly, ParentProperty.IsReadOnly);

                RaisePropertyChanged(IsEditableProperty, !IsEditable, IsEditable);
            }
        }

        /// <summary>
        /// Clears the value.
        /// </summary>
        public void ClearValue()
        {
            _parentProperty.ClearValue();
        }

        /// <summary>
        /// Converts the string to value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Value instance</returns>
        protected object ConvertStringToValue(string value)
        {
            if (_parentProperty.PropertyType == typeof(string))
                return value;
            //if (value.Length == 0) return null;
            if (string.IsNullOrEmpty(value))
                return null;
            if (!_parentProperty.Converter.CanConvertFrom(typeof(string)))
                throw new InvalidOperationException("Value to String conversion is not supported!");
            return _parentProperty.Converter.ConvertFromString(null, GetSerializationCulture(), value);
        }

        /// <summary>
        /// Converts the value to string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>String presentation of the value</returns>
        protected string ConvertValueToString(object value)
        {
            string collectionValue = string.Empty;
            if (value == null)
                return collectionValue;

            collectionValue = value as String;
            if (collectionValue != null)
                return collectionValue;

            var converter = _parentProperty.Converter;
            if (converter.CanConvertTo(typeof(string)))
                collectionValue = converter.ConvertToString(null, GetSerializationCulture(), value);
            else
                collectionValue = value.ToString();

            // TODO: refer to resources or some constant
            if (string.IsNullOrEmpty(collectionValue) && (value is IEnumerable))
                collectionValue = "(Collection)";

            return collectionValue;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns>Property value</returns>
        protected object GetValueCore()
        {
            return _parentProperty.GetValue();
        }

        /// <summary>
        /// Gets the serialization culture.
        /// </summary>
        /// <returns>Culture to serialize value.</returns>
        protected virtual CultureInfo GetSerializationCulture()
        {
            return ObjectServices.GetSerializationCulture(_parentProperty.PropertyType);
        }

        /// <summary>
        /// Notifies the root value changed.
        /// </summary>
        protected virtual void NotifyRootValueChanged()
        {
            RaisePropertyChanged(IsDefaultValueProperty, !IsDefaultValue, IsDefaultValue);

            //does not exist
            //OnPropertyChanged("IsMixedValue");

            RaisePropertyChanged(IsCollectionProperty, !IsCollection, IsCollection);

            //does not exist
            //OnPropertyChanged("Collection");

            RaisePropertyChanged(HasSubPropertiesProperty, !HasSubProperties, HasSubProperties);

            RaisePropertyChanged(SubPropertiesProperty, null, SubProperties);

            //does not exist
            //OnPropertyChanged("Source");

            RaisePropertyChanged(CanConvertFromStringProperty, !CanConvertFromString, CanConvertFromString);

            NotifyValueChanged();
            OnRootValueChanged();
        }

        private void NotifyStringValueChanged()
        {
            RaisePropertyChanged(StringValueProperty, null, StringValue);
        }

        /// <summary>
        /// Notifies the sub property changed.
        /// </summary>
        protected void NotifySubPropertyChanged(object sender, System.EventArgs args)
        {
            NotifyValueChanged();
            OnSubPropertyChanged();
        }

        private void NotifyValueChanged()
        {
            RaisePropertyChanged(ValueProperty, null, Value);
            NotifyStringValueChanged();
        }

        private void OnRootValueChanged()
        {
            var handler = RootValueChanged;
            if (handler != null)
                handler(this, System.EventArgs.Empty);
        }

        private void OnSubPropertyChanged()
        {
            var handler = SubPropertyChanged;
            if (handler != null)
                handler(this, System.EventArgs.Empty);
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="value">The value.</param>
        protected void SetValueCore(object value)
        {
            _parentProperty.SetValue(value);
        }

        // TODO: AvaloniaProperty validation should be placed here
        /// <summary>
        /// Validates the value.
        /// </summary>
        /// <param name="valueToValidate">The value to validate.</param>
        protected void ValidateValue(object valueToValidate)
        {
            //throw new NotImplementedException();
            // Do nothing
        }

        private void SetValueImpl(object value)
        {
            //this.ValidateValue(value);
            if (ParentProperty.Validate(value))
                SetValueCore(value);

            NotifyValueChanged();
            OnRootValueChanged();
        }

        /// <summary>
        /// Raises the <see cref="PropertyValueException"/> event.
        /// </summary>
        /// <param name="e">The <see cref="ValueExceptionEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPropertyValueException(ValueExceptionEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e");
            if (PropertyValueException != null)
                PropertyValueException(this, e);
        }

        /// <summary>
        /// Gets a value indicating whether exceptions should be cought.
        /// </summary>
        /// <value><c>true</c> if expceptions should be cought; otherwise, <c>false</c>.</value>
        protected virtual bool CatchExceptions
        {
            get { return (PropertyValueException != null); }
        }
    }
}