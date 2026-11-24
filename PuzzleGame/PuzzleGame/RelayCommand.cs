using System;
using System.Windows.Input;

namespace PuzzleGame
{
    // чтобы кнопки в XAML можно было привязывать через Command="{Binding ...}".
    // CanExecute не поддерживается: в этой игре все кнопки всегда активны.
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;

        public RelayCommand(Action<object> execute)
        {
            _execute = execute;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) => _execute(parameter);

        // ICommand требует это событие, но у нас CanExecute всегда true,
        // поэтому уведомлять не о чем — пустые add/remove.
        public event EventHandler CanExecuteChanged
        {
            add { }
            remove { }
        }
    }
}