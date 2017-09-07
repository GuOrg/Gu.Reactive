namespace Gu.Wpf.Reactive.UiTests
{
    using System;
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    public abstract class WindowTests : IDisposable
    {
        private Application application;
        private bool disposed;

        protected Window Window { get; private set; }

        protected abstract string WindowName { get; }

        public void Restart()
        {
            for (var i = 0; i < 3; i++)
            {
                try
                {
                    this.application?.WaitWhileBusy();
                    this.application?.Dispose();
                    this.application = Application.AttachOrLaunch(Info.CreateStartInfo(this.WindowName));
                    this.application.WaitWhileMainHandleIsMissing();
                    this.Window = this.application.MainWindow;
                    this.application.WaitWhileBusy();
                    return;
                }
                catch
                {
                    // We get this on AppVeyor.
                    // Testin a retry strategy :)
                }
            }
        }

        [OneTimeSetUp]
        public virtual void OneTimeSetUp()
        {
            this.Restart();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.application?.Dispose();
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            if (disposing)
            {
                this.application?.Dispose();
            }
        }
    }
}