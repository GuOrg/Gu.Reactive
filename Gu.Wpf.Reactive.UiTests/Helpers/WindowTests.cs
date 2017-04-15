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
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    this.application?.Dispose();
                    this.application = Application.AttachOrLaunch(Info.CreateStartInfo(this.WindowName));
                    this.automation?.Dispose();
                    this.automation = new UIA3Automation();
                    this.Window = this.application.GetMainWindow(this.automation);
                    return;
                }
                catch (System.ComponentModel.Win32Exception)
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
            if (this.Window != null)
            {
                Helpers.WaitUntilResponsive(this.Window);
            }

            this.automation?.Dispose();
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
                this.automation?.Dispose();
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