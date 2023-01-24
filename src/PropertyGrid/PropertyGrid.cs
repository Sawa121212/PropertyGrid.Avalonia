using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using PropertyGrid.Controls;
using PropertyGrid.Design;
using PropertyGrid.Editors;
using PropertyGrid.GridEntryTypes;
using PropertyGrid.Metadata;
using PropertyGrid.PropertyEditing.Filters;
using PropertyGrid.PropertyTypes;
using PropertyGrid.Utils;
using VisualExtensions = Avalonia.VisualTree.VisualExtensions;

namespace PropertyGrid
{
    /// <summary>
    /// Порт wpf.ExtendedToolkit под Avalonia
    /// </summary>
    public partial class PropertyGrid : TemplatedControl
    {
        public PropertyGrid()
        {
            GotFocus += (o, e) => { ShowDescription(o, e); };

            // По умолчанию назначьте Layout в CategorizedLayout
            Layout = new CategorizedLayout();
            //Layout = new AlphabeticalLayout();

            // Проводные командные привязки
            InitializeCommandBindings();

            CurrentDescriptionProperty.Changed.AddClassHandler<PropertyGrid>(
                (o, e) => OnCurrentDescriptionChanged(o, e));
            LayoutProperty.Changed.AddClassHandler<PropertyGrid>(
                (o, e) => OnLayoutChanged(o, e));
            ShowReadOnlyPropertiesProperty.Changed.AddClassHandler<PropertyGrid>(
                (o, e) => OnShowReadOnlyPropertiesChanged(o, e));
            ShowAttachedPropertiesProperty.Changed.AddClassHandler<PropertyGrid>(
                (o, e) => OnShowAttachedPropertiesChanged(o, e));
            PropertyFilterProperty.Changed.AddClassHandler<PropertyGrid>(
                (o, e) => OnPropertyFilterChanged(o, e));
            PropertyDisplayModeProperty.Changed.AddClassHandler<PropertyGrid>(
                (o, e) => OnPropertyDisplayModePropertyChanged(o, e));

            SelectedObjectProperty.Changed.AddClassHandler<PropertyGrid>(
                (o, e) => OnPropertySelectedObjectPropertyChanged(o, e));

            /*SelectedObjectProperty.Changed.Subscribe(x =>  OnPropertySelectedObjectPropertyChanged(x.Sender, x));*/
        }


        private void RaisePropertyValueChangedEvent(PropertyItem property, object oldValue)
        {
            var args = new PropertyValueChangedEventArgs(PropertyValueChangedEvent, property, oldValue);
            RaiseEvent(args);
        }

        // internal static void RaisePreparePropertyItemEvent(Control source, PropertyItemBase propertyItemBase,
        //     object item)
        // {
        //     source.RaiseEvent(new PropertyItemEventArgs());
        // }

        internal CategoryItem CreateCategory(CategoryAttribute attribute)
        {
            // Проверьте передаваемый аргумент атрибута
            Debug.Assert(attribute != null);
            if (attribute == null)
                return null;

            // Проверьте ограничения для просмотра
            //if (!ShouldDisplayCategory(attribute.Category)) return null;

            // Создать новый CategoryItem
            var categoryItem = new CategoryItem(this, attribute);
            categoryItem.Editor = GetEditor(categoryItem);
            categoryItem.IsBrowsable = ShouldDisplayCategory(categoryItem.Name);

            // Вернуть результирующий элемент
            return categoryItem;
        }

        private PropertyItem CreatePropertyItem(PropertyDescriptor descriptor)
        {
            // Проверьте ограничения для просмотра
            //if (!ShoudDisplayProperty(descriptor)) return null;

            //DependencyPropertyDescriptor.FromProperty(descriptor);
            var dpDescriptor = TypeDescriptor.GetProperties(SelectedObject).OfType<PropertyDescriptor>()
                .FirstOrDefault(x => x.Name == descriptor.Name && x.PropertyType == descriptor.PropertyType);

            // Обеспечить дополнительные проверки свойств зависимостей
            if (dpDescriptor != null)
            {
                // Проверьте, не запрещены ли свойства зависимостей
                if (PropertyDisplayMode == PropertyDisplayMode.Native)
                    return null;

                // Проверьте, должны ли отображаться прикрепленные свойства

                //if (dpDescriptor.IsAttached && !ShowAttachedProperties)
                //    return null;
            }
            else
            {
                if (PropertyDisplayMode == PropertyDisplayMode.Dependency)
                    return null;
            }

            // Проверьте, должны ли отображаться свойства, доступные только для чтения
            if (descriptor.IsReadOnly && !ShowReadOnlyProperties)
                return null;

            // Note: superceded by ShouldDisplayProperty method call
            // Проверьте, доступно ли свойство для просмотра, и добавьте его в коллекцию
            // if (!descriptor.IsBrowsable) return null;

            //PropertyItem item = new PropertyItem(this, this.SelectedObject, descriptor);

            var item = new PropertyItem(this, SelectedObject, descriptor);

            //item.OverrideIsBrowsable(new bool?(ShoudDisplayProperty(descriptor)));
            item.IsBrowsable = ShouldDisplayProperty(descriptor);

            return item;
        }

