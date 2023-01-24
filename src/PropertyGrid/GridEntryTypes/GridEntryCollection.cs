using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace PropertyGrid.GridEntryTypes
{
    /// <summary>
    /// Представляет строго типизированную коллекцию элементов на основе <см. cref="GridEntry"/>, доступ к которым можно получить по индексу или имени.
    /// Предоставляет уведомления об изменении коллекции, методы поиска, сортировки и управления списками.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    
    public class GridEntryCollection<T> : ObservableCollection<T> where T : GridEntry
    {
        private readonly Dictionary<string, T> _itemsMap = new Dictionary<string, T>();

        /// <summary>
        /// Инициализирует новый экземпляр класса GridEntryCollection
        /// </summary>
        public GridEntryCollection()
        { }

        /// <summary>
        /// Инициализирует новый экземпляр класса GridEntryCollection
        /// </summary>
        /// <param name="collection">The collection from which the elements are copied.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="collection"/> parameter cannot be null.
        /// </exception>
        public GridEntryCollection(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            CopyFrom(collection);
        }

        /// <summary>
        /// Выполняет поиск элемента во всей отсортированной коллекции GridEntryCollection
        /// с использованием указанного средства сравнения и возвращает нулевой индекс элемента.
        /// </summary>
        /// <param name="item">The object to locate. The value can be null for reference types.</param>
        /// <returns>
        /// The zero-based index of item in the sorted GridEntryCollection&lt;T&gt;,
        /// if item is found; otherwise, a negative number that is the bitwise complement
        /// of the index of the next element that is larger than item or, if there is
        /// no larger element, the bitwise complement of GridEntryCollection&lt;T&gt;.Count.
        /// </returns>
        public int BinarySearch(T item)
        {
            return ((List<T>)Items).BinarySearch(item);
        }

        /// <summary>
        /// Выполняет поиск элемента во всей отсортированной коллекции GridEntryCollection с использованием указанного средства
        /// сравнения и возвращает индекс элемента, основанный на нуле.
        /// </summary>
        /// <param name="item">The object to locate. The value can be null for reference types.</param>
        /// <param name="comparer">
        /// The System.Collections.Generic.IComparer&lt;T&gt; implementation to use when comparing elements.
        /// -or- null to use the default comparer System.Collections.Generic.Comparer&lt;T&gt;.Default.
        /// </param>
        /// <returns>
        /// The zero-based index of item in the sorted GridEntryCollection&lt;T&gt;,
        /// if item is found; otherwise, a negative number that is the bitwise complement
        /// of the index of the next element that is larger than item or, if there is
        /// no larger element, the bitwise complement of GridEntryCollection&lt;T&gt;.Count.
        /// </returns>
        public int BinarySearch(T item, IComparer<T> comparer)
        {
            return ((List<T>)Items).BinarySearch(item, comparer);
        }

        /// <summary>
        /// Сортирует элементы во всей коллекции с помощью указанного средства сравнения.
        /// </summary>
        /// <param name="comparer">
        /// The System.Collections.Generic.IComparer&lt;T&gt; implementation to use when comparing elements.
        /// -or- null to use the default comparer System.Collections.Generic.Comparer&lt;T&gt;.Default.
        /// </param>
        public void Sort(IComparer<T> comparer)
        {
            ((List<T>)Items).Sort(comparer);
            OnItemsChanged();
        }

        private void OnItemsChanged()
        {
            base.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            base.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, null, -1));
        }

        /// <summary>
        /// Копирует значения из коллекции.
        /// </summary>
        /// <param name="collection">The collection.</param>
        protected void CopyFrom(IEnumerable<T> collection)
        {
            if (collection != null)
            {
                using (IEnumerator<T> enumerator = collection.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Add(enumerator.Current);
                    }
                }
            }
        }

        /// <summary>
        /// Вставляет элемент в коллекцию по указанному индексу.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert.</param>
        protected override void InsertItem(int index, T item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            EncacheItem(item);
            base.InsertItem(index, item);
        }

        /// <summary>
        /// Удаляет элемент с указанным индексом коллекции.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        protected override void RemoveItem(int index)
        {
            T item = Items[index];
            DecacheItem(item);
            base.RemoveItem(index);
        }

        /// <summary>
        /// Удаляет все элементы из коллекции.
        /// </summary>
        protected override void ClearItems()
        {
            _itemsMap.Clear();
            base.ClearItems();
        }

        /// <summary>
        /// Заменяет элемент с указанным индексом.
        /// </summary>
        /// <param name="index">The zero-based index of the element to replace.</param>
        /// <param name="item">The new value for the element at the specified index.</param>
        protected override void SetItem(int index, T item)
        {
            DecacheItem(this[index]);
            EncacheItem(item);
            base.SetItem(index, item);
        }

        /// <summary>
        /// Возвращает элемент с указанным именем или значение null, если элемент с указанным именем не был найден.
        /// </summary>
        /// <value></value>
        public T this[string name]
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                    return null;

                if (_itemsMap.ContainsKey(name))
                    return _itemsMap[name];
                else
                    return null;
            }
        }

        private void EncacheItem(T item)
        {
            if (_itemsMap.ContainsKey(item.Name))
                throw new ArgumentException(string.Format("The entry '{0}' is already added to collection!", item.Name));

            _itemsMap.Add(item.Name, item);
        }

        private void DecacheItem(T item)
        {
            if (_itemsMap.ContainsKey(item.Name))
                _itemsMap.Remove(item.Name);
        }
    }
}