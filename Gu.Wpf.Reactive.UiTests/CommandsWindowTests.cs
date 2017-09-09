namespace Gu.Wpf.Reactive.UiTests
{
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    public class CommandsWindowTests
    {
        private static readonly string WindowName = "CommandsWindow";

        [SetUp]
        public void SetUp()
        {
            if (Application.TryAttach(Info.ExeFileName, WindowName, out var app))
            {
                using (app)
                {
                    var window = app.MainWindow;
                    var canExecuteCheckBox = window.FindCheckBox("CanExecute");
                    canExecuteCheckBox.IsChecked = false;

                    var raiseCanExecuteButton = window.FindButton("RaiseCanExecute");
                    raiseCanExecuteButton.Invoke();

                    var executedTextBox = window.FindTextBox("Executed");
                    executedTextBox.Text = string.Empty;
                }
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Application.KillLaunched(Info.ExeFileName);
        }

        [Test]
        public void ManualRelayCommand()
        {
            using (var app = Application.AttachOrLaunch(Info.ExeFileName, WindowName))
            {
                var window = app.MainWindow;
                var button = window.FindButton("ManualRelayCommand");
                var canExecuteCheckBox = window.FindCheckBox("CanExecute");
                var raiseCanExecuteButton = window.FindButton("RaiseCanExecute");

                Assert.AreEqual(false, button.IsEnabled);

                canExecuteCheckBox.IsChecked = true;
                Assert.AreEqual(false, button.IsEnabled);

                raiseCanExecuteButton.Invoke();
                Assert.AreEqual(true, button.IsEnabled);

                button.Invoke();
                Assert.AreEqual("ManualRelayCommand", window.FindTextBox("Executed").Text);
            }
        }

        [TestCase("RelayCommand", "RelayCommand")]
        [TestCase("RelayCommandWithParameter", "RelayCommandWithParameter: RelayCommandWithParameter")]
        [TestCase("ObservingRelayCommand", "ObservingRelayCommand")]
        [TestCase("ConditionRelayCommand", "ConditionRelayCommand")]
        [TestCase("ConditionRelayCommandWithParameter", "ConditionRelayCommandWithParameter: ConditionRelayCommandWithParameter")]
        [TestCase("ObservingRelayCommandWithParameter", "ObservingRelayCommandWithParameter: ObservingRelayCommandWithParameter")]
        public void UpdatesCanExecuteWhenToggling(string buttonContent, string expected)
        {
            using (var app = Application.AttachOrLaunch(Info.ExeFileName, WindowName))
            {
                var window = app.MainWindow;
                var button = window.FindButton(buttonContent);
                var canExecuteCheckBox = window.FindCheckBox("CanExecute");

                Assert.AreEqual(false, button.IsEnabled);
                canExecuteCheckBox.IsChecked = true;
                window.FindButton("RaiseCanExecute").Invoke();
                Assert.AreEqual(true, button.IsEnabled);

                button.Invoke();
                Assert.AreEqual(expected, window.FindTextBox("Executed").Text);
            }
        }

        [TestCase("ManualRelayCommandNoCondition", "ManualRelayCommandNoCondition")]
        [TestCase("RelayCommandNoCondition", "RelayCommandNoCondition")]
        public void CanAlwaysExecute(string buttonContent, string expected)
        {
            using (var app = Application.AttachOrLaunch(Info.ExeFileName, WindowName))
            {
                var window = app.MainWindow;
                var button = window.FindButton(buttonContent);
                Assert.AreEqual(true, button.IsEnabled);

                var canExecuteCheckBox = window.FindCheckBox("CanExecute");
                canExecuteCheckBox.IsChecked = true;
                Assert.AreEqual(true, button.IsEnabled);

                var raiseCanExecuteButton = window.FindButton("RaiseCanExecute");
                raiseCanExecuteButton.Invoke();
                Assert.AreEqual(true, button.IsEnabled);

                canExecuteCheckBox.IsChecked = false;
                Assert.AreEqual(true, button.IsEnabled);

                raiseCanExecuteButton.Invoke();
                Assert.AreEqual(true, button.IsEnabled);

                button.Invoke();
                Assert.AreEqual(expected, window.FindTextBox("Executed").Text);
            }
        }
    }
}