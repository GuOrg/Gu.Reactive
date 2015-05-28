namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Reactive.Concurrency;
    using System.Runtime.CompilerServices;

    using Gu.Reactive;
    using Gu.Wpf.Reactive.Annotations;

    [Serializable]
    public class ObservableFixedSizeQueue<T> : FixedSizedQueue<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private static readonly PropertyChangedEventArgs CountPropertyChangedEventArgs = new PropertyChangedEventArgs("Count");
        public ObservableFixedSizeQueue(int size)
            : base(size)
        {
        }

        [field: NonSerialized]
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public override void Enqueue(T item)
        {
            bool flag = true;
            if (Count >= Size)
            {
                flag = false;
                T overflow;
                while (Count >= Size && TryDequeue(out overflow))
                {
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, overflow, 0));
                }
            }
            base.Enqueue(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, Count - 1));

            if (flag)
            {
                OnPropertyChanged(CountPropertyChangedEventArgs);
            }
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var handler = CollectionChanged;
            handler.InvokeOnDispatcher(this, e);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
