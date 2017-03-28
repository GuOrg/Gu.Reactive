namespace Gu.Wpf.Reactive.Tests.FakesAndHelpers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;

    using Gu.Reactive.Internals;

    public class App : Application
    {
        public static void Start()
        {
            if (Current == null)
            {
                Task.Run(
                    () =>
                        {
                            var app = new App { ShutdownMode = ShutdownMode.OnExplicitShutdown };
                            app.Run();
                        });
                SpinWait.SpinUntil(() => Current != null, TimeSpan.FromMilliseconds(100));
                Ensure.NotNull(Current, nameof(Current));
            }
        }
    }
}