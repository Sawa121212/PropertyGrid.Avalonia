using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;

namespace PropertyGrid.Design
{
    /// <summary>
    /// Native Tab item for <see cref="TabbedLayout"/>
    /// </summary>
    
    public class TabbedLayoutItem : TabItem
    {
        public Type StyleKey => typeof(TabbedLayoutItem);
        
        /// <summary>
        /// Gets or sets a value indicating whether this instance can close.
        /// </summary>
        /// <value><c>true</c> if this instance can close; otherwise, <c>false</c>.</value>
        public bool CanClose
        {
            get { return (bool)GetValue(CanCloseProperty); }
            set { SetValue(CanCloseProperty, value); }
        }

        public static readonly StyledProperty<bool> CanCloseProperty =
            AvaloniaProperty.Register<TabbedLayoutItem, bool>(nameof(CanClose));


        /// <summary>
        /// command which is set by the TabbedLayout Control
        /// </summary>
        public ICommand ClosePropertyTabCommand
        {
            get { return (ICommand)GetValue(ClosePropertyTabCommandProperty); }
            set { SetValue(ClosePropertyTabCommandProperty, value); }
        }


        public static readonly StyledProperty<ICommand> ClosePropertyTabCommandProperty =
            AvaloniaProperty.Register<TabbedLayoutItem, ICommand>(nameof(ClosePropertyTabCommand));

    }
}