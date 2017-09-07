namespace Gu.Wpf.Reactive.UiTests
{
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    public class CommandsWindowTests : WindowTests
    {
        protected override string WindowName { get; } = "CommandsWindow";

        private CheckBox CanExecuteCheckBox => this.Window.FindCheckBox("CanExecute");

        private Button RaiseCanExecuteButton => this.Window.FindButton("RaiseCanExecute");

        private TextBox ExecutedTextBox => this.Window.FindTextBox("Executed");

        private Button ManualRelayCommandButton => this.Window.FindButton("ManualRelayCommand");

        private Button ManualRelayCommandNoConditionButton => this.Window.FindButton("ManualRelayCommandNoCondition");

        private Button RelayCommandButton => this.Window.FindButton("RelayCommand");

        private Button RelayCommandWithParameterButton => this.Window.FindButton("RelayCommandWithParameter");

        private Button RelayCommandNoConditionButton => this.Window.FindButton("RelayCommandNoCondition");

        private Button ObservingRelayCommandButton => this.Window.FindButton("ObservingRelayCommand");

        private Button ObservingRelayCommandWithParameterButton => this.Window.FindButton("ObservingRelayCommandWithParameter");

        private Button ConditionRelayCommandButton => this.Window.FindButton("ConditionRelayCommand");

        private Button ConditionRelayCommandWithParameterButton => this.Window.FindButton("ConditionRelayCommandWithParameter");

        [SetUp]
        public void SetUp()
        {
            this.CanExecuteCheckBox.IsChecked = false;
            this.RaiseCanExecuteButton.Click();
            this.ExecutedTextBox.Enter(string.Empty);
        }

        [Test]
        public void ManualRelayCommand()
        {
            Assert.AreEqual(false, this.ManualRelayCommandButton.IsEnabled);
            this.CanExecuteCheckBox.IsChecked = true;
            Assert.AreEqual(false, this.ManualRelayCommandButton.IsEnabled);
            this.RaiseCanExecuteButton.Click();
            Assert.AreEqual(true, this.ManualRelayCommandButton.IsEnabled);

            this.ManualRelayCommandButton.Click();
            Assert.AreEqual("ManualRelayCommand", this.ExecutedTextBox.Text);
        }

        [Test]
        public void RelayCommandUpdatesCanExecuteWhenToggling()
        {
            Assert.AreEqual(false, this.RelayCommandButton.IsEnabled);
            this.CanExecuteCheckBox.IsChecked = true;
            Assert.AreEqual(true, this.RelayCommandButton.IsEnabled);

            this.RelayCommandButton.Click();
            Assert.AreEqual("RelayCommand", this.ExecutedTextBox.Text);
        }

        [Test]
        public void RelayCommandWithParameterCanExecuteWhenToggling()
        {
            Assert.AreEqual(false, this.RelayCommandWithParameterButton.IsEnabled);
            this.CanExecuteCheckBox.IsChecked = true;
            Assert.AreEqual(true, this.RelayCommandWithParameterButton.IsEnabled);

            this.RelayCommandWithParameterButton.Click();
            Assert.AreEqual("RelayCommandWithParameter: RelayCommandWithParameter", this.ExecutedTextBox.Text);
        }

        [Test]
        public void ManualRelayCommandNoConditionCanAlwaysExecute()
        {
            Assert.AreEqual(true, this.ManualRelayCommandNoConditionButton.IsEnabled);
            this.CanExecuteCheckBox.IsChecked = true;
            Assert.AreEqual(true, this.ManualRelayCommandNoConditionButton.IsEnabled);
            this.RaiseCanExecuteButton.Click();
            Assert.AreEqual(true, this.ManualRelayCommandNoConditionButton.IsEnabled);
            this.CanExecuteCheckBox.IsChecked = false;
            Assert.AreEqual(true, this.ManualRelayCommandNoConditionButton.IsEnabled);
            this.RaiseCanExecuteButton.Click();
            Assert.AreEqual(true, this.ManualRelayCommandNoConditionButton.IsEnabled);

            this.ManualRelayCommandNoConditionButton.Click();
            Assert.AreEqual("ManualRelayCommandNoCondition", this.ExecutedTextBox.Text);
        }

        [Test]
        public void RelayCommandNoConditionCanAlwaysExecute()
        {
            Assert.AreEqual(true, this.RelayCommandNoConditionButton.IsEnabled);
            this.CanExecuteCheckBox.IsChecked = true;
            Assert.AreEqual(true, this.RelayCommandNoConditionButton.IsEnabled);
            this.RaiseCanExecuteButton.Click();
            Assert.AreEqual(true, this.RelayCommandNoConditionButton.IsEnabled);
            this.CanExecuteCheckBox.IsChecked = false;
            Assert.AreEqual(true, this.RelayCommandNoConditionButton.IsEnabled);
            this.RaiseCanExecuteButton.Click();
            Assert.AreEqual(true, this.RelayCommandNoConditionButton.IsEnabled);

            this.RelayCommandNoConditionButton.Click();
            Assert.AreEqual("RelayCommandNoCondition", this.ExecutedTextBox.Text);
        }

        [Test]
        public void ObservingRelayCommandUpdatesCanExecuteWhenToggling()
        {
            Assert.AreEqual(false, this.ObservingRelayCommandButton.IsEnabled);
            this.CanExecuteCheckBox.IsChecked = true;
            Assert.AreEqual(true, this.ObservingRelayCommandButton.IsEnabled);

            this.ObservingRelayCommandButton.Click();
            Assert.AreEqual("ObservingRelayCommand", this.ExecutedTextBox.Text);
        }

        [Test]
        public void ObservingRelayCommandWithParameterUpdatesCanExecuteWhenToggling()
        {
            Assert.AreEqual(false, this.ObservingRelayCommandWithParameterButton.IsEnabled);
            this.CanExecuteCheckBox.IsChecked = true;
            Assert.AreEqual(true, this.ObservingRelayCommandWithParameterButton.IsEnabled);

            this.ObservingRelayCommandWithParameterButton.Click();
            Assert.AreEqual("ObservingRelayCommandWithParameter: ObservingRelayCommandWithParameter", this.ExecutedTextBox.Text);
        }

        [Test]
        public void ConditionRelayCommandUpdatesCanExecuteWhenToggling()
        {
            Assert.AreEqual(false, this.ConditionRelayCommandButton.IsEnabled);
            this.CanExecuteCheckBox.IsChecked = true;
            Assert.AreEqual(true, this.ConditionRelayCommandButton.IsEnabled);

            this.ConditionRelayCommandButton.Click();
            Assert.AreEqual("ConditionRelayCommand", this.ExecutedTextBox.Text);
        }

        [Test]
        public void ConditionRelayCommandWithParameterUpdatesCanExecuteWhenToggling()
        {
            Assert.AreEqual(false, this.ConditionRelayCommandWithParameterButton.IsEnabled);
            this.CanExecuteCheckBox.IsChecked = true;
            Assert.AreEqual(true, this.ConditionRelayCommandWithParameterButton.IsEnabled);

            this.ConditionRelayCommandWithParameterButton.Click();
            Assert.AreEqual("ConditionRelayCommandWithParameter: ConditionRelayCommandWithParameter", this.ExecutedTextBox.Text);
        }
    }
}