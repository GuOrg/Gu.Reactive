namespace Gu.Wpf.Reactive
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Input;

    public class ManualRelayCommand : IToolTipCommand
    {
        private readonly Action<object> _action;
        private readonly Predicate<object> _condition;
        private readonly bool _raiseCanExecuteOnDispatcher;
        private string _toolTipText;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="condition"></param>
        /// <param name="raiseCanExecuteOnDispatcher">Default true, use false in tests</param>
        public ManualRelayCommand(Action<object> action, Predicate<object> condition, bool raiseCanExecuteOnDispatcher = true)
        {
            _action = action;
            _condition = condition ?? (_ => true);
            _raiseCanExecuteOnDispatcher = raiseCanExecuteOnDispatcher;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="raiseCanExecuteOnDispatcher">Default true, use false in tests</param>
        public ManualRelayCommand(Action<object> action, bool raiseCanExecuteOnDispatcher = true)
            : this(action, _ => true, raiseCanExecuteOnDispatcher)
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler CanExecuteChanged
        {
            add
            {
                InternalCanExecuteChangedEventManager.AddHandler(this, value);
            }
            remove
            {
                InternalCanExecuteChangedEventManager.RemoveHandler(this, value);
            }
        }

        private event EventHandler InternalCanExecuteChanged;

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

        public void RaiseCanExecuteChanged()
        {
            var handler = this.InternalCanExecuteChanged;
            if (handler != null)
            {
                var application = Application.Current;
                if (_raiseCanExecuteOnDispatcher && application != null && application.Dispatcher != null)
                {
                    application.Dispatcher.BeginInvoke(new Action(() => handler(this, new EventArgs())));
                }
                else
                {
                    handler(this, new EventArgs());
                }
            }
        }

        public bool CanExecute(object parameter)
        {
            return _condition(parameter);
        }

        public void Execute(object parameter)
        {
            _action(parameter);
            RaiseCanExecuteChanged();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private class InternalCanExecuteChangedEventManager : WeakEventManager
        {
            private static readonly InternalCanExecuteChangedEventManager Manager = new InternalCanExecuteChangedEventManager();
            static InternalCanExecuteChangedEventManager()
            {
                SetCurrentManager(typeof(InternalCanExecuteChangedEventManager), Manager);
            }
            internal static void AddHandler(ManualRelayCommand source, EventHandler handler)
            {
                Manager.ProtectedAddHandler(source, handler);
            }
            internal static void RemoveHandler(ManualRelayCommand source, EventHandler handler)
            {
                Manager.ProtectedRemoveHandler(source, handler);
            }
            ////protected override ListenerList NewListenerList()
            ////{
            ////    return new ListenerList();
            ////}
            protected override void StartListening(object source)
            {
                ((ManualRelayCommand)source).InternalCanExecuteChanged += this.DeliverEvent;
            }
            protected override void StopListening(object source)
            {
                ((ManualRelayCommand)source).InternalCanExecuteChanged -= this.DeliverEvent;
            }
        }
    }
}