using Avalonia;
using PropertyGrid.PropertyTypes;

namespace PropertyGrid.Editors
{
    public class ObservableCollectionEditor : Editor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableCollectionEditor"/>.
        /// </summary>

        public ObservableCollectionEditor()
        {
            
        }
        
        public new Type StyleKey => typeof(ObservableCollectionEditor);

        public static readonly StyledProperty<Type> ObservableCollectionEditedProperty =
            AvaloniaProperty.Register<ObservableCollectionEditor, Type>(nameof(ObservableCollectionEdited));
        /// <summary>
        /// Gets or sets the type of the object editor is bound to.
        /// </summary>
        /// <value>The type of the object editor is bound to.</value>
        public Type ObservableCollectionEdited
        {
            get { return (Type)GetValue(ObservableCollectionEditedProperty); }
            set { SetValue(ObservableCollectionEditedProperty, value); }
        }

        
        public PropertyItemValue PropertyValue
        {
            get { return (PropertyItemValue)GetValue(PropertyValueProperty); }
            set { SetValue(PropertyValueProperty, value); }
        }

        public static readonly StyledProperty<PropertyItemValue> PropertyValueProperty =
            AvaloniaProperty.Register<ObservableCollectionEditor, PropertyItemValue>(nameof(PropertyValue));
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableCollectionEditor"/>.
        /// </summary>
        /// <param name="editedType">The type of the object editor is bound to.</param>
        public ObservableCollectionEditor(Type editedType) : this(editedType, null, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableCollectionEditor"/>.
        /// </summary>
        /// <param name="editedType">The type of the object editor is bound to.</param>
        /// <param name="inlineTemplate">The inline template for UI presentation. Can be either a DataTemplate or ComponentResourceKey object.</param>
        public ObservableCollectionEditor(Type editedType, object inlineTemplate) : this(editedType, inlineTemplate, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableCollectionEditor"/>.
        /// </summary>
        /// <param name="editedType">Type of the edited.</param>
        /// <param name="inlineTemplate">The inline template for UI presentation. Can be either a DataTemplate or ComponentResourceKey object.</param>
        /// <param name="extendedTemplate">The extended template for UI presentation. Can be either a DataTemplate or ComponentResourceKey object.</param>
        public ObservableCollectionEditor(Type editedType, object inlineTemplate, object extendedTemplate)
        {
            if (editedType == null)
                throw new ArgumentNullException("editedType");

            ObservableCollectionEdited = editedType;

            InlineTemplate = GetEditorTemplate(inlineTemplate);

            ExtendedTemplate = GetEditorTemplate(extendedTemplate);
        }
    }
}