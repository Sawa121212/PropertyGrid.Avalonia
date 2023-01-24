namespace PropertyGrid.EventArgs
{
    /// <summary>
    /// Событие с Guid.
    /// </summary>
    public class GuidEventArgs : System.EventArgs
    {
        /// <summary>
        /// Событие с Guid.
        /// </summary>
        /// <param name="guid"></param>
        public GuidEventArgs(Guid guid)
        {
            Guid = guid;
        }

        public Guid Guid { get; }
    }
}
