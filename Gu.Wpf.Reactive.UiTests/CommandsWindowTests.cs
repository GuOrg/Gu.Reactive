namespace Gu.Wpf.Reactive.UiTests
{
    using FlaUI.Core.AutomationElements;
    using FlaUI.Core.Definitions;

    using NUnit.Framework;

    public class CommandsWindowTests : WindowTests
    {
        protected override string WindowName { get; } = "CommandsWindow";

        private CheckBox CanExecuteCheckBox => this.Window
                                                         .FindFirstDescendant(x => x.ByText("CanExecute"))
                                                         .AsCheckBox();

        private Button RaiseCanExecuteButton => this.Window
                                               .FindFirstDescendant(x => x.ByText("RaiseCanExecute"))
                                               .AsButton();

        private TextBox ExecutedTextBox => this.Window
                                               .FindFirstDescendant(x => x.ByAutomationId("Executed"))
                                               .AsTextBox();

        private Button ManualRelayCommandButton => this.Window
                                                       .FindFirstDescendant(x => x.ByText("ManualRelayCommand"))
                                                       .AsButton();

        private Button ManualRelayCommandNoConditionButton => this.Window
                                               .FindFirstDescendant(x => x.ByText("ManualRelayCommandNoCondition"))
                                               .AsButton();

        private Button RelayCommandButton => this.Window
                                                 .FindFirstDescendant(x => x.ByText("RelayCommand"))
                                                 .AsButton();

        private Button RelayCommandWithParamaterButton => this.Window
                                         .FindFirstDescendant(x => x.ByText("RelayCommandWithParamater"))
                                         .AsButton();

        private Button RelayCommandNoConditionButton => this.Window
                                               .FindFirstDescendant(x => x.ByText("RelayCommandNoCondition"))
                                               .AsButton();

        private Button ObservingRelayCommandButton => this.Window
                                 .FindFirstDescendant(x => x.ByText("ObservingRelayCommand"))
                                 .AsButton();

        private Button ObservingRelayCommandWithParameterButton => this.Window
                         .FindFirstDescendant(x => x.ByText("ObservingRelayCommandWithParameter"))
                         .AsButton();

        private Button ConditionRelayCommandButton => this.Window
                         .FindFirstDescendant(x => x.ByText("ConditionRelayCommand"))
                         .AsButton();

        private Button ConditionRelayCommandWithParameterButton => this.Window
                         .FindFirstDescendant(x => x.ByText("ConditionRelayCommandWithParameter"))
                         .AsButton();

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
        public void RelayCommandWithParamaterCanExecuteWhenToggling()
        {
            Assert.AreEqual(false, this.RelayCommandWithParamaterButton.Properties.IsEnabled);
            this.CanExecuteCheckBox.State = ToggleState.On;
            Assert.AreEqual(true, this.RelayCommandWithParamaterButton.Properties.IsEnabled);

            this.RelayCommandWithParamaterButton.Click();
            Assert.AreEqual("RelayCommandWithParamater: RelayCommandWithParamater", this.ExecutedTextBox.Text);
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