        private bool ShouldDisplayProperty(PropertyDescriptor propertyDescriptor)
        {
            Debug.Assert(propertyDescriptor != null);
            if (propertyDescriptor == null)
                return false;

            // Проверьте, не ограничена ли категория владения выводом
            var showWithinCategory = ShouldDisplayCategory(propertyDescriptor.Category);
            if (!showWithinCategory)
                return false;

            if (propertyDescriptor.DisplayName == "Changing" || propertyDescriptor.DisplayName == "Changed" ||
                propertyDescriptor.DisplayName == "ThrownExceptions")
            {
                return false;
            }

            // Проверьте явное объявление
            var attribute = browsableProperties.FirstOrDefault(item => item.PropertyName == propertyDescriptor.Name);
            if (attribute != null)
                return attribute.Browsable;

            // Проверьте подстановочный знак
            var wildcard =
                browsableProperties.FirstOrDefault(item => item.PropertyName == BrowsablePropertyAttribute.All);
            if (wildcard != null)
                return wildcard.Browsable;

            // Возвращает настройки по умолчанию/стандартные доступные для просмотра для свойства
            return propertyDescriptor.IsBrowsable;
        }

        private bool ShouldDisplayCategory(string categoryName)
        {
            if (string.IsNullOrEmpty(categoryName))
                return false;

            // Проверьте явное объявление
            var attribute = browsableCategories.FirstOrDefault(item => item.CategoryName == categoryName);
            if (attribute != null)
                return attribute.Browsable;

            // Проверьте подстановочный знак
            var wildcard =
                browsableCategories.FirstOrDefault(item => item.CategoryName == BrowsableCategoryAttribute.All);
            if (wildcard != null)
                return wildcard.Browsable;

            // Разрешить по умолчанию, если не было применено никаких ограничений
            return true;
        }

        /// <summary>
        /// Возвращает редактор для записи в таблице.
        /// </summary>
        /// <param name="entry">The entry to look the editor for.</param>
        /// <returns>Editor for the entry</returns>
        public virtual Editor GetEditor(GridEntry entry)
        {
            var property = entry as PropertyItem;
            if (property != null)
                return Editors.GetEditor(property);

            var category = entry as CategoryItem;
            if (category != null)
                return Editors.GetEditor(category);

            return null;
        }

        private void SwapSelectedObject(object value)
        {
            //foreach (PropertyItem property in this.Properties)
            //{
            //  property.SetPropertySouce(value);
            //}
            DoReload();
        }

        private IEnumerable<CategoryItem> CollectCategories(IEnumerable<PropertyItem> properties)
        {
            var categories = new Dictionary<string, CategoryItem>();
            var refusedCategories = new HashSet<string>();

            foreach (var property in properties)
            {
                if (refusedCategories.Contains(property.CategoryName))
                    continue;
                CategoryItem category;

                if (categories.ContainsKey(property.CategoryName))
                {
                    category = categories[property.CategoryName];

                    if (category.Name.Contains("Account"))
                    {
                    }
                }
                else
                {
                    category = CreateCategory(property.GetAttribute<CategoryAttribute>());

                    if (category == null)
                    {
                        refusedCategories.Add(property.CategoryName);
                        continue;
                    }

                    categories[category.Name] = category;
                }

                category.AddProperty(property);
            }

            return categories.Values.ToList();
        }

        private IEnumerable<PropertyItem> CollectProperties(object[] components)
        {
            if (components == null || components.Length == 0)
                throw new ArgumentNullException("components");

            // TODO: PropertyItem is to be wired with PropertyData rather than pure PropertyDescriptor in the next version!
            var descriptors = (components.Length == 1)
                ? MetadataRepository.GetProperties(components[0]).Select(prop => prop.Descriptor)
                : ObjectServices.GetMergedProperties(components);

            IList<PropertyItem> propertyCollection = new List<PropertyItem>();

            foreach (var propertyDescriptor in descriptors)
            {
                var item = CreatePropertyItem(propertyDescriptor);
                if (item != null)
                    propertyCollection.Add(item);
            }

            return propertyCollection;
        }

