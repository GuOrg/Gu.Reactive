namespace Gu.Reactive.Demo
{
    using System.Windows.Input;

    using Gu.Wpf.Reactive;

    public class ObservableFixedSizeQueueViewModel
    {
        private readonly ObservableFixedSizeQueue<int> _queue = new ObservableFixedSizeQueue<int>(10);
        private int _count = 0;

        public ObservableFixedSizeQueueViewModel()
        {
            EnqueueCommand = new RelayCommand(() => Queue.Enqueue(++_count), () => true);
        }

        public ObservableFixedSizeQueue<int> Queue
        {
            get { return _queue; }
        }

        public ICommand EnqueueCommand { get; private set; }
    }
}