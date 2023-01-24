﻿using System.Diagnostics;
using Avalonia;

namespace PropertyGrid.Editors
{
    /// <summary>
    /// Provides value editing service for a property value.
    /// </summary>
    [DebuggerDisplay("PropertyName: {PropertyName} DeclaringType: {DeclaringType}")]
    
    public class PropertyEditor : Editor
    {
        /// <summary>
        /// Gets or sets type of the object containing edited property.
        /// </summary>
        /// <value>The type of the edited property </value>
        public Type DeclaringType
        {
            get { return (Type)GetValue(DeclaringTypeProperty); }
            set { SetValue(DeclaringTypeProperty, value); }
        }

        public static readonly StyledProperty<Type> DeclaringTypeProperty =
            AvaloniaProperty.Register<PropertyEditor, Type>(nameof(DeclaringType));

        public string PropertyName
        {
            get { return (string)GetValue(PropertyNameProperty); }
            set { SetValue(PropertyNameProperty, value); }
        }

        public static readonly StyledProperty<string> PropertyNameProperty =
            AvaloniaProperty.Register<PropertyEditor, string>(nameof(PropertyName));

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyEditor"/>.
        /// </summary>
        public PropertyEditor()
        {
            // Default empty constructor
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyEditor"/>.
        /// </summary>
        /// <param name="declaringType">Type that contains targeted property.</param>
        /// <param name="propertyName">Name of the property to bind the editor to.</param>
        public PropertyEditor(Type declaringType, string propertyName)
        {
            if (declaringType == null)
                throw new ArgumentNullException("declaringType");
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException("propertyName");

            DeclaringType = declaringType;
            PropertyName = propertyName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyEditor"/>.
        /// </summary>
        /// <param name="declaringType">Type that contains targeted property.</param>
        /// <param name="propertyName">Name of the property to bind the editor to.</param>
        /// <param name="inlineTemplate">The inline template for UI presentation. Can be either a DataTemplate or ComponentResourceKey object.</param>
        public PropertyEditor(Type declaringType, string propertyName, object inlineTemplate)
          : this(declaringType, propertyName)
        {
            InlineTemplate = GetEditorTemplate(inlineTemplate);
        }
    }
}