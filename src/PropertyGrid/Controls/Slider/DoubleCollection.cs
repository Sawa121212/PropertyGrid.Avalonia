using Avalonia.Collections;

namespace PropertyGrid.Controls.Slider
{
    /// <summary>
    /// avalonia collection of doubles
    /// </summary>
    public class DoubleCollection : AvaloniaList<double>
    {
        public static DoubleCollection Empty()
        {
            return new DoubleCollection();
        }
    }
}
