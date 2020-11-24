namespace Gu.Wpf.Reactive.Tests.FakesAndHelpers
{
    using System.Windows.Threading;

    public static class TestExtensions
    {
#pragma warning disable VSTHRD001 // Avoid legacy thread switching APIs
        public static DispatcherOperation SimulateYield(this Dispatcher dispatcher) => dispatcher.InvokeAsync(() => { }, DispatcherPriority.Background);
#pragma warning restore VSTHRD001 // Avoid legacy thread switching APIs
    }
}
