using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia;
using PropertyGrid.PropertyEditing;
using PropertyGrid.PropertyEditing.Filters;

namespace PropertyGrid.GridEntryTypes
{
    /// <summary>
    /// Специальная запись в таблице, которая предоставляет информацию о категории Property и предоставляет доступ к базовым свойствам.
    /// </summary>
    
    public class CategoryItem : GridEntry
    {
        /// <summary>
        /// Возвращает или задает атрибут, с помощью которого была создана категория.
        /// </summary>
        /// <value>The attribute.</value>
        public Attribute Attribute { get; set; }

        public static readonly DirectProperty<CategoryItem, int> OrderProperty =
                AvaloniaProperty.RegisterDirect<CategoryItem, int>(
                    nameof(Order),
                    o => o.Order, unsetValue: -1);

        private int _order = -1;

        /// <summary>
        /// Возвращает или задает порядок категории.
        /// </summary>
        public int Order
        {
            get { return _order; }
            set
            {
                //if (_order == value)
                //    return;
                SetAndRaise(OrderProperty, ref _order, value);
            }
        }

        public static readonly DirectProperty<CategoryItem, bool> IsExpandedProperty =
                AvaloniaProperty.RegisterDirect<CategoryItem, bool>(
                    nameof(IsExpanded),
                    o => o.IsExpanded, unsetValue: false);

        private bool _isExpanded;

