using System.Collections;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Common.Core;

namespace PropertyGrid.CollectionControl
{
    public partial class CollectionControlView : Window, IViewWithResult<IList>
    {
        public CollectionControlView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public IList Result => DataContext is IResult<IList> result ? result.GetResult() : null;
    }
}