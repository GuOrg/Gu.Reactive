namespace Gu.Wpf.Reactive
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;

    public class RelayCommand : IToolTipCommand
    {
        private readonly Action<object> _action;
        private readonly Predicate<object> _condition;
        private string _toolTipText;

        public RelayCommand(Action<object> action, Predicate<object> condition)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            _action = action;
            _condition = condition ?? (o => true);
        }

        public RelayCommand(Action action, Predicate<object> condition)
            :this(o=>action(),condition)
        {
        }

        public RelayCommand(Action action, Func<bool> condition)
            : this(o => action(),o=> condition())
        {
        }

        public RelayCommand(Action<object> action)
            : this(action, o => true)
        {
        }
        
        public RelayCommand(Action action)
            : this(o => action(), o => true)
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// http://stackoverflow.com/a/2588145/1069200
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public string ToolTipText
        {
            get { return _toolTipText; }
            set
            {
                if (Equals(value, _toolTipText))
                {
                    return;
                }
                _toolTipText = value;
                OnPropertyChanged();
            }
        }

        public bool CanExecute(object parameter)
        {
            return _condition(parameter);
        }

        public void Execute(object parameter)
        {
            _action(parameter);
        }
    
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}