        /// <summary>
        /// Возвращает или задает значение, указывающее, расширена ли эта категория.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this category is expanded; otherwise, <c>false</c>.
        /// </value>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                //if (_isExpanded == value)
                //    return;
                SetAndRaise(IsExpandedProperty, ref _isExpanded, value);
            }
        }

        private readonly GridEntryCollection<PropertyItem> _properties = new GridEntryCollection<PropertyItem>();

        /// <summary>
        /// Получите все свойства в категории.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// An enumerable collection of all the properties in the category.
        /// </returns>
        public ReadOnlyObservableCollection<PropertyItem> Properties
        {
            get { return new ReadOnlyObservableCollection<PropertyItem>(_properties); }
        }

        /// <summary>
        /// Возвращает PropertyItem с указанным именем свойства.
        /// </summary>
        /// <value></value>
        public PropertyItem this[string propertyName]
        {
            get { return _properties[propertyName]; }
        }

        public static readonly DirectProperty<CategoryItem, IComparer<PropertyItem>> ComparerProperty =
                AvaloniaProperty.RegisterDirect<CategoryItem, IComparer<PropertyItem>>(
                    nameof(Comparer),
                    o => o.Comparer);

        private IComparer<PropertyItem> _comparer = new PropertyItemComparer();

        /// <summary>
        /// Возвращает или задает средство сравнения, используемое для сортировки свойств.
        /// </summary>
        /// <value>The comparer. </value>
        public IComparer<PropertyItem> Comparer
        {
            get { return _comparer; }
            set
            {
                //if (_comparer == value)
                //    return;
                SetAndRaise(ComparerProperty, ref _comparer, value);
                _properties.Sort(_comparer);
            }
        }

        public static readonly DirectProperty<CategoryItem, bool> HasVisiblePropertiesProperty =
                AvaloniaProperty.RegisterDirect<CategoryItem, bool>(
                    nameof(HasVisibleProperties),
                    o => o.HasVisibleProperties, unsetValue: true);

        private bool _hasVisibleProperties;

        /// <summary>
        /// Получает или задает значение, указывающее, имеет ли этот экземпляр видимые свойства.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has visible properties; otherwise, <c>false</c>.
        /// </value>
        public bool HasVisibleProperties
        {
            get { return _hasVisibleProperties; }
            private set
            {
                //if (_hasVisibleProperties == value)
                //    return;
                SetAndRaise(HasVisiblePropertiesProperty, ref _hasVisibleProperties, value);
            }
        }

        public static new readonly DirectProperty<CategoryItem, bool> IsVisibleProperty =
                AvaloniaProperty.RegisterDirect<CategoryItem, bool>(
                    nameof(IsVisible),
                    o => o.IsVisible, unsetValue: true);

        /// <summary>
        /// Возвращает значение, указывающее, должен ли этот экземпляр быть видимым.
        /// </summary>
        public new bool IsVisible
        {
            get { return base.IsVisible && HasVisibleProperties; }
        }

        //prop.IsBrowsable && prop.MatchesFilter

        /// <summary>
        /// Инициализирует новый экземпляр класса CategoryItem
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="name">The name.</param>
        public CategoryItem(PropertyGrid owner, string name)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            FilterApplied+= (o, e) =>
            {
                RaisePropertyChanged(IsBrowsableProperty, !IsBrowsable, IsBrowsable);
                RaisePropertyChanged(HasVisiblePropertiesProperty, !HasVisibleProperties, HasVisibleProperties);
                RaisePropertyChanged(MatchesFilterProperty, !MatchesFilter, MatchesFilter);
            };
            //this.BrowsableChanged += (o, e) =>
            //{
            //    //foreach(var item in _properties)
            //    //{
            //    //    //foreach(var att in item.Attributes.OfType<BrowsableAttribute>().ToList())
            //    //    //{
            //    //    //    att.Browsable = IsBrowsable;
            //    //    //}

            //    //}


            //    //RaisePropertyChanged(IsBrowsableProperty, !IsBrowsable, IsBrowsable);
            //    //RaisePropertyChanged(HasVisiblePropertiesProperty, !HasVisibleProperties, HasVisibleProperties);
            //    //RaisePropertyChanged(MatchesFilterProperty, !MatchesFilter, MatchesFilter);
            //};


            Owner = owner;
            Name = name;
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса CategoryItem
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="category">The category.</param>
        public CategoryItem(PropertyGrid owner, CategoryAttribute category)
          : this(owner, category.Category)
        {
            Attribute = category;
        }

        private static readonly Func<PropertyItem, bool> IsPropertyVisible = prop =>
                                                           prop.IsBrowsable && prop.MatchesFilter;

        /// <summary>
        /// Добавляет свойство.
        /// </summary>
        /// <param name="property">The property.</param>
        public void AddProperty(PropertyItem property)
        {
            if (property == null)
                throw new ArgumentNullException("property");
            if (_properties.Contains(property))
                throw new ArgumentException("Cannot add a duplicated property " + property.Name);

            int index = _properties.BinarySearch(property, _comparer);
            if (index < 0)
                index = ~index;

            _properties.Insert(index, property);

            if (property.IsBrowsable)
                HasVisibleProperties = true;
            else
                HasVisibleProperties = _properties.Any(IsPropertyVisible);

            property.BrowsableChanged += PropertyBrowsableChanged;
        }

        private void PropertyBrowsableChanged(object sender, System.EventArgs e)
        {
            HasVisibleProperties = _properties.Any(IsPropertyVisible);
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
            return _properties.All(property => property.MatchesPredicate(predicate));
        }

        /// <summary>
        /// Применяет фильтр для записи.
        /// </summary>
        /// <param name="filter">The filter.</param>
        public override void ApplyFilter(PropertyFilter filter)
        {
            bool propertiesMatch = false;
            foreach (var entry in Properties)
            {
                if (PropertyMatchesFilter(filter, entry))
                    propertiesMatch = true;
            }

            HasVisibleProperties = _properties.Any(IsPropertyVisible);
            MatchesFilter = propertiesMatch;

            if (propertiesMatch && !IsExpanded)
                IsExpanded = true;

            OnFilterApplied(filter);
        }

        private static bool PropertyMatchesFilter(PropertyFilter filter, PropertyItem entry)
        {
            entry.ApplyFilter(filter);
            return entry.MatchesFilter;
        }
    }
}