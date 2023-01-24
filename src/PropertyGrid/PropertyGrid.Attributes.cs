using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Media;
using PropertyGrid.Design;
using PropertyGrid.Editors;
using PropertyGrid.GridEntryTypes;
using PropertyGrid.Metadata;
using PropertyGrid.PropertyEditing;
using PropertyGrid.PropertyEditing.Filters;
using PropertyGrid.PropertyTypes;

namespace PropertyGrid
{
    public partial class PropertyGrid
    {
        public Type StyleKey => typeof(PropertyGrid);

        private static Attribute[] DefaultPropertiesFilter = new Attribute[]
        {
            new PropertyFilterAttribute(
                PropertyFilterOptions.SetValues
                | PropertyFilterOptions.UnsetValues
                | PropertyFilterOptions.Valid)
        };

        private List<BrowsablePropertyAttribute> browsableProperties = new List<BrowsablePropertyAttribute>();
        private List<BrowsableCategoryAttribute> browsableCategories = new List<BrowsableCategoryAttribute>();

        private EditorCollection _Editors = new EditorCollection();

        /// <summary>
        /// Gets the editors collection.
        /// </summary>
        /// <value>The editors collection.</value>
        public EditorCollection Editors => _Editors;

        public static readonly StyledProperty<bool> HasPropertiesProperty =
            AvaloniaProperty.Register<PropertyGrid, bool>(
                nameof(HasProperties),
                default);

        public bool HasProperties => _properties != null && _properties.Count > 0;

        /// <summary>
        /// Gets a value indicating whether this instance has properties.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has properties; otherwise, <c>false</c>.
        /// </value>
        //public bool HasProperties
        //{
        //    get { return _properties != null && _properties.Count > 0; }
        //}

        /// <summary>
        /// Gets a value indicating whether this instance has categories.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has categories; otherwise, <c>false</c>.
        /// </value>
        //public bool HasCategories
        //{
        //    get { return _categories != null && _categories.Count > 0; }
        //}
        public static readonly StyledProperty<bool> HasCategoriesProperty =
            AvaloniaProperty.Register<PropertyGrid, bool>(
                nameof(HasCategories),
                default);

        public bool HasCategories => _categories != null && _categories.Count > 0;

        public string CurrentDescription
        {
            get => GetValue(CurrentDescriptionProperty);
            set => SetValue(CurrentDescriptionProperty, value);
        }

        public static readonly StyledProperty<string> CurrentDescriptionProperty =
            AvaloniaProperty.Register<PropertyGrid, string>(nameof(CurrentDescription), defaultValue: string.Empty);

        /// <summary>
        /// Gets or sets the brush for items background.
        /// </summary>
        /// <value>The items background brush.</value>
        public Brush ItemsBackground
        {
            get => GetValue(ItemsBackgroundProperty);
            set => SetValue(ItemsBackgroundProperty, value);
        }

        public static readonly StyledProperty<Brush> ItemsBackgroundProperty =
            AvaloniaProperty.Register<PropertyGrid, Brush>(nameof(ItemsBackground));

        /// <summary>
        /// Gets or sets the items foreground brush.
        /// </summary>
        /// <value>The items foreground brush.</value>
        public IBrush ItemsForeground
        {
            get => GetValue(ItemsForegroundProperty);
            set => SetValue(ItemsForegroundProperty, value);
        }

        public static readonly StyledProperty<IBrush> ItemsForegroundProperty =
            AvaloniaProperty.Register<PropertyGrid, IBrush>(nameof(ItemsForeground));

        /// <summary>
        /// Gets or sets the layout to be used to display properties.
        /// </summary>
        /// <value>The layout to be used to display properties.</value>
        //[Content]
        public IControl Layout
        {
            get => GetValue(LayoutProperty);
            set => SetValue(LayoutProperty, value);
        }

        public static readonly StyledProperty<IControl> LayoutProperty =
            AvaloniaProperty.Register<PropertyGrid, IControl>(nameof(Layout),
                defaultValue: default(AlphabeticalLayout));

        /// <summary>
        /// Gets or sets the selected object.
        /// </summary>
        /// <value>The selected object.</value>
        public static readonly StyledProperty<object> SelectedObjectProperty =
            AvaloniaProperty.Register<PropertyGrid, object>(nameof(SelectedObject));

        /// <summary>
        /// Selected Object
        /// </summary>
        public object SelectedObject
        {
            get => GetValue(SelectedObjectProperty);
            set => SetValue(SelectedObjectProperty, value);
        }

