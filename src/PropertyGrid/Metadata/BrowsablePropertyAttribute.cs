namespace PropertyGrid.Metadata
{
    /// <summary>
    /// Controls the Browsable state of the category with corresponding properties.
    /// Supports the "*" (All) wildcard determining whether all the categories within the given class should be Browsable.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class BrowsablePropertyAttribute : Attribute
    {
        /// <summary>
        /// Determines a wildcard for all properties to be affected.
        /// </summary>
        public const string All = "*";

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        public string PropertyName { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether property is browsable.
        /// </summary>
        /// <value><c>true</c> if property should be displayed at run time; otherwise, <c>false</c>.</value>
        public bool Browsable { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowsablePropertyAttribute"/>.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="browsable">if set to <c>true</c> the property is browsable.</param>
        public BrowsablePropertyAttribute(string propertyName, bool browsable)
        {
            PropertyName = string.IsNullOrEmpty(propertyName) ? All : propertyName;
            Browsable = browsable;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowsablePropertyAttribute"/>.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public BrowsablePropertyAttribute(string propertyName) : this(propertyName, true) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowsablePropertyAttribute"/>.
        /// </summary>
        /// <param name="browsable">if set to <c>true</c> all public properties are browsable; otherwise hidden.</param>
        public BrowsablePropertyAttribute(bool browsable) : this(All, browsable) { }
    }
}