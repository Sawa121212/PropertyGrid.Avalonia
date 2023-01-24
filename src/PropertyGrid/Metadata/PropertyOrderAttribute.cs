namespace PropertyGrid.Metadata
{
    /// <summary>
    /// Specifies the order of property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    
    public sealed class PropertyOrderAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>The order.</value>
        public int Order { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyOrderAttribute"/>.
        /// </summary>
        /// <param name="order">The order.</param>
        public PropertyOrderAttribute(int order)
        {
            Order = order;
        }
    }
}