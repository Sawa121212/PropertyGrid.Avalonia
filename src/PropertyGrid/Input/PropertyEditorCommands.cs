using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using ReactiveUI;

namespace PropertyGrid.Input
{
    public sealed class PropertyEditorCommands : ICreatesCommandBinding
    {
        public IDisposable BindCommandToObject(ICommand command, object target, IObservable<object> commandParameter)
        {
            return BindCommandToObject(command, target, commandParameter);
        }

        public IDisposable BindCommandToObject<TEventArgs>(ICommand command, object target, IObservable<object> commandParameter, string eventName)
        {
            var button = (PropertyGrid)target;
            var disposables = new CompositeDisposable();

            disposables.Add(Observable.FromEventPattern(button, eventName)
                .Subscribe(_ => command.Execute(null)));
            disposables.Add(Observable.FromEventPattern(command, "CanExecuteChanged")
                .Subscribe(x =>
                    button.IsEnabled = command.CanExecute(null)));



            return disposables;
        }

        public int GetAffinityForObject(Type type, bool hasEventTarget)
        {
            return typeof(PropertyGrid).IsAssignableFrom(type) ? 2 : 0;
        }
    }
}