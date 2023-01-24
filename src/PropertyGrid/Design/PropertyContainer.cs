namespace PropertyGrid.Design
{
    /// <summary>
    /// Specialized UI container for a property entry.
    /// </summary>
    
    public class PropertyContainer : GridEntryContainer
    {
        public new Type StyleKey => typeof(PropertyContainer);

        
        static PropertyContainer()
        {
        }

        public PropertyContainer()
        {
            SetParentContainer(this, this);
        }
    }
}