using System.ComponentModel;

namespace Common.Core
{
    /// <summary>
    /// Интерфейс необходимый для отделения реализации реализации
    /// INotifyPropertyChanged и INotifyPropertyChanging от классов, которым эта реализация требуется.
    /// </summary>
    /// <remarks>Реализовывать только явно!</remarks>
    public interface IObservableObject : INotifyPropertyChanged, INotifyPropertyChanging //, IWeakEventListener
    {
        /// <summary>
        /// Свойства, зависимые от текущего свойства.
        /// </summary>
        Dictionary<string, List<string>> PropertyDependencies { get; }

        /// <summary>
        /// Свойства зависимые от 
        /// </summary>
        Dictionary<string, List<string>> CollectionPropertyDependencies { get; }

        /// <summary>
        /// Обработчик события завершения изменения значения свойства.
        /// </summary>
        PropertyChangedEventHandler PropertyChangedHandler { get; }

        /// <summary>
        /// Обработчик события начала изменения значения свойства.
        /// </summary>
        PropertyChangingEventHandler PropertyChangingHandler { get; }
    }
}