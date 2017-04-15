namespace Gu.Wpf.Reactive.UiTests
{
    using FlaUI.Core.AutomationElements;
    using FlaUI.Core.Definitions;

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
            this.CanExecuteCheckBox.State = ToggleState.Off;
            this.RaiseCanExecuteButton.Click();
            this.ExecutedTextBox.Enter(string.Empty);
        }

        [Test]
        public void ManualRelayCommand()
        {
            Assert.AreEqual(false, this.ManualRelayCommandButton.Properties.IsEnabled);
            this.CanExecuteCheckBox.State = ToggleState.On;
            Assert.AreEqual(false, this.ManualRelayCommandButton.Properties.IsEnabled);
            this.RaiseCanExecuteButton.Click();
            Assert.AreEqual(true, this.ManualRelayCommandButton.Properties.IsEnabled);

            this.ManualRelayCommandButton.Click();
            Assert.AreEqual("ManualRelayCommand", this.ExecutedTextBox.Text);
        }

        [Test]
        public void RelayCommandUpdatesCanExecuteWhenToggling()
        {
            Assert.AreEqual(false, this.RelayCommandButton.Properties.IsEnabled);
            this.CanExecuteCheckBox.State = ToggleState.On;
            Assert.AreEqual(true, this.RelayCommandButton.Properties.IsEnabled);

            this.RelayCommandButton.Click();
            Assert.AreEqual("RelayCommand", this.ExecutedTextBox.Text);
        }

        [Test]
        public void RelayCommandWithParameterCanExecuteWhenToggling()
        {
            Assert.AreEqual(false, this.RelayCommandWithParameterButton.Properties.IsEnabled);
            this.CanExecuteCheckBox.State = ToggleState.On;
            Assert.AreEqual(true, this.RelayCommandWithParameterButton.Properties.IsEnabled);

            this.RelayCommandWithParameterButton.Click();
            Assert.AreEqual("RelayCommandWithParameter: RelayCommandWithParameter", this.ExecutedTextBox.Text);
        }

        [Test]
        public void ManualRelayCommandNoConditionCanAlwaysExecute()
        {
            Assert.AreEqual(true, this.ManualRelayCommandNoConditionButton.Properties.IsEnabled);
            this.CanExecuteCheckBox.State = ToggleState.On;
            Assert.AreEqual(true, this.ManualRelayCommandNoConditionButton.Properties.IsEnabled);
            this.RaiseCanExecuteButton.Click();
            Assert.AreEqual(true, this.ManualRelayCommandNoConditionButton.Properties.IsEnabled);
            this.CanExecuteCheckBox.State = ToggleState.Off;
            Assert.AreEqual(true, this.ManualRelayCommandNoConditionButton.Properties.IsEnabled);
            this.RaiseCanExecuteButton.Click();
            Assert.AreEqual(true, this.ManualRelayCommandNoConditionButton.Properties.IsEnabled);

            this.ManualRelayCommandNoConditionButton.Click();
            Assert.AreEqual("ManualRelayCommandNoCondition", this.ExecutedTextBox.Text);
        }

        [Test]
        public void RelayCommandNoConditionCanAlwaysExecute()
        {
            Assert.AreEqual(true, this.RelayCommandNoConditionButton.Properties.IsEnabled);
            this.CanExecuteCheckBox.State = ToggleState.On;
            Assert.AreEqual(true, this.RelayCommandNoConditionButton.Properties.IsEnabled);
            this.RaiseCanExecuteButton.Click();
            Assert.AreEqual(true, this.RelayCommandNoConditionButton.Properties.IsEnabled);
            this.CanExecuteCheckBox.State = ToggleState.Off;
            Assert.AreEqual(true, this.RelayCommandNoConditionButton.Properties.IsEnabled);
            this.RaiseCanExecuteButton.Click();
            Assert.AreEqual(true, this.RelayCommandNoConditionButton.Properties.IsEnabled);

            this.RelayCommandNoConditionButton.Click();
            Assert.AreEqual("RelayCommandNoCondition", this.ExecutedTextBox.Text);
        }

        [Test]
        public void ObservingRelayCommandUpdatesCanExecuteWhenToggling()
        {
            Assert.AreEqual(false, this.ObservingRelayCommandButton.Properties.IsEnabled);
            this.CanExecuteCheckBox.State = ToggleState.On;
            Assert.AreEqual(true, this.ObservingRelayCommandButton.Properties.IsEnabled);

            this.ObservingRelayCommandButton.Click();
            Assert.AreEqual("ObservingRelayCommand", this.ExecutedTextBox.Text);
        }

        [Test]
        public void ObservingRelayCommandWithParameterUpdatesCanExecuteWhenToggling()
        {
            Assert.AreEqual(false, this.ObservingRelayCommandWithParameterButton.Properties.IsEnabled);
            this.CanExecuteCheckBox.State = ToggleState.On;
            Assert.AreEqual(true, this.ObservingRelayCommandWithParameterButton.Properties.IsEnabled);

            this.ObservingRelayCommandWithParameterButton.Click();
            Assert.AreEqual("ObservingRelayCommandWithParameter: ObservingRelayCommandWithParameter", this.ExecutedTextBox.Text);
        }

        [Test]
        public void ConditionRelayCommandUpdatesCanExecuteWhenToggling()
        {
            Assert.AreEqual(false, this.ConditionRelayCommandButton.Properties.IsEnabled);
            this.CanExecuteCheckBox.State = ToggleState.On;
            Assert.AreEqual(true, this.ConditionRelayCommandButton.Properties.IsEnabled);

            this.ConditionRelayCommandButton.Click();
            Assert.AreEqual("ConditionRelayCommand", this.ExecutedTextBox.Text);
        }

        [Test]
        public void ConditionRelayCommandWithParameterUpdatesCanExecuteWhenToggling()
        {
            Assert.AreEqual(false, this.ConditionRelayCommandWithParameterButton.Properties.IsEnabled);
            this.CanExecuteCheckBox.State = ToggleState.On;
            Assert.AreEqual(true, this.ConditionRelayCommandWithParameterButton.Properties.IsEnabled);

            this.ConditionRelayCommandWithParameterButton.Click();
            Assert.AreEqual("ConditionRelayCommandWithParameter: ConditionRelayCommandWithParameter", this.ExecutedTextBox.Text);
        }
    }
}