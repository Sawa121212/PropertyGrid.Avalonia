namespace PropertyGrid.Design
{
    public class ExtendedCategoryContainer : GridEntryContainer
    {
        public new Type StyleKey => typeof(ExtendedCategoryContainer);

        /// <summary>
        /// Initializes the <see cref="CategoryContainer"/>.
        /// </summary>
        static ExtendedCategoryContainer()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryContainer"/>.
        /// </summary>
        public ExtendedCategoryContainer()
        {
            SetParentContainer(this, this);
        }
    }
}