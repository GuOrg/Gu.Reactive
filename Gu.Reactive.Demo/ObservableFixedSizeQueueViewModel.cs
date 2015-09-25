namespace Gu.Reactive.Demo
{
    using System.Threading.Tasks;
    using System.Windows.Input;

    using Gu.Wpf.Reactive;

    public class ObservableFixedSizeQueueViewModel
    {
        private int _count = 0;

        public ObservableFixedSizeQueueViewModel()
        {
            EnqueueCommand = new RelayCommand(() => Queue.Enqueue(++_count), () => true);
            EnqueueOnThreadCommand = new RelayCommand(() => Task.Run(() => Queue.Enqueue(++_count)), () => true);
        }

        public ObservableFixedSizeQueue<int> Queue { get; } = new ObservableFixedSizeQueue<int>(5);

        public ICommand EnqueueCommand { get; private set; }

        public ICommand EnqueueOnThreadCommand { get; private set; }
    }
}