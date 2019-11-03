using System;
using System.Windows.Input;

namespace UndoRedo
{
    public class RelayCommand : RelayCommand<object>
    {
        public RelayCommand(Action execute, Func<bool> canExecute) 
            : base(_ => execute(), _ => canExecute())
        {
        }

        public RelayCommand(Action execute)
           : base(_ => execute(), null)
        {
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> execute;
        private readonly Predicate<T> canExecute;

        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            this.execute = execute ?? throw new ArgumentNullException("execute");

            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => canExecute?.Invoke((T)parameter) ?? true;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            if (CanExecute(parameter))
                execute((T)parameter);
        }

    }
}
