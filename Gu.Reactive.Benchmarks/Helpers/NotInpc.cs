namespace Gu.Reactive.Benchmarks
{
    // ReSharper disable once ClassNeverInstantiated.Global
#pragma warning disable INPC001 // Implement INotifyPropertyChanged.
    public class NotInpc
    {
        public bool IsTrue { get; set; }

        public bool? IsTrueOrNull { get; set; }

        public string? Name { get; set; }

        public int Value { get; set; }
    }
#pragma warning restore INPC001 // Implement INotifyPropertyChanged.
}
