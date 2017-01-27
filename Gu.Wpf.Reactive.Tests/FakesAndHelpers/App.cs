namespace Gu.Wpf.Reactive.Tests.FakesAndHelpers
{
    using System.Threading.Tasks;
    using System.Windows;

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
            }
        }
    }
}