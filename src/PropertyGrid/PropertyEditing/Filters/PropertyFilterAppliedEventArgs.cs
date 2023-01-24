namespace PropertyGrid.PropertyEditing.Filters
{
    /// <summary>
    /// Contains state information and data related to FilterApplied event.
    /// </summary>
    
    public class PropertyFilterAppliedEventArgs
    {
        /// <summary>
        /// Gets the filter.
        /// </summary>
        /// <value>The filter.</value>
        public PropertyFilter Filter { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyFilterAppliedEventArgs"/>.
        /// </summary>
        /// <param name="filter">The filter.</param>
        public PropertyFilterAppliedEventArgs(PropertyFilter filter)
        {
            Filter = filter;
        }
    }
}