using System.Windows.Input;
using System;

namespace LoL_Assist_WAPP.Commands
{
    public class Command: ICommand
    {
        private readonly Action<object> r_execute;
        private readonly Func<object, bool> r_canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public Command(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.r_execute = execute;
            this.r_canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return r_canExecute == null || r_canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            r_execute(parameter);
        }
    }
}
