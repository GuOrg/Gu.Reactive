namespace Gu.Wpf.Reactive.UiTests
{
    using System;
    using FlaUI.Core;
    using FlaUI.Core.AutomationElements;
    using FlaUI.Core.Input;
    using FlaUI.Core.WindowsAPI;
    using FlaUI.UIA3;

    using NUnit.Framework;

    public abstract class WindowTests : IDisposable
    {
        private Application application;
        private UIA3Automation automation;
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
                    this.automation?.Dispose();
                    this.automation = new UIA3Automation();
                    this.Window = this.application.GetMainWindow(this.automation);
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
            Keyboard.ReleaseScanCode((ushort)ScanCodeShort.CONTROL, isExtendedKey: false);
            Keyboard.ReleaseScanCode((ushort)ScanCodeShort.SHIFT, isExtendedKey: false);
            
            try
            {
                this.application?.WaitWhileBusy();
                this.automation?.Dispose();
                this.application?.Dispose();
            }
            catch
            {
            }
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
                try
                {
                    this.application?.WaitWhileBusy();
                    this.automation?.Dispose();
                    this.application?.Dispose();
                }
                catch
                {
                }
            }
        }

        protected void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }
    }
}