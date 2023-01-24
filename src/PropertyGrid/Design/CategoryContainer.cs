namespace PropertyGrid.Design
{
    public class CategoryContainer : GridEntryContainer
    {
        public new Type StyleKey => typeof(CategoryContainer);

        /// <summary>
        /// Initializes the <see cref="CategoryContainer"/>.
        /// </summary>
        static CategoryContainer()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryContainer"/>.
        /// </summary>
        public CategoryContainer()
        {
            SetParentContainer(this, this);
        }
    }
}