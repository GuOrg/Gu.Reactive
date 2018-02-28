namespace Gu.Wpf.Reactive.UiTests
{
    using System;
    using System.IO;
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    public class AsyncCommandsWindowTests
    {
        private static readonly string WindowName = "AsyncCommandsWindow";

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Application.KillLaunched(Info.ExeFileName);
        }

        [TestCase("AsyncCommand")]
        [TestCase("AsyncThrowCommand")]
        [TestCase("AsyncParameterCommand")]
        [TestCase("AsyncCancelableCommand")]
        [TestCase("AsyncCancelableParameterCommand")]
        public void ClickOnce(string header)
        {
            using (var app = Application.AttachOrLaunch(Info.ExeFileName, WindowName))
            {
                var window = app.MainWindow;
                window.FindTextBox("Delay").Text = "100";
                var button = window.FindGroupBox(header)
                                   .FindButton("Run");
                button.Click();
            }
        }

        [TestCase("AsyncCommand")]
        [TestCase("AsyncThrowCommand")]
        [TestCase("AsyncParameterCommand")]
        [TestCase("AsyncCancelableCommand")]
        [TestCase("AsyncCancelableParameterCommand")]
        public void ClickTwice(string header)
        {
            using (var app = Application.AttachOrLaunch(Info.ExeFileName, WindowName))
            {
                var window = app.MainWindow;
                window.FindTextBox("Delay").Text = "100";
                var button = window.FindGroupBox(header)
                                   .FindButton("Run");
                button.Click();
                button.Click();
            }
        }

        [TestCase("AsyncCancelableCommand")]
        [TestCase("AsyncCancelableParameterCommand")]
        public void CancelCommand(string header)
        {
            try
            {
                using (var app = Application.AttachOrLaunch(Info.ExeFileName, WindowName))
                {
                    var window = app.MainWindow;
                    window.FindTextBox("Delay").Text = "200";
                    var groupBox = window.FindGroupBox(header);
                    var button = groupBox.FindButton("Run");
                    var cancelButton = groupBox.FindButton("Cancel");

                    Assert.AreEqual(false, cancelButton.IsEnabled);
                    button.Invoke();
                    Assert.AreEqual(true, cancelButton.IsEnabled);
                }
            }
            catch (TimeoutException)
            {
                Capture.ScreenToFile(Path.Combine(Path.GetTempPath(), "AsyncCommandsWindowTests.CancelCommand.png"));
                throw;
            }
        }
    }
}
