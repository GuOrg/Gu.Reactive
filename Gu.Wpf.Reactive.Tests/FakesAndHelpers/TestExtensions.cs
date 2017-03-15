namespace Gu.Wpf.Reactive.Tests.FakesAndHelpers
{
    using System.Windows.Threading;

    public static class TestExtensions
    {
        public static DispatcherOperation SimulateYield(this Dispatcher dispatcher) => dispatcher.InvokeAsync(() => { }, DispatcherPriority.Background);
    }
}