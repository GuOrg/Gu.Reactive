namespace Gu.Wpf.Reactive.UiTests
{
    using FlaUI.Core.AutomationElements;
    using FlaUI.Core.AutomationElements.Infrastructure;
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
            this.RaiseCanExecuteButton.Click(false);
            this.ExecutedTextBox.Enter(string.Empty);
        }

        [Test]
        public void ManualRelayCommand()
        {
            Assert.AreEqual(false, this.ManualRelayCommandButton.Current.IsEnabled);
            this.CanExecuteCheckBox.State = ToggleState.On;
            Assert.AreEqual(false, this.ManualRelayCommandButton.Current.IsEnabled);
            this.RaiseCanExecuteButton.Click(false);
            Assert.AreEqual(true, this.ManualRelayCommandButton.Current.IsEnabled);

            this.ManualRelayCommandButton.Click(false);
            Assert.AreEqual("ManualRelayCommand", this.ExecutedTextBox.Text);
        }

        [Test]
        public void RelayCommandUpdatesCanExecuteWhenToggling()
        {
            Assert.AreEqual(false, this.RelayCommandButton.Current.IsEnabled);
            this.CanExecuteCheckBox.State = ToggleState.On;
            Assert.AreEqual(true, this.RelayCommandButton.Current.IsEnabled);

            this.RelayCommandButton.Click(false);
            Assert.AreEqual("RelayCommand", this.ExecutedTextBox.Text);
        }

        [Test]
        public void RelayCommandWithParamaterCanExecuteWhenToggling()
        {
            Assert.AreEqual(false, this.RelayCommandWithParamaterButton.Current.IsEnabled);
            this.CanExecuteCheckBox.State = ToggleState.On;
            Assert.AreEqual(true, this.RelayCommandWithParamaterButton.Current.IsEnabled);

            this.RelayCommandWithParamaterButton.Click(false);
            Assert.AreEqual("RelayCommandWithParamater: RelayCommandWithParamater", this.ExecutedTextBox.Text);
        }

        [Test]
        public void ManualRelayCommandNoConditionCanAlwaysExecute()
        {
            Assert.AreEqual(true, this.ManualRelayCommandNoConditionButton.Current.IsEnabled);
            this.CanExecuteCheckBox.State = ToggleState.On;
            Assert.AreEqual(true, this.ManualRelayCommandNoConditionButton.Current.IsEnabled);
            this.RaiseCanExecuteButton.Click(false);
            Assert.AreEqual(true, this.ManualRelayCommandNoConditionButton.Current.IsEnabled);
            this.CanExecuteCheckBox.State = ToggleState.Off;
            Assert.AreEqual(true, this.ManualRelayCommandNoConditionButton.Current.IsEnabled);
            this.RaiseCanExecuteButton.Click(false);
            Assert.AreEqual(true, this.ManualRelayCommandNoConditionButton.Current.IsEnabled);

            this.ManualRelayCommandNoConditionButton.Click(false);
            Assert.AreEqual("ManualRelayCommandNoCondition", this.ExecutedTextBox.Text);
        }

        [Test]
        public void RelayCommandNoConditionCanAlwaysExecute()
        {
            Assert.AreEqual(true, this.RelayCommandNoConditionButton.Current.IsEnabled);
            this.CanExecuteCheckBox.State = ToggleState.On;
            Assert.AreEqual(true, this.RelayCommandNoConditionButton.Current.IsEnabled);
            this.RaiseCanExecuteButton.Click(false);
            Assert.AreEqual(true, this.RelayCommandNoConditionButton.Current.IsEnabled);
            this.CanExecuteCheckBox.State = ToggleState.Off;
            Assert.AreEqual(true, this.RelayCommandNoConditionButton.Current.IsEnabled);
            this.RaiseCanExecuteButton.Click(false);
            Assert.AreEqual(true, this.RelayCommandNoConditionButton.Current.IsEnabled);

            this.RelayCommandNoConditionButton.Click(false);
            Assert.AreEqual("RelayCommandNoCondition", this.ExecutedTextBox.Text);
        }

        [Test]
        public void ObservingRelayCommandUpdatesCanExecuteWhenToggling()
        {
            Assert.AreEqual(false, this.ObservingRelayCommandButton.Current.IsEnabled);
            this.CanExecuteCheckBox.State = ToggleState.On;
            Assert.AreEqual(true, this.ObservingRelayCommandButton.Current.IsEnabled);

            this.ObservingRelayCommandButton.Click(false);
            Assert.AreEqual("ObservingRelayCommand", this.ExecutedTextBox.Text);
        }

        [Test]
        public void ObservingRelayCommandWithParameterUpdatesCanExecuteWhenToggling()
        {
            Assert.AreEqual(false, this.ObservingRelayCommandWithParameterButton.Current.IsEnabled);
            this.CanExecuteCheckBox.State = ToggleState.On;
            Assert.AreEqual(true, this.ObservingRelayCommandWithParameterButton.Current.IsEnabled);

            this.ObservingRelayCommandWithParameterButton.Click(false);
            Assert.AreEqual("ObservingRelayCommandWithParameter: ObservingRelayCommandWithParameter", this.ExecutedTextBox.Text);
        }

        [Test]
        public void ConditionRelayCommandUpdatesCanExecuteWhenToggling()
        {
            Assert.AreEqual(false, this.ConditionRelayCommandButton.Current.IsEnabled);
            this.CanExecuteCheckBox.State = ToggleState.On;
            Assert.AreEqual(true, this.ConditionRelayCommandButton.Current.IsEnabled);

            this.ConditionRelayCommandButton.Click(false);
            Assert.AreEqual("ConditionRelayCommand", this.ExecutedTextBox.Text);
        }

        [Test]
        public void ConditionRelayCommandWithParameterUpdatesCanExecuteWhenToggling()
        {
            Assert.AreEqual(false, this.ConditionRelayCommandWithParameterButton.Current.IsEnabled);
            this.CanExecuteCheckBox.State = ToggleState.On;
            Assert.AreEqual(true, this.ConditionRelayCommandWithParameterButton.Current.IsEnabled);

            this.ConditionRelayCommandWithParameterButton.Click(false);
            Assert.AreEqual("ConditionRelayCommandWithParameter: ConditionRelayCommandWithParameter", this.ExecutedTextBox.Text);
        }
    }
}