        public static readonly DirectProperty<PropertyGrid, object[]> SelectedObjectsProperty =
            AvaloniaProperty.RegisterDirect<PropertyGrid, object[]>(
                nameof(SelectedObjects),
                o => o.SelectedObjects);

        private void OnPropertySelectedObjectPropertyChanged(PropertyGrid propertyGrid,
            AvaloniaPropertyChangedEventArgs avaloniaPropertyChangedEventArgs)
        {
            if (SelectedObject != null)
            {
                // OnCoerceSelectedObjectName();
                // OnSelectedObjectTypeChanged();
            }

            DoReload();
        }

        private object[] _currentObjects;

        public object[] SelectedObjects
        {
            get { return (_currentObjects == null) ? new object[0] : (object[])_currentObjects.Clone(); }
            set
            {
                object[] currentObjects = _currentObjects;
                // Ensure there are no nulls in the array
                VerifySelectedObjects(value);

                var sameSelection = false;

                // Check whether new selection is the same as was previously defined
                if (currentObjects != null && value != null && currentObjects.Length == value.Length)
                {
                    sameSelection = true;

                    for (var i = 0; i < value.Length && sameSelection; i++)
                    {
                        if (currentObjects[i] != value[i])
                            sameSelection = false;
                    }
                }

                if (!sameSelection)
                {
                    // Assign new objects and reload
                    if (value == null)
                    {
                        currentObjects = new object[0];
                        DoReload();
                    }
                    else
                    {
                        // process single selection
                        if (value.Length == 1 && currentObjects != null && currentObjects.Length == 1)
                        {
                            var oldValue = (currentObjects != null && currentObjects.Length > 0)
                                ? currentObjects[0]
                                : null;
                            var newValue = (value.Length > 0) ? value[0] : null;

                            currentObjects = (object[])value.Clone();

                            if (oldValue != null && newValue != null && oldValue.GetType().Equals(newValue.GetType()))
                                SwapSelectedObject(newValue);
                            else
                            {
                                DoReload();
                            }
                        }
                        // process multiple selection
                        else
                        {
                            currentObjects = (object[])value.Clone();
                            DoReload();
                        }
                    }

                    RaisePropertyChanged(SelectedObjectProperty, null, SelectedObject);

                    SetAndRaise(SelectedObjectsProperty, ref _currentObjects, currentObjects);

                    OnSelectedObjectsChanged();
                }
                else
                {
                    // TODO: Swap multiple objects here? Guess nothing can be done in this case...
                }
            }
        }

        #region SelectedObjectType Region

        public static readonly StyledProperty<Type> SelectedObjectTypeProperty =
            AvaloniaProperty.Register<PropertyGrid, Type>(nameof(SelectedObjectType));

        public Type SelectedObjectType
        {
            get => SelectedObject?.GetType();
            set => SetValue(SelectedObjectTypeProperty, value);
        }

        #endregion //SelectedObjectType

        #region SelectedObjectName Region

        public static readonly StyledProperty<string> SelectedObjectNameProperty =
            AvaloniaProperty.Register<PropertyGrid, string>(nameof(SelectedObjectName));

        /// <summary>
        /// Имя выбранного объекта
        /// </summary>
        public string SelectedObjectName
        {
            get => SelectedObject?.ToString();
            set => SetValue(SelectedObjectNameProperty, value);
        }

        #endregion //SelectedObjectName

        #region SelectedObjectTypeName Region

        public static readonly StyledProperty<string> SelectedObjectTypeNameProperty =
            AvaloniaProperty.Register<PropertyGrid, string>(nameof(SelectedObjectTypeName));

        public string SelectedObjectTypeName
        {
            get => GetValue(SelectedObjectTypeNameProperty);
            set => SetValue(SelectedObjectTypeNameProperty, value);
        }

        #endregion //SelectedObjectTypeName

        public static readonly StyledProperty<GridEntryCollection<PropertyItem>> PropertiesProperty =
            AvaloniaProperty.Register<PropertyGrid, GridEntryCollection<PropertyItem>>(nameof(Properties));

        private GridEntryCollection<PropertyItem> _properties;

