namespace Gu.Reactive.Demo
{
    using System.Threading.Tasks;
    using System.Windows.Input;

    using Gu.Wpf.Reactive;

    public class ObservableFixedSizeQueueViewModel
    {
        private int count;

        public ObservableFixedSizeQueueViewModel()
        {
            this.EnqueueCommand = new RelayCommand(() => this.Queue.Enqueue(++this.count), () => true);
            this.EnqueueOnThreadCommand = new RelayCommand(() => Task.Run(() => this.Queue.Enqueue(++this.count)), () => true);
        }

        public ObservableFixedSizeQueue<int> Queue { get; } = new ObservableFixedSizeQueue<int>(5);

        public ICommand EnqueueCommand { get; private set; }

        public ICommand EnqueueOnThreadCommand { get; private set; }
    }
}