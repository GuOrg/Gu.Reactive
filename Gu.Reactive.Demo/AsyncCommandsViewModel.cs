namespace Gu.Reactive.Demo
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Annotations;
    using Wpf.Reactive;

    public class AsyncCommandsViewModel : INotifyPropertyChanged
    {
        private int _delay = 500;

        private int _count;

        public AsyncCommandsViewModel()
        {
            AsyncCommand = new AsyncCommand(SimpleTask) { ToolTipText = "AsyncCommand" };

            AsyncCancelableCommand = new AsyncCommand(CancelableTask) { ToolTipText = "AsyncCancelableCommand" };

            AsyncParameterCommand = new AsyncCommand<string>(ParameterTask) { ToolTipText = "AsyncParameterCommand" };
            AsyncCancelableParameterCommand = new AsyncCommand<string>(CancelableParameterTask) { ToolTipText = "AsyncCancelableParameterCommand" };

            AsyncThrowCommand = new AsyncCommand(VoidTaskThrowMethod) { ToolTipText = "AsyncThrowCommand" };
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public AsyncCommand AsyncCommand { get; }

        public AsyncCommand AsyncCancelableCommand { get; }

        public AsyncCommand<string> AsyncParameterCommand { get; }

        public AsyncCommand<string> AsyncCancelableParameterCommand { get; }

        public AsyncCommand AsyncThrowCommand { get; }

        public int Count
        {
            get { return _count; }
            private set
            {
                if (value == _count)
                {
                    return;
                }
                _count = value;
                OnPropertyChanged();
            }
        }

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

        private async Task SimpleTask()
        {
            await Task.Delay(Delay);
        }

        private async Task CancelableTask(CancellationToken token)
        {
            Count = 0;
            for (int i = 0; i < 5; i++)
            {
                token.ThrowIfCancellationRequested();
                Count++;
                await Task.Delay(Delay);
            }
        }

        private Task ParameterTask(string arg)
        {
            return SimpleTask();
        }

        private Task CancelableParameterTask(string arg, CancellationToken token)
        {
            return CancelableTask(token);
        }

        public async Task VoidTaskThrowMethod()
        {
            await Task.Delay(Delay);
            throw new Exception("Something went wrong");
        }


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
