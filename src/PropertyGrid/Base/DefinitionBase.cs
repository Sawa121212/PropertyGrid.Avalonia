using System.Linq.Expressions;
using Avalonia;

namespace PropertyGrid.Base
{
    public abstract class DefinitionBase : AvaloniaObject
    {
        internal bool IsLocked { get; private set; }

        internal void ThrowIfLocked<TMember>(Expression<Func<TMember>> propertyExpression)
        {
            //In XAML, when using any properties of PropertyDefinition, the error of ThrowIfLocked is always thrown => prevent it !
            //if (DesignerProperties.GetIsInDesignMode(this))
            //    return;

            /// Аналог в авалонии
            if (global::Avalonia.Controls.Design.IsDesignMode)
            {
                return;
            }
            if (IsLocked)
            {
                var propertyName = ReflectionHelper.GetPropertyOrFieldName(propertyExpression);
                var message = string.Format(
                    @"Cannot modify {0} once the definition has beed added to a collection.",
                    propertyName);
                throw new InvalidOperationException(message);
            }
        }

        internal virtual void Lock()
        {
            if (!IsLocked)
                IsLocked = true;
        }
    }
}