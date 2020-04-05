using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Xce.UndoRedo
{
    public class AsyncCommand : AsyncCommand<object>
    {
        public AsyncCommand(Func<Task> execute, Func<bool> canExecute = null)
            : base(_ => execute(), canExecute == null ? (Predicate<object>) null : (_) => canExecute())
        {
        }

        public AsyncCommand(Action execute, Func<bool> canExecute = null)
          : base(_ => execute(), canExecute == null ? (Predicate<object>) null : (_) => canExecute())
        {
        }
    }

    public class AsyncCommand<T> : ICommand
    {
        private readonly Func<T, Task> execute;
        private readonly Predicate<T> canExecute;

        public AsyncCommand(Action<T> execute, Predicate<T> canExecute = null)
            : this((x) => Task.Run(() => execute(x)), canExecute)
        {
        }

        public AsyncCommand(Func<T, Task> execute, Predicate<T> canExecute = null)
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
