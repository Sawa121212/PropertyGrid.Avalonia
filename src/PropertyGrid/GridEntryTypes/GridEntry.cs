using Avalonia;
using PropertyGrid.Editors;
using PropertyGrid.PropertyEditing.Filters;

namespace PropertyGrid.GridEntryTypes
{
    public abstract class GridEntry : AvaloniaObject, IPropertyFilterTarget, IDisposable
    {
        /// <summary>
        /// Возвращает имя инкапсулированного элемента.
        /// </summary>
        public string Name { get; protected set; }

        public static readonly DirectProperty<GridEntry, bool> IsBrowsableProperty =
                AvaloniaProperty.RegisterDirect<GridEntry, bool>(
                    nameof(IsBrowsable),
                    o => o.IsBrowsable, unsetValue: true);

        private bool _isBrowsable;

        /// <summary>
        /// Получает или задает значение, указывающее, доступен ли этот экземпляр для просмотра.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is browsable; otherwise, <c>false</c>.
        /// </value>
        public bool IsBrowsable
        {
            get { return _isBrowsable; }
            set
            {
                SetAndRaise(IsBrowsableProperty, ref _isBrowsable, value);
                RaisePropertyChanged(IsVisibleProperty, !IsVisible, IsVisible);
                OnBrowsableChanged();
            }
        }

        public static readonly DirectProperty<GridEntry, bool> IsVisibleProperty =
                AvaloniaProperty.RegisterDirect<GridEntry, bool>(
                   nameof(IsVisible),
                    o => o.IsVisible, unsetValue: true);

        /// <summary>
        /// Возвращает значение, указывающее, должен ли этот экземпляр быть видимым.
        /// </summary>
        
        public bool IsVisible
        {
            get { return IsBrowsable && MatchesFilter; }
        }

        /// <summary>
        /// Получает или устанавливает владельца элемента.
        /// </summary>
        /// <value>The owner of the item.</value>
        public PropertyGrid Owner { get; protected set; }

        public static readonly DirectProperty<GridEntry, Editor> EditorProperty =
                AvaloniaProperty.RegisterDirect<GridEntry, Editor>(
                    nameof(Editor),
                    o => o.Editor);

        private Editor _editor;

        /// <summary>
        /// Получает или устанавливает редактор.
        /// </summary>
        /// <value>The editor.</value>
        public Editor Editor
        {
            get
            {
                if (_editor == null && Owner != null)
                    Editor = Owner.GetEditor(this);

                return _editor;
            }

            internal set { SetAndRaise(EditorProperty, ref _editor, value); }
        }

        private bool _disposed;

        /// <summary>
        /// Возвращает значение, указывающее, удален ли этот PropertyItem
        /// </summary>
        /// <value><c>true</c> if disposed; otherwise, <c>false</c>.</value>
        protected bool Disposed
        {
            get { return _disposed; }
        }

        /// <summary>
        /// Освобождает неуправляемые и - необязательно - управляемые ресурсы
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources;
        /// <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                }

                _disposed = true;
            }
        }

        /// <summary>
        /// Выполняет определенные приложением задачи, связанные с освобождением, деблокированием или сбросом неуправляемых ресурсов.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Освобождает неуправляемые ресурсы и выполняет другие операции очистки, прежде чем элемент свойства будет восстановлен сборкой мусора.
        /// </summary>
        ~GridEntry()
        {
            Dispose(false);
        }

        /// <summary>
        /// Возникает, когда для записи применяется фильтр.
        /// </summary>
        public event EventHandler<PropertyFilterAppliedEventArgs> FilterApplied;

        /// <summary>
        /// Вызывается, когда для записи был применен фильтр.
        /// </summary>
        /// <param name="filter">The filter.</param>
        protected virtual void OnFilterApplied(PropertyFilter filter)
        {
            var handler = FilterApplied;
            if (handler != null)
                handler(this, new PropertyFilterAppliedEventArgs(filter));
        }

        /// <summary>
        /// Применяет фильтр для записи.
        /// </summary>
        /// <param name="filter">The filter.</param>
        public abstract void ApplyFilter(PropertyFilter filter);

        /// <summary>
        /// Проверяет, соответствует ли запись предикату фильтрации.
        /// </summary>
        /// <param name="predicate">The filtering predicate.</param>
        /// <returns><c>true</c> if entry matches predicate; otherwise, <c>false</c>.</returns>
        public abstract bool MatchesPredicate(PropertyFilterPredicate predicate);

        public static readonly DirectProperty<GridEntry, bool> MatchesFilterProperty =
                AvaloniaProperty.RegisterDirect<GridEntry, bool>(
                    nameof(MatchesFilter),
                    o => o.MatchesFilter, unsetValue: true);

        private bool _matchesFilter = true;

        /// <summary>
        /// Возвращает или задает значение, указывающее, соответствует ли запись фильтру.
        /// </summary>
        /// <value><c>true</c> if entry matches filter; otherwise, <c>false</c>.</value>
        public bool MatchesFilter
        {
            get { return _matchesFilter; }
            protected set
            {
                //if (_matchesFilter == value)
                //    return;
                SetAndRaise(MatchesFilterProperty, ref _matchesFilter, value);

                RaisePropertyChanged(IsVisibleProperty, !IsVisible, IsVisible);
            }
        }

        /// <summary>
        /// Возникает при изменении состояния видимости свойства.
        /// </summary>
        public event EventHandler BrowsableChanged;

        /// <summary>
        /// Вызывается при изменении состояния видимости свойства.
        /// </summary>
        protected virtual void OnBrowsableChanged()
        {
            var handler = BrowsableChanged;
            if (handler != null)
                handler(this, System.EventArgs.Empty);
        }
    }
}