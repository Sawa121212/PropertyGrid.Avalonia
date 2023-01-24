using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Common.Core;
using Common.Core.Extensions;
using Prism.Commands;
using Prism.Mvvm;

namespace PropertyGrid.CollectionControl
{
    public class CollectionControlViewModel : BindableBase, IInitializable<IList>, IResult<IList>
    {
        private ObservableCollection<object> _collection;
        private object _selectedElement;
        private bool _needSaveChanges;
        private int _selectedElementIndex;

        public CollectionControlViewModel()
        {
            UpCommand = new DelegateCommand(OnUp);
            DownCommand = new DelegateCommand(OnDown);
            DeleteCommand = new DelegateCommand(OnDelete);
            OkCommand = new DelegateCommand(OnOk);
            CancelCommand = new DelegateCommand(OnCancel);

            Collection = new ObservableCollection<object>();
        }

        public void Initialize(IList collection)
        {
            foreach (var element in collection)
            {
                Collection.Add(element);
            }
        }


        /// <summary>
        /// Меняет порядок NodeElement в коллекции, меняет со предыдущим по порядку, Если первый не трогает
        /// </summary>
        private void OnUp()
        {
            var oldIndex = Collection.IndexOf(_selectedElement);
            if (oldIndex.IsBetween(0, Collection.Count - 1, false, true))
            {
                var newIndex = oldIndex - 1;
                Collection.Move(oldIndex, newIndex);
                SelectedItemIndex = newIndex;
            }
        }

        /// <summary>
        /// Меняет порядок элементов в коллекции, меняет со следующим по порядку, Если последний не трогает 
        /// </summary>
        private void OnDown()
        {
            var oldIndex = Collection.IndexOf(_selectedElement);
            if (oldIndex.IsBetween(0, Collection.Count - 1, true, false))
            {
                var newIndex = oldIndex + 1;
                Collection.Move(oldIndex, newIndex);
                SelectedItemIndex = newIndex;
            }
        }

        /// <summary>
        /// Удаляет выбранный NodeElement из коллекции
        /// </summary>
        private void OnDelete()
        {
            Collection.Remove(_selectedElement);
        }

        private void OnOk()
        {
            _needSaveChanges = true;
        }

        private void OnCancel()
        {
            _needSaveChanges = false;
        }

        public IList GetResult()
        {
            if (_needSaveChanges)
            {
                return Collection;
            }
            else
            {
                return null;
            }
        }

        public ObservableCollection<object> Collection
        {
            get => _collection;
            set => SetProperty(ref _collection, value);
        }

        public object SelectedItem
        {
            get => _selectedElement;
            set => SetProperty(ref _selectedElement, value);
        }

        public int SelectedItemIndex
        {
            get => _selectedElementIndex;
            set => SetProperty(ref _selectedElementIndex, value);
        }

        public ICommand UpCommand { get; }
        public ICommand DownCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }
    }
}