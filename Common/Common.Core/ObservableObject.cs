using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Common.Avalonia.Attributes;
using ReactiveUI;

namespace Common.Core
{
    public class ObservableObject : ReactiveObject, IObservableObject
    {
        /// <inheritdoc />
        [Browsable(false)]
        public Dictionary<string, List<string>> PropertyDependencies { get; }

        [Browsable(false)] public Dictionary<string, List<string>> CollectionPropertyDependencies { get; private set; }

        private readonly Lazy<Dictionary<INotifyCollectionChanged, string>> _collectionSources;
        //private readonly Lazy<Dictionary<INotifyPropertyChanged, Action>> _notifiedPropertiesActions;

        public ObservableObject()
        {
            PropertyDependencies = this.BuildPropertyDependencies();

            _collectionSources = new Lazy<Dictionary<INotifyCollectionChanged, string>>();
            //_notifiedPropertiesActions = new Lazy<Dictionary<INotifyPropertyChanged, Action>>();
        }

        /// <summary>
        /// Построить зависимости Properties от изменения коллекций (INotifyCollectionChanged) 
        /// </summary>
        /*public void BuildCollectionDependencies()
        {
            CollectionPropertyDependencies = BuildCollectionPropertyDependencies();
        }*/

        /// <summary>
        /// Occurs after a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Provides access to the PropertyChanged event handler to Helpers through IObservableObject.
        /// </summary>
        PropertyChangedEventHandler IObservableObject.PropertyChangedHandler => PropertyChanged;

        /// <summary>
        /// Occurs before a property value changes.
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;

        /// <summary>
        /// Provides access to the PropertyChanging event handler to Helpers through IObservableObject.
        /// </summary>
        PropertyChangingEventHandler IObservableObject.PropertyChangingHandler => PropertyChanging;

        /*  public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
          {
              /*if (managerType == typeof(PropertyChangedEventManager))
              {
                  if (sender is INotifyPropertyChanged propertyChanged)
                  {
                      if (_notifiedPropertiesActions.Value.TryGetValue(propertyChanged, out var action))
                      {
                          action.Invoke();
                      }
                  }
                  return true;
            
                 
              }
              else #1#
              if (managerType == typeof(CollectionChangedEventManager))
              {
                  var dependencySourceName = GetDependencySourceName((INotifyCollectionChanged)sender);
  
                  if (dependencySourceName != null)
                  {
                      if (CollectionPropertyDependencies != null)
                          RaisePropertyChanged(CollectionPropertyDependencies, dependencySourceName, new string[0]);
                  }
  
                  return true;
              }
  
              return false;
          }*/

        /// <summary>
        /// Verifies that a property name exists in this ViewModel. This method
        /// can be called before the property is used, for instance before
        /// calling RaisePropertyChanged. It avoids errors when a property name
        /// is changed but some places are missed.
        /// </summary>
        /// <remarks>This method is only active in DEBUG mode.</remarks>
        /// <param name="propertyName">The name of the property that will be
        /// checked.</param>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        private void VerifyPropertyName(string propertyName)
        {
            var myType = this.GetType();

            if (!string.IsNullOrEmpty(propertyName) && myType.GetProperty(propertyName) == null)
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                var descriptor = this as ICustomTypeDescriptor;
                if (descriptor?.GetProperties().Cast<PropertyDescriptor>()
                        .Any(property => property.Name == propertyName) == true)
                {
                    return;
                }

                throw new ArgumentException("Property not found", propertyName);
            }
        }

