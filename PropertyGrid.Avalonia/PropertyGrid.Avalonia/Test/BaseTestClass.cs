using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using PropertyGrid.Metadata;

namespace PropertyGrid.Avalonia.Test
{
    public enum CollectionEnum
    {
        one,
        two,
        three
    }

    public class BaseTestClass : NodeElement
    {
        private ObservableCollection<string> _stringCollection;
        private ObservableCollection<NodeElement> _nodeElementsCollection;
        private string _typicalString;
        private bool _testBool;
        private IEnumerable<NodeElement> _collectionElements;
        private CollectionEnum _enum;
        private List<NodeElement> _elements;

        public BaseTestClass(string name) : base(name)
        {
        }

        [Category("SecondCategory")]
        [DisplayName("StringCollection")]
        [Browsable(true)]
        public ObservableCollection<string> StringCollection
        {
            get => _stringCollection;
            set => Set(ref _stringCollection, value);
        }

        [Category("SecondCategory")]
        [DisplayName("NodeElementCollection")]
        [Browsable(true)]
        public ObservableCollection<NodeElement> NodeElementsCollection
        {
            get => _nodeElementsCollection;
            set => Set(ref _nodeElementsCollection, value);
        }

        [Category("Base")]
        [DisplayName("ReadOnly строковое значение")]
        [Description("SomeText")]
        [ReadOnly(true)]
        public string TypicalString
        {
            get => _typicalString;
            set => Set(ref _typicalString, value);
        }

        [BrowsableCategory("Base")]
        [DisplayName("bool")]
        public bool TestBool
        {
            get => _testBool;
            set => Set(ref _testBool, value);
        }

        [DisplayName("Collection")]
        [BrowsableCategory("Collection")]
        [Browsable(false)]
        public IEnumerable<NodeElement> CollectionElements
        {
            get => _collectionElements;
            set => Set(ref _collectionElements, value);
        }


        [Category("Enum")]
        [DisplayName("Enum")]
        public CollectionEnum Enum
        {
            get => _enum;
            set => Set(ref _enum, value);
        }

        [DisplayName("Elements")]
        [Category("Collect")]
        public List<NodeElement> ListOfElements
        {
            get => _elements;
            set => Set(ref _elements, value);
        }
    }
}