namespace Gu.Reactive.Dummy
{
    using System;
    using System.Collections.ObjectModel;
    using System.Reactive.Linq;

    // ReSharper disable once ClassNeverInstantiated.Global
    class Program
    {
        static void Main(string[] args)
        {
            PauseWith("Starting");
            ObserveItemPropertyChanged();
            //ObservePropertyChanged();
            PauseWith("Press key to exit");
        }

        private static void ObserveItemPropertyChanged()
        {
            var item1 = new Fake { Name = "1" };
            var item2 = new Fake { Name = "2" };
            var collection = new ObservableCollection<Fake> { item1, item2 };

            var wr = new WeakReference(collection);
            int count = 0;
            PauseWith("Before subscribe");

            var subscription = collection.ObserveItemPropertyChanged(x => x.Name, false)
                                         .Subscribe(x => count++);

            PauseWith("After subscribe");
            collection = null;
            subscription.Dispose();
            PauseWith("After dispose");

            GC.Collect();
            Console.WriteLine("IsAlive: {0}: ", wr.IsAlive);
            PauseWith("After GC.Collect()");

            var s = subscription.ToString(); // touching it after GC.Collect for no optimizations
        }

        private static void ObservePropertyChanged()
        {
            PauseWith("Before loop");
            for (int i = 0; i < 10; i++)
            {
                var fake = new Fake { Next = new Level { Value = i } };

                var subscription = fake.ObservePropertyChanged(x => x.Next.Value)
                                       .Subscribe();
                subscription.Dispose();

                fake = null;
            }
            PauseWith("After loop");
            GC.Collect();
            PauseWith("After GC.Collect()");
            //var s = subscription.ToString(); // touching it after GC.Collect for no optimizations
        }

        private static void PauseWith(string s)
        {
            Console.WriteLine(s);
            Console.ReadKey();
        }
    }
}
