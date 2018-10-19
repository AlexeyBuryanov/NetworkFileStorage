using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Client
{
    public class RelayCommand : ICommand
    {
        private Action<object>     _execute;
        private Func<object, bool> _canExecute;

        public event EventHandler CanExecuteChanged {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        } // CanExecuteChanged

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute    = execute;
            _canExecute = canExecute;
        } // RelayCommand

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        } // CanExecute

        public void Execute(object parameter)
        {
            _execute(parameter);
        } // Execute
    } // class RelayCommand
}