        /// <summary>
        /// Raises the PropertyChanging event if needed.
        /// </summary>
        /// <remarks>If the propertyName parameter
        /// does not correspond to an existing property on the current class, an
        /// exception is thrown in DEBUG configuration only.</remarks>
        /// <param name="propertyName">(optional) The name of the property that
        /// changed.</param>
        public void RaisePropertyChanging([CallerMemberName] string propertyName = null)
        {
            VerifyPropertyName(propertyName);
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

        /// <summary>
        /// Raises the PropertyChanged event if needed.
        /// </summary>
        /// <remarks>If the propertyName parameter
        /// does not correspond to an existing property on the current class, an
        /// exception is thrown in DEBUG configuration only.</remarks>
        /// <param name="propertyName">(optional) The name of the property that
        /// changed.</param>
        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            VerifyPropertyName(propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            if (PropertyDependencies != null && !string.IsNullOrEmpty(propertyName))
            {
                RaiseDependentPropertiesChanged(PropertyDependencies, propertyName);
            }
        }

        /// <summary>
        /// Assigns a new value to the property. Then, raises the
        /// PropertyChanged event if needed. 
        /// </summary>
        /// <typeparam name="T">The type of the property that
        /// changed.</typeparam>
        /// <param name="field">The field storing the property's value.</param>
        /// <param name="newValue">The property's value after the change
        /// occurred.</param>
        /// <param name="propertyName">(optional) The name of the property that
        /// changed.</param>
        /// <returns>True if the PropertyChanged event has been raised,
        /// false otherwise. The event is not raised if the old
        /// value is equal to the new value.</returns>
        public bool Set<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
                return false;

            // ReSharper disable ExplicitCallerInfoArgument
            RaisePropertyChanging(propertyName);
            // ReSharper restore ExplicitCallerInfoArgument

            field = newValue;

            // ReSharper disable ExplicitCallerInfoArgument
            RaisePropertyChanged(propertyName);
            // ReSharper restore ExplicitCallerInfoArgument

            return true;
        }

        /// <summary>
        /// Assigns a new value to the property. Then, raises the
        /// PropertyChanged event if needed. 
        /// </summary>
        /// <typeparam name="T">The type of the property that
        /// changed.</typeparam>
        /// <param name="field">The field storing the property's value.</param>
        /// <param name="newValue">The property's value after the change
        /// occurred.</param>
        /// <param name="propertyName">(optional) The name of the property that
        /// changed.</param>
        /// <returns>True if the PropertyChanged event has been raised,
        /// false otherwise. The event is not raised if the old
        /// value is equal to the new value.</returns>
        public void SetAndRise<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (field == null)
                return;

            // ReSharper disable ExplicitCallerInfoArgument
            RaisePropertyChanging(propertyName);
            // ReSharper restore ExplicitCallerInfoArgument

            field = newValue;

