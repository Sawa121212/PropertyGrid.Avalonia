using Avalonia.Media;

namespace PropertyGrid.Font
{
    public static class FontWeightExtensions
    {
        public static int ToOpenTypeWeight(this FontWeight fontWeight)
        {
            //realfont weight in wpf (don't know where 400 comes from not documented. )
            return (int)fontWeight + 400;
        }
    }
}