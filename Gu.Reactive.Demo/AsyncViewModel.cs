namespace Gu.Reactive.Demo
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Annotations;
    using Wpf.Reactive;

    public class AsyncViewModel : INotifyPropertyChanged
    {
        private int _delay = 2000;

        public AsyncViewModel()
        {
            AsyncCommand = new AsyncCommand(VoidTaskMethod) { ToolTipText = "AsyncCommand" };

            AsyncParameterCommand = new AsyncCommand<string>(VoidTaskMethod) { ToolTipText = "AsyncParameterCommand" };

            AsyncResultCommand = new AsyncResultCommand<int>(ResultTaskMethod) { ToolTipText = "AsyncResultCommand" };

            AsyncThrowCommand = new AsyncCommand(VoidTaskThrowMethod) { ToolTipText = "AsyncThrowCommand" };

            AsyncResultThrowCommand = new AsyncResultCommand<int>(ResultTaskThrowMethod)
                                          {
                                              ToolTipText = "AsyncResultThrowCommand"
                                          };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public AsyncCommand AsyncCommand { get; private set; }
       
        public AsyncCommand<string> AsyncParameterCommand { get; private set; }
        
        public AsyncResultCommand<int> AsyncResultCommand { get; private set; }

        public AsyncCommand AsyncThrowCommand { get; private set; }
        
        public AsyncResultCommand<int> AsyncResultThrowCommand { get; private set; }

        public int Delay
        {
            get
            {
                return _delay;
            }
            set
            {
                if (value == _delay)
                {
                    return;
                }
                _delay = value;
                OnPropertyChanged();
            }
        }

        public async Task VoidTaskMethod()
        {
            await Task.Delay(Delay);
        }

        private Task VoidTaskMethod(string arg)
        {
            return VoidTaskMethod();
        }

        public async Task VoidTaskThrowMethod()
        {
            await Task.Delay(Delay);
            throw new Exception("Something went wrong");
        }

        public async Task<int> ResultTaskMethod()
        {
            await Task.Delay(Delay);
            return 5;
        }

        public async Task<int> ResultTaskThrowMethod()
        {
            await Task.Delay(Delay);
            throw new Exception("Something went wrong");
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
