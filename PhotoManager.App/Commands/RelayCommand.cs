using System;
using System.Windows.Input;

namespace PhotoManager.App.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        // Constructor for commands with optional canExecute
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        // Constructor overload for parameterless actions
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            if (execute == null) throw new ArgumentNullException(nameof(execute));

            _execute = _ => execute();
            if (canExecute != null)
                _canExecute = _ => canExecute();
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

        public void Execute(object parameter) => _execute(parameter);

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();
    }
}
