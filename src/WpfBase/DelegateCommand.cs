using System.Data.SqlTypes;
using System.Windows.Input;

namespace BlackSugar.Wpf
{
    public class DelegateCommand : ICommand
    {
        private readonly Action? execute;
        private readonly Func<bool>? canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public DelegateCommand(Action? execute) : this(execute, () => true) { }

        public DelegateCommand(Action? execute, Func<bool>? canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => canExecute?.Invoke() == true;

        public void Execute(object? parameter) => execute?.Invoke();
    }

    public class DelegateCommand<T> : ICommand
    {
        private readonly Action<T>? execute;
        private readonly Func<T?, bool>? canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public DelegateCommand(Action<T>? execute) : this(execute, null) { }

        public DelegateCommand(Action<T>? execute, Func<T?, bool>? canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            
            var arg = parameter ?? default(T); 

            return canExecute == null ? true : canExecute((T)arg);
        } 

        public void Execute(object? parameter) => execute?.Invoke((T)parameter);
    }
}
