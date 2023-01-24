using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace PropertyGrid.Controls
{
    /// <summary>
    /// Property search box control.
    /// </summary>
    public class SearchTextBox : TextBox
    {
        private const string DefaultLabelText = "Поиск";

        private readonly DispatcherTimer _searchEventDelayTimer;

        public Type StyleKey => typeof(SearchTextBox);

        /// <summary>
        /// Gets or sets the search mode.
        /// </summary>
        public SearchMode SearchMode
        {
            get { return (SearchMode)GetValue(SearchModeProperty); }
            set { SetValue(SearchModeProperty, value); }
        }

        public static readonly StyledProperty<SearchMode> SearchModeProperty =
            AvaloniaProperty.Register<SearchTextBox, SearchMode>(nameof(SearchMode)
                , defaultValue: SearchMode.Instant);

        /// <summary>
        /// Gets a value indicating whether this instance has text.
        /// </summary>
        /// <value><c>true</c> if this instance has text;
        ///     otherwise, <c>false</c>.</value>
        public bool HasText
        {
            get { return (bool)GetValue(HasTextProperty); }
            set { SetValue(HasTextProperty, value); }
        }

        public static readonly StyledProperty<bool> HasTextProperty =
            AvaloniaProperty.Register<SearchTextBox, bool>(nameof(HasText));

        /// <summary>
        /// Gets or sets a value indicating whether mouse left button is down.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if mouse left button is down; otherwise, <c>false</c>.
        /// </value>
        public bool IsMouseLeftButtonDown
        {
            get { return (bool)GetValue(IsMouseLeftButtonDownProperty); }
            set { SetValue(IsMouseLeftButtonDownProperty, value); }
        }

        public static readonly StyledProperty<bool> IsMouseLeftButtonDownProperty =
            AvaloniaProperty.Register<SearchTextBox, bool>(nameof(IsMouseLeftButtonDown));

        /// <summary>
        /// Gets or sets the search event time delay.
        /// </summary>
        /// <value>The search event time delay.</value>
        public TimeSpan SearchEventTimeDelay
        {
            get { return (TimeSpan)GetValue(SearchEventTimeDelayProperty); }
            set { SetValue(SearchEventTimeDelayProperty, value); }
        }

        public static readonly StyledProperty<TimeSpan> SearchEventTimeDelayProperty =
            AvaloniaProperty.Register<SearchTextBox, TimeSpan>(nameof(SearchEventTimeDelay)
                , defaultValue: TimeSpan.FromMilliseconds(500));

        public static readonly RoutedEvent<RoutedEventArgs> SearchEvent =
            RoutedEvent.Register<SearchTextBox, RoutedEventArgs>(nameof(SearchEvent),
                RoutingStrategies.Bubble);

        /// <summary>
        /// Occurs when search is performed.
        /// </summary>
        public event EventHandler Search
        {
            add { AddHandler(SearchEvent, value); }
            remove { RemoveHandler(SearchEvent, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchTextBox"/>.
        /// </summary>
        public SearchTextBox()
        {
            _searchEventDelayTimer = new DispatcherTimer { Interval = SearchEventTimeDelay };
            _searchEventDelayTimer.Tick += OnSeachEventDelayTimerTick;

            SearchEventTimeDelayProperty.Changed.AddClassHandler<SearchTextBox>(((o, e) =>
                OnSearchEventTimeDelayChanged(o, e)));
            TextProperty.Changed.AddClassHandler<SearchTextBox>(((o, e) => OnTextChanged(o, e)));
        }

        /// <summary>
        /// Is called when content in this editing control changes.
        /// </summary>
        private void OnTextChanged(SearchTextBox searchTextBox, AvaloniaPropertyChangedEventArgs e)
        {
            HasText = Text.Length != 0;

            if (SearchMode == SearchMode.Instant)
            {
                _searchEventDelayTimer.Stop();
                _searchEventDelayTimer.Start();
            }
        }

        private void OnSeachEventDelayTimerTick(object o, System.EventArgs e)
        {
            _searchEventDelayTimer.Stop();
            RaiseSearchEvent();
        }

        private void OnSearchEventTimeDelayChanged(SearchTextBox searchTextBox, AvaloniaPropertyChangedEventArgs e)
        {
            if (searchTextBox != null)
            {
                searchTextBox._searchEventDelayTimer.Interval = ((TimeSpan)e.NewValue);
                searchTextBox._searchEventDelayTimer.Stop();
            }
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            var iconBorder = e.NameScope.Find<Border>("PART_SearchIconBorder");
            if (iconBorder != null)
            {
                iconBorder.PointerPressed += IconBorderMouseLeftButtonDown;
                iconBorder.PointerReleased += IconBorderMouseLeftButtonUp;
                iconBorder.PointerExited += IconBorderMouseLeave;
                ;
            }

            //base.OnTemplateApplied(e);
            base.OnApplyTemplate(e);
        }

        private void IconBorderMouseLeave(object sender, PointerEventArgs e)
        {
            IsMouseLeftButtonDown = false;
        }

        private void IconBorderMouseLeftButtonUp(object sender, PointerReleasedEventArgs e)
        {
            var prop = e.GetCurrentPoint(sender as IVisual).Properties;
            if (IsMouseLeftButtonDown == false)
            {
                return;
            }

            if (!IsMouseLeftButtonDown)
                return;

            if (HasText && SearchMode == SearchMode.Instant)
            {
                Text = "";
            }

            if (HasText && SearchMode == SearchMode.Delayed)
            {
                RaiseSearchEvent();
            }

            IsMouseLeftButtonDown = false;
        }

        private void IconBorderMouseLeftButtonDown(object sender, PointerPressedEventArgs e)
        {
            var prop = e.GetCurrentPoint(sender as IVisual).Properties;
            if (prop.IsLeftButtonPressed)
            {
                IsMouseLeftButtonDown = true;
            }
            else
            {
                IsMouseLeftButtonDown = false;
            }
        }

        /// <summary>
        /// Вызывается всякий раз, когда необработанное подключенное маршрутизируемое событие  <see cref="InputElement.KeyDown"/>
        /// достигает элемента, производного от этого класса в своем маршруте.
        /// Реализуйте этот метод, чтобы добавить обработку класса для этого события.
        /// </summary>
        /// <param name="e">Provides data about the event.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape && SearchMode == SearchMode.Instant)
            {
                Text = "";
            }
            else if ((e.Key == Key.Return || e.Key == Key.Enter) &&
                     SearchMode == SearchMode.Delayed)
            {
                RaiseSearchEvent();
            }
            else
            {
                base.OnKeyDown(e);
            }
        }

        private void RaiseSearchEvent()
        {
            var args = new RoutedEventArgs(SearchEvent);
            RaiseEvent(args);
        }

        /// <summary>
        /// Вызывается, когда одно или несколько свойств avalonia, существующих в элементе, изменили свои эффективные значения.
        /// </summary>
        /// <param name="e">Arguments for the associated event.</param>
        protected void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> e)
        {
            if (e.Property == IsVisibleProperty)
            {
                Text = string.Empty;
            }

            base.OnPropertyChanged(e);
        }
    }
}