        public GridEntryCollection<PropertyItem> Properties
        {
            get => GetValue(PropertiesProperty);
            private set
            {
                if (_properties == value)
                    return;

                //SetAndRaise(PropertiesProperty, ref _properties, value);
                SetValue(PropertiesProperty, value);

                if (_properties != null)
                {
                    foreach (var item in _properties)
                    {
                        UnhookPropertyChanged(item);
                        //item.Dispose();
                    }
                }

                if (value != null)
                {
                    SetAndRaise(PropertiesProperty, ref _properties, value);

                    if (PropertyComparer != null)
                        _properties.Sort(PropertyComparer);

                    foreach (var item in _properties)
                        HookPropertyChanged(item);
                }

                //OnPropertyChanged("Properties");

                RaisePropertyChanged(HasPropertiesProperty, !HasProperties, HasProperties);
                //OnPropertyChanged("HasProperties");

                RaisePropertyChanged(BrowsablePropertiesProperty, null,
                    new BindingValue<IEnumerable<PropertyItem>>(BrowsableProperties));
                //OnPropertyChanged("BrowsableProperties");
            }
        }

        public static readonly StyledProperty<IEnumerable<PropertyItem>> BrowsablePropertiesProperty =
            AvaloniaProperty.Register<PropertyGrid, IEnumerable<PropertyItem>>(
                nameof(BrowsableProperties),
                default);

        public IEnumerable<PropertyItem> BrowsableProperties
        {
            get
            {
                if (_properties != null)
                {
                    foreach (var property in _properties)
                        if (property.IsBrowsable)
                            yield return property;
                }
            }
        }

        public static readonly StyledProperty<IComparer<PropertyItem>> PropertyComparerProperty =
            AvaloniaProperty.Register<PropertyGrid, IComparer<PropertyItem>>(
                nameof(PropertyComparer),
                default);

        private IComparer<PropertyItem> _propertyComparer;

        public IComparer<PropertyItem> PropertyComparer
        {
            get => _propertyComparer ?? (_propertyComparer = new PropertyItemComparer());
            private set
            {
                if (_propertyComparer == value)
                    return;

                SetAndRaise(PropertyComparerProperty, ref _propertyComparer, value);

                if (_properties != null)
                    _properties.Sort(_propertyComparer);
            }
        }

        public static readonly StyledProperty<IComparer<CategoryItem>> CategoryComparerProperty =
            AvaloniaProperty.Register<PropertyGrid, IComparer<CategoryItem>>(
                nameof(CategoryComparer),
                default);

        private IComparer<CategoryItem> _categoryComparer;

        public IComparer<CategoryItem> CategoryComparer
        {
            get => GetValue(CategoryComparerProperty);
            private set
            {
                if (_categoryComparer == value)
                    return;

                SetAndRaise(CategoryComparerProperty, ref _categoryComparer, value);

                if (_categories != null)
                    _categories.Sort(_categoryComparer);
            }
        }

        public static readonly StyledProperty<GridEntryCollection<CategoryItem>> CategoriesProperty =
            AvaloniaProperty.Register<PropertyGrid, GridEntryCollection<CategoryItem>>(
                nameof(Categories),
                default);

        private GridEntryCollection<CategoryItem> _categories;

        public GridEntryCollection<CategoryItem> Categories
        {
            get => GetValue(CategoriesProperty);
            private set
            {
                if (_categories == value)
                    return;

                //SetAndRaise(CategoriesProperty, ref _categories, value);
                SetValue(CategoriesProperty, value);

                if (CategoryComparer != null)
                    _categories.Sort(CategoryComparer);

                RaisePropertyChanged(HasCategoriesProperty, !HasCategories, HasCategories);
                RaisePropertyChanged(BrowsablePropertiesProperty, null,
                    new BindingValue<IEnumerable<PropertyItem>>(BrowsableProperties));
                //        OnPropertyChanged("BrowsableCategories");
            }
        }


        public static readonly StyledProperty<IEnumerable<CategoryItem>> BrowsableCategoriesProperty =
            AvaloniaProperty.Register<PropertyGrid, IEnumerable<CategoryItem>>(
                nameof(BrowsableCategories),
                default);

        public IEnumerable<CategoryItem> BrowsableCategories
        {
            get
            {
                if (_categories != null)
                {
                    foreach (var category in _categories)
                        if (category.IsBrowsable)
                            yield return category;
                }
            }
        }

        /// <summary>
        /// Возвращает или задает значение, указывающее, должны ли отображаться свойства, доступные только для чтения.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if read-only properties should be displayed; otherwise,
        /// 	<c>false</c>. Default is <c>false</c>.
        /// </value>
        public bool ShowReadOnlyProperties
        {
            get => GetValue(ShowReadOnlyPropertiesProperty);
            set => SetValue(ShowReadOnlyPropertiesProperty, value);
        }

        public static readonly StyledProperty<bool> ShowReadOnlyPropertiesProperty =
            AvaloniaProperty.Register<PropertyGrid, bool>(nameof(ShowReadOnlyProperties));

