using Avalonia.Controls;

namespace PropertyGrid.Design
{
    /// <summary>
    /// Defines a content presenter control for a Property editor.
    /// </summary>
    
    public sealed class PropertyEditorContentPresenter : ContentControl
    {
        public Type StyleKey => typeof(PropertyEditorContentPresenter);

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyEditorContentPresenter"/>.
        /// </summary>
        public PropertyEditorContentPresenter()
        {
        }
    }
}