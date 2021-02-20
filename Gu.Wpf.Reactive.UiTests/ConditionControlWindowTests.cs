namespace Gu.Wpf.Reactive.UiTests
{
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    public static class ConditionControlWindowTests
    {
        private const string WindowName = "ConditionControlWindow";

        [Test]
        public static void DoesNotCrash()
        {
            using var app = Application.Launch(Info.ExeFileName, WindowName);
            var window = app.MainWindow;
            var button = window.FindButton("Clear");
            button.Invoke();
            Assert.AreEqual(1, window.FindDataGrid().RowCount);
        }
    }
}