            // ReSharper disable ExplicitCallerInfoArgument
            RaisePropertyChanged(propertyName);
            // ReSharper restore ExplicitCallerInfoArgument
        }

        /// <summary>
        /// Assigns a new value to the property. Then, raises the
        /// PropertyChanged event if needed, and than do Action
        /// </summary>
        /// <typeparam name="T">The type of the property that
        /// changed.</typeparam>
        /// <param name="field">The field storing the property's value.</param>
        /// <param name="newValue">The property's value after the change
        /// occurred.</param>
        /// <param name="action"></param>
        /// <param name="propertyName">(optional) The name of the property that
        /// changed.</param>
        /// <returns>True if the PropertyChanged event has been raised,
        /// false otherwise. The event is not raised if the old
        /// value is equal to the new value.</returns>
        public void Set<T>(ref T field, T newValue, Action<T> action, [CallerMemberName] string propertyName = null)
        {
            // ReSharper disable ExplicitCallerInfoArgument
            if (Set(ref field, newValue, propertyName))
                // ReSharper restore ExplicitCallerInfoArgument
                action(newValue);
        }

        private void RaiseDependentPropertiesChanged<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key,
            params string[] propertiesToExclude) where TValue : IEnumerable<string>
        {
            TValue props;
            if (!dictionary.TryGetValue(key, out props))
                return;

            foreach (var propertyName in props.Except(propertiesToExclude))
            {
                VerifyPropertyName(propertyName);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void RaisePropertyChanged<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> dictionary, TKey key,
            params string[] propertiesToExclude)
            where TValue : IEnumerable<string>
        {
            if (dictionary.TryGetValue(key, out var property))
            {
                foreach (var propertyName in property.Except(propertiesToExclude))
                {
                    RaisePropertyChanged(propertyName);
                }
            }
        }

        private Dictionary<string, List<string>> BuildPropertyDependencies()
        {
            Dictionary<string, List<string>> result = null;

            foreach (var property in GetType()
                         .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                foreach (var propertyAttribute in property.GetCustomAttributes<ValueDependsOnPropertyAttribute>(false))
                {
                    if (result == null)
                        result = new Dictionary<string, List<string>>();

                    if (!result.TryGetValue(propertyAttribute.SourceProperName, out var targetPropertiesNames))
                    {
                        targetPropertiesNames = new List<string>();
                        result.Add(propertyAttribute.SourceProperName, targetPropertiesNames);
                    }

                    targetPropertiesNames.Add(property.Name);
                }
            }

            return result;
        }

        // private Dictionary<string, List<string>> BuildCollectionPropertyDependencies()
        // {
        //     Dictionary<string, List<string>> dictionary = null;
        //
        //     foreach (var propertyInfo in GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        //     {
        //         foreach (var collectionAttribute in propertyInfo.GetCustomAttributes<ValueDependsOnCollectionAttribute>())
        //         {
        //             if (dictionary == null)
        //             {
        //                 dictionary = new Dictionary<string, List<string>>();
        //             }
        //
        //             if (!dictionary.TryGetValue(collectionAttribute.SourceName, out var targetPropertiesNames))
        //             {
        //                 targetPropertiesNames = new List<string>();
        //                 dictionary.Add(collectionAttribute.SourceName, targetPropertiesNames);
        //                 var sourceCollection = GetObservableCollectionFor(this, collectionAttribute.SourceName);
        //                 if (sourceCollection != null)
        //                     AddDependencySource(collectionAttribute.SourceName, sourceCollection);
        //             }
        //
        //             targetPropertiesNames.Add(propertyInfo.Name);
        //         }
        //     }
        //
        //     return dictionary;
        // }


        private INotifyPropertyChanged GetObservablePropertyFor(object sourceObject, string propertyName)
        {
            foreach (var info in sourceObject.GetType().GetProperties()
                         .Where(pi => pi.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase)))
            {
                if (info.PropertyType.GetInterfaces().Contains(typeof(INotifyPropertyChanged)))
                {
                    var property = info.GetValue(this);
                    if (property != null)
                        return (INotifyPropertyChanged)property;
                }
            }

            return null;
        }

        /// <summary>
        /// Добавить в словарь свойство зависящее от изменения коллекции
        /// </summary>
        /// <param name="name">Имя Свойства</param>
        /// <param name="source">Коллекция от которой зависит свойство.</param>
        /*private void AddDependencySource(string name, INotifyCollectionChanged source)
        {
            if (!_collectionSources.Value.ContainsKey(source))
            {
                _collectionSources.Value.Add(source, name);
                CollectionChangedEventManager.AddListener(source, this);
            }
        }*/

        /// <summary>
        /// Получить из словаря Свойство зависящее от коллекции
        /// </summary>
        /// <param name="source">Коллекция от которой зависит Свойство.</param>
        /// <returns></returns>
        private string GetDependencySourceName(INotifyCollectionChanged source)
        {
            string sourceName = null;
            if (_collectionSources.IsValueCreated)
            {
                _collectionSources.Value.TryGetValue(source, out sourceName);
            }

            return sourceName;
        }

        private INotifyCollectionChanged GetObservableCollectionFor(object sObject, string collectionPropertyName)
        {
            foreach (PropertyInfo info in sObject.GetType().GetProperties()
                         .Where(pi => pi.Name == collectionPropertyName))
            {
                if (info.PropertyType.GetInterfaces().Contains(typeof(INotifyCollectionChanged)))
                {
                    var collection = info.GetValue(this);
                    if (collection != null)
                        return (INotifyCollectionChanged)collection;
                }
            }

            return null;
        }
    }
}