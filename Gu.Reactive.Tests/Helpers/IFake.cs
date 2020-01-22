namespace Gu.Reactive.Tests.Helpers
{
    using System.ComponentModel;

    public interface IFake : INotifyPropertyChanged
    {
        NotNotifying? NotNotifying { get; }

        int WriteOnly { set; }

        bool? IsTrueOrNull { get; set; }

        bool IsTrue { get; set; }

        string? Name { get; set; }

        Level? Next { get; set; }

        StructLevel StructLevel { get; set; }

        int Value { get; set; }

        Level? Method();
    }
}
