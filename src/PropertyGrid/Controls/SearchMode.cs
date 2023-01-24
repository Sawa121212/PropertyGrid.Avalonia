namespace PropertyGrid.Controls
{
    /// <summary>
    /// Определяет свойство search behavior of <see cref="SearchTextBox"/>.
    /// </summary>
    public enum SearchMode
    {
        /// <summary>
        /// Immediatelly fire search.
        /// </summary>
        Instant,

        /// <summary>
        /// Fire search with a delay.
        /// </summary>
        Delayed,
    }
}