namespace Gu.Reactive.Benchmarks
{
    // ReSharper disable once ClassNeverInstantiated.Global
#pragma warning disable WPF1011 // Implement INotifyPropertyChanged.
    public class NotInpc
    {
        public bool IsTrue { get; set; }

        public bool? IsTrueOrNull { get; set; }

        public string Name { get; set; }

        public int Value { get; set; }
    }
#pragma warning restore WPF1011 // Implement INotifyPropertyChanged.
}