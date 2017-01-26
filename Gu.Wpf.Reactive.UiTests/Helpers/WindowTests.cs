namespace Gu.Wpf.Reactive.UiTests
{
    using FlaUI.Core;
    using FlaUI.Core.AutomationElements;
    using FlaUI.Core.Input;
    using FlaUI.Core.WindowsAPI;
    using FlaUI.UIA3;

    using NUnit.Framework;

    public abstract class WindowTests
    {
        private Application application;
        private UIA3Automation automation;

        protected Window Window { get; private set; }

        protected abstract string WindowName { get; }

        [OneTimeSetUp]
        public virtual void OneTimeSetUp()
        {
            this.application = Application.AttachOrLaunch(Info.CreateStartInfo(this.WindowName));
            this.automation = new UIA3Automation();
            this.Window = this.application.GetMainWindow(this.automation);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Keyboard.ReleaseScanCode((ushort)ScanCodeShort.CONTROL, false);
            Keyboard.ReleaseScanCode((ushort)ScanCodeShort.SHIFT, false);
            this.application?.Dispose();
        }
    }
}