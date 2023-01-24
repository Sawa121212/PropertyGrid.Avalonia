using Avalonia;
using Avalonia.Controls;

namespace PropertyGrid.Utils
{
    /// <summary>
    /// Provides a unified approach for resolving resources.
    /// </summary>
    
    public class ResourceLocator
    {
        private readonly Application _application;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceLocator"/>.
        /// </summary>
        public ResourceLocator() : this(Application.Current) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceLocator"/>.
        /// </summary>
        /// <param name="application">The application instance.</param>
        public ResourceLocator(Application application)
        {
            _application = application;
        }

        /// <summary>
        /// Gets the resource.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Object from resources.</returns>
        public virtual object GetResource(object key)
        {
            object result = null;
            _application.TryFindResource(key, out result);
            return result;
        }
    }
}