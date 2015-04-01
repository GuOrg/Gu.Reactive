namespace Gu.Reactive.Dummy
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using System.Reactive.Linq;
    using Gu.Reactive.Annotations;

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting");
            Console.ReadKey();
            var item1 = new Fake { Name = "1" };
            var item2 = new Fake { Name = "2" };
            var collection = new ObservableCollection<Fake> { item1, item2 };

            var wr = new WeakReference(collection);
            int count = 0;
            Console.WriteLine("Before subscribe");
            Console.ReadKey();
            var subscription = collection.ObserveItemPropertyChanged(x => x.Name, false)
                                         .Subscribe(x => count++);
            Console.WriteLine("After subscribe");
            Console.ReadKey();
            collection = null;
            subscription.Dispose();
            Console.WriteLine("After dispose");
            Console.ReadKey();

            GC.Collect();
            Console.WriteLine("IsAlive: {0}: ", wr.IsAlive);
            Console.WriteLine("After GC.Collect()");
            Console.ReadKey();

            GC.Collect(3, GCCollectionMode.Forced, true);
            Console.WriteLine("IsAlive: {0}: ", wr.IsAlive);
            Console.WriteLine("After GC.Collect(3, GCCollectionMode.Forced, true);");
            Console.ReadKey();

            var status = GC.WaitForFullGCComplete();
            Console.WriteLine("After GC.WaitForFullGCComplete(): {0}", status);
            Console.ReadKey();
            Console.WriteLine(wr.IsAlive);
            var s = subscription.ToString(); // touching it after GC.Collect for no optimizations
            Console.ReadKey();
        }
    }

    internal class Fake : INotifyPropertyChanged
    {
        private string _name;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
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
    }
}
