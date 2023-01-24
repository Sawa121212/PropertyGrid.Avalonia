using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using PropertyGrid.Avalonia.Test;

namespace PropertyGrid.Avalonia.Views
{
    public class ShellViewModel : BindableBase
    {
        private object _selectedItem;

        public ShellViewModel()
        {
            SendNewItemCommand = new DelegateCommand (OnChangeItem);
        }

        private void OnChangeItem()
        {
            var element = new BaseTestClass("testClass")
            {
                Description = "Базовый тестовый класс",
                TypicalString = "string",
                Enum = CollectionEnum.one,
                StringCollection = new ObservableCollection<string>(),
                NodeElementsCollection = new ObservableCollection<NodeElement>(),
                ListOfElements = new List<NodeElement>(),
                CollectionElements = new List<NodeElement>()
            };
            element.ListOfElements.Add(new NodeElement("Name"));
            element.ListOfElements.Add(new NodeElement("Name1"));
            element.ListOfElements.Add(new NodeElement("Name2"));
            element.NodeElementsCollection.Add(new NodeElement("name"));
            element.NodeElementsCollection.Add(new NodeElement("name1"));
            element.NodeElementsCollection.Add(new NodeElement("name2"));
            element.StringCollection.Add("n1");
            element.StringCollection.Add("n2");
            element.StringCollection.Add("n3");
            SelectedItem = element;
        }


        public object SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        public string Title => "Avalonia+Prism Application";


        public ICommand SendNewItemCommand { get; }
    }
}