        private static void VerifySelectedObjects(object[] value)
        {
            if (value != null && value.Length > 0)
            {
                // Ensure there are no nulls in the array
                for (var i = 0; i < value.Length; i++)
                {
                    if (value[i] == null)
                    {
                        var args = new object[]
                        {
                            i.ToString(CultureInfo.CurrentCulture), value.Length.ToString(CultureInfo.CurrentCulture)
                        };
                        // TODO: Move exception format to resources/settings!
                        throw new ArgumentNullException(string.Format(
                            "Item {0} in the 'objs' array is null. The array must begin with at least {1} members.",
                            args));
                    }
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Tab && e.Source is AvaloniaObject) //tabbing over the property editors
            {
                var source = e.Source as IControl;
                var element = e.KeyModifiers == KeyModifiers.Shift
                    ? GetTabElement(source, -1)
                    : GetTabElement(source, 1);
                if (element != null)
                {
                    element.Focus();
                    e.Handled = true;
                    return;
                }
            }

            base.OnKeyDown(e);
        }

        /// <summary>
        /// Возвращает элемент вкладки, на который можно поместить фокус.
        /// </summary>
        /// <remarks>
        /// Если элемент не включен, он не будет возвращен.
        /// </remarks>
        /// <param name="source">The source.</param>
        /// <param name="delta">The delta.</param>
        private IControl GetTabElement(IControl source, int delta)
        {
            if (source == null)
                return null;
            PropertyContainer container = null;
            if (source is SearchTextBox && HasCategories)
            {
                var itemspres = this.FindVisualChild<ItemsPresenter>();
                if (itemspres != null)
                {
                    var catcontainer = itemspres.FindVisualChild<CategoryContainer>();

                    if (catcontainer != null)
                    {
                        container = catcontainer.FindVisualChild<PropertyContainer>();
                    }
                }
            }
            else
                container = source.FindVisualParent<PropertyContainer>();

            var spanel = container.FindVisualParent<StackPanel>();
            if (spanel != null && spanel.Children.Contains(container))
            {
                var index = spanel.Children.IndexOf(container);
                if (delta > 0)
                    index = (index == spanel.Children.Count - 1) ? 0 : index + delta; //go back to the first after last
                else
                    index = (index == 0) ? spanel.Children.Count - 1 : index + delta; //go to last after first
                //loop inside the list
                if (index < 0)
                    index = spanel.Children.Count - 1;
                if (index >= spanel.Children.Count)
                    index = 0;

                var next = VisualExtensions.GetVisualChildren(spanel)
                    .ElementAt(index) as PropertyContainer; //  VisualTreeHelper.GetChild(spanel, index) as PropertyContainer;//this has always a Grid as visual child

                var grid = next.FindVisualChild<Grid>();
                if (grid != null && grid.Children.Count > 1)
                {
                    var pecp = grid.Children[1] as PropertyEditorContentPresenter;
                    var final = VisualExtensions.GetVisualChildren(pecp).ElementAt(0);
                    //VisualTreeHelper.GetChild(pecp, 0);
                    if ((final as Control).IsEnabled && (final as Control).Focusable &&
                        !(next.DataContext as PropertyItem).IsReadOnly)
                        return final as Control;
                    return GetTabElement(final as IControl, delta);
                }
            }

            return null;
        }

        private void OnPropertyDisplayModePropertyChanged(PropertyGrid propertyGrid, AvaloniaPropertyChangedEventArgs e)
        {
            if (propertyGrid.SelectedObject == null)
                return;
            propertyGrid.DoReload();
        }

        private void OnPropertyFilterChanged(PropertyGrid propertyGrid, AvaloniaPropertyChangedEventArgs e)
        {
            if (propertyGrid.SelectedObject == null || !propertyGrid.HasCategories)
                return;

            foreach (var category in propertyGrid.Categories)
                category.ApplyFilter(new PropertyFilter(propertyGrid.PropertyFilter));
        }

        private void OnShowAttachedPropertiesChanged(PropertyGrid propertyGrid, AvaloniaPropertyChangedEventArgs e)
        {
            if (propertyGrid.SelectedObject == null)
                return;
            if (propertyGrid.HasCategories | propertyGrid.HasProperties)
                propertyGrid.DoReload();
        }

        private void OnShowReadOnlyPropertiesChanged(PropertyGrid propertyGrid, AvaloniaPropertyChangedEventArgs e)
        {
            // Проверьте, был ли выбран какой-либо объект
            if (propertyGrid.SelectedObject == null)
                return;

            // Проверьте, были ли созданы категории или свойства
            if (propertyGrid.HasCategories | propertyGrid.HasProperties)
                propertyGrid.DoReload();
        }

        private void OnLayoutChanged(PropertyGrid propertyGrid, AvaloniaPropertyChangedEventArgs e)
        {
            var layoutContainer = e.NewValue as TemplatedControl;
            if (layoutContainer != null)
            {
                layoutContainer.DataContext = propertyGrid;
            }
        }

        private void OnCurrentDescriptionChanged(PropertyGrid o, AvaloniaPropertyChangedEventArgs e)
        {
            o.OnCurrentDescriptionChanged(e);
        }

        /// <summary>
        /// Предоставляет производным классам возможность обрабатывать изменения в текущем свойстве описания.
        /// </summary>
        protected virtual void OnCurrentDescriptionChanged(AvaloniaPropertyChangedEventArgs e)
        {
        }

        private void ShowDescription(object sender, RoutedEventArgs e)
        {
            if (e.Source == null || !(e.Source is StyledElement) ||
                (e.Source as StyledElement).DataContext == null ||
                !((e.Source as StyledElement).DataContext is PropertyItemValue) ||
                ((e.Source as StyledElement).DataContext as PropertyItemValue).ParentProperty == null)
                return;
            var descri = ((e.Source as StyledElement).DataContext as PropertyItemValue).ParentProperty.ToolTip;
            CurrentDescription = descri == null ? "" : descri.ToString();
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            InvalidateMeasure();
            InvalidateArrange();
            InvalidateVisual();
            RaisePropertyChanged(LayoutProperty, null, new BindingValue<IControl>(Layout));
            DoReload();
        }


        internal void DoReload()
        {
            if (SelectedObject == null)
            {
                Categories = new GridEntryCollection<CategoryItem>();
                Properties = new GridEntryCollection<PropertyItem>();
            }
            else
            {
                // Сбор BrowsableCategoryAttribute, 
                var categoryAttributes = PropertyGridUtils.GetAttributes<BrowsableCategoryAttribute>(SelectedObject);
                browsableCategories = new List<BrowsableCategoryAttribute>(categoryAttributes);

                // Сбор BrowsablePropertyAttribute items
                var propertyAttributes = PropertyGridUtils.GetAttributes<BrowsablePropertyAttribute>(SelectedObject);
                browsableProperties = new List<BrowsablePropertyAttribute>(propertyAttributes);

                // Сбор категорий и свойств
                var properties = CollectProperties(new[] { SelectedObject });

                // TODO: This needs more elegant implementation
                var categories = new GridEntryCollection<CategoryItem>(CollectCategories(properties));
                if (Categories != null && Categories.Count > 0)
                    CopyCategoryFrom(Categories, categories);

                // Получение и применение заказов по категориям
                var categoryOrders = PropertyGridUtils.GetAttributes<CategoryOrderAttribute>(SelectedObject);
                foreach (var orderAttribute in categoryOrders)
                {
                    var category = categories[orderAttribute.Category];
                    // не применяйте Order, если он был применен ранее (Order равен нулю или больше),
                    // поэтому выигрывает первое обнаруженное значение Order для той же категории.
                    if (category != null && category.Order < 0)
                        category.Order = orderAttribute.Order;
                }

                Categories = categories; //new CategoryCollection(CollectCategories(properties));
                Properties = new GridEntryCollection<PropertyItem>(properties);
            }
        }

        private static void CopyCategoryFrom(GridEntryCollection<CategoryItem> oldValue,
            IEnumerable<CategoryItem> newValue)
        {
            foreach (var category in newValue)
            {
                var prev = oldValue[category.Name];
                if (prev == null)
                    continue;

                category.IsExpanded = prev.IsExpanded;
            }
        }

        private void OnPropertyItemValueChanged(PropertyItem property, object oldValue, object newValue)
        {
            RaisePropertyValueChangedEvent(property, oldValue);
        }

        private void HookPropertyChanged(PropertyItem item)
        {
            if (item == null)
                return;
            item.ValueChanged += OnPropertyItemValueChanged;
        }

        private void UnhookPropertyChanged(PropertyItem item)
        {
            if (item == null)
                return;
            item.ValueChanged -= OnPropertyItemValueChanged;
        }

        /// <summary>
        /// Called when selected objects were changed.
        /// </summary>
        protected virtual void OnSelectedObjectsChanged()
        {
            var handler = SelectedObjectsChanged;
            if (handler != null)
                handler(this, System.EventArgs.Empty);
        }
    }
}