        /// <summary>
        /// Возвращает или задает значение, указывающее, должны ли отображаться прикрепленные свойства.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if attached properties should be displayed; otherwise, <c>false</c>. Default is <c>false</c>.
        /// </value>
        public bool ShowAttachedProperties
        {
            get => GetValue(ShowAttachedPropertiesProperty);
            set => SetValue(ShowAttachedPropertiesProperty, value);
        }

        public static readonly StyledProperty<bool> ShowAttachedPropertiesProperty =
            AvaloniaProperty.Register<PropertyGrid, bool>(nameof(ShowAttachedProperties));

        /// <summary>
        /// Возвращает или устанавливает фильтр свойств.
        /// </summary>
        /// <value>The property filter.</value>
        public string PropertyFilter
        {
            get => GetValue(PropertyFilterProperty);
            set => SetValue(PropertyFilterProperty, value);
        }

        public static readonly StyledProperty<string> PropertyFilterProperty =
            AvaloniaProperty.Register<PropertyGrid, string>(nameof(PropertyFilter)
                , defaultValue: string.Empty
                , defaultBindingMode: BindingMode.TwoWay
            );

        /// <summary>
        /// Возвращает или задает состояние видимости фильтра свойств.
        /// </summary>
        /// <value>The property filter visibility state.</value>
        public bool PropertyFilterIsVisible
        {
            get => GetValue(PropertyFilterIsVisibleProperty);
            set => SetValue(PropertyFilterIsVisibleProperty, value);
        }

        public static readonly StyledProperty<bool> PropertyFilterIsVisibleProperty =
            AvaloniaProperty.Register<PropertyGrid, bool>(nameof(PropertyFilterIsVisible)
                , defaultValue: true);

        /// <summary>
        /// Возвращает или устанавливает режим отображения свойств.
        /// </summary>
        /// <value>The property display mode.</value>
        public PropertyDisplayMode PropertyDisplayMode
        {
            get => GetValue(PropertyDisplayModeProperty);
            set => SetValue(PropertyDisplayModeProperty, value);
        }

        public static readonly StyledProperty<PropertyDisplayMode> PropertyDisplayModeProperty =
            AvaloniaProperty.Register<PropertyGrid, PropertyDisplayMode>(nameof(PropertyDisplayMode)
                , defaultValue: PropertyDisplayMode.All);

        /// <summary>
        /// Возникает при изменении выбранных объектов.
        /// </summary>
        public event EventHandler SelectedObjectsChanged;

        public static readonly RoutedEvent<PropertyEditingEventArgs> PropertyEditingStartedEvent =
            RoutedEvent.Register<PropertyGrid, PropertyEditingEventArgs>
                (nameof(PropertyEditingStartedEvent), RoutingStrategies.Bubble);

        /// <summary>
        /// Возникает при запуске редактирования свойств.
        /// </summary>
        /// <remarks>
        /// Это событие предназначено для использования в сценариях настройки. Он не используется PropertyGrid control напрямую.
        /// </remarks>
        public event PropertyEditingEventHandler PropertyEditingStarted
        {
            add => AddHandler(PropertyEditingStartedEvent, value);
            remove => RemoveHandler(PropertyEditingStartedEvent, value);
        }

        public static readonly RoutedEvent<PropertyEditingEventArgs> PropertyEditingFinishedEvent =
            RoutedEvent.Register<PropertyGrid, PropertyEditingEventArgs>
                (nameof(PropertyEditingFinishedEvent), RoutingStrategies.Bubble);

        /// <summary>
        /// Происходит после завершения редактирования свойств.
        /// </summary>
        /// <remarks>
        /// Это событие предназначено для использования в сценариях настройки. Он не используется PropertyGrid control напрямую.
        /// </remarks>
        public event PropertyEditingEventHandler PropertyEditingFinished
        {
            add => AddHandler(PropertyEditingFinishedEvent, value);
            remove => RemoveHandler(PropertyEditingFinishedEvent, value);
        }

        public static readonly RoutedEvent<RoutedEventArgs> PropertyValueChangedEvent =
            RoutedEvent.Register<PropertyGrid, RoutedEventArgs>(nameof(PropertyValueChangedEvent),
                RoutingStrategies.Bubble);

        /// <summary>
        /// Возникает при изменении значения элемента свойства.
        /// </summary>
        public event EventHandler PropertyValueChanged
        {
            add => AddHandler(PropertyValueChangedEvent, value);
            remove => RemoveHandler(PropertyValueChangedEvent, value);
        }
    }
}