using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;

namespace PropertyGrid.Design
{
    /// <summary>
    /// Specifies a property name presenter.
    /// </summary>
    
    public sealed class PropertyNameTextBlock : TextBox
    {
        public Type StyleKey => typeof(PropertyNameTextBlock);

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyNameTextBlock"/>.
        /// </summary>
        public PropertyNameTextBlock()
        {
            IsReadOnly = true;
            //TextTrimming = TextTrimming.CharacterEllipsis;
            //TextWrapping = Media.TextWrapping.NoWrap;
            TextAlignment = TextAlignment.Left;
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Center;
            ClipToBounds = true;

            KeyboardNavigation.SetTabNavigation(this, KeyboardNavigationMode.None);
        }
    }
}