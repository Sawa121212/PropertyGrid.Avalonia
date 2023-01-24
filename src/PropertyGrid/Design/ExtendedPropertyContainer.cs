namespace PropertyGrid.Design
{
    public class ExtendedPropertyContainer : GridEntryContainer
    {
        public new Type StyleKey => typeof(ExtendedPropertyContainer);

        
        static ExtendedPropertyContainer()
        {
        }

        
        public ExtendedPropertyContainer()
        {
            SetParentContainer(this, this);
        }
    }
}