using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Generators;
using Avalonia.Data;

namespace PropertyGrid.Design
{
    internal class GridEntryLayoutContainer<T> : ItemContainerGenerator<T> where T : GridEntryContainer, new()
    {
        public GridEntryLayoutContainer(GridEntryLayout<T> owner)
            : base(owner, ContentControl.ContentProperty, ItemsControl.ItemTemplateProperty)
        {
            Owner = owner;
        }

        public new GridEntryLayout<T> Owner { get; }

        protected override IControl CreateContainer(object element)
        {
            if (element is GridEntryContainer)
            {
                var item = element as GridEntryContainer;

                item.DataContext = Owner.DataContext;
                item.Bind(GridEntryContainer.EntryProperty, new Binding());
                return item;
            }
            return base.CreateContainer(element);
        }
    }
}