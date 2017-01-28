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

        [SetUp]
        public void SetUp()
        {
            this.CanExecuteCheckBox.State = ToggleState.Off;
            this.RaiseCanExecuteButton.Click(false);
        }

        [Test]
        public void ManualRelayCommandUpdatesCanExecute()
        {
            Assert.AreEqual(false, this.ManualRelayCommandButton.Current.IsEnabled);
            this.CanExecuteCheckBox.State = ToggleState.On;
            Assert.AreEqual(false, this.ManualRelayCommandButton.Current.IsEnabled);
            this.RaiseCanExecuteButton.Click(false);
            Assert.AreEqual(true, this.ManualRelayCommandButton.Current.IsEnabled);
        }

        [Test]
        public void RelayCommandUpdatesCanExecuteWhenToggling()
        {
            Assert.AreEqual(false, this.RelayCommandButton.Current.IsEnabled);
            this.CanExecuteCheckBox.State = ToggleState.On;
            Assert.AreEqual(true, this.RelayCommandButton.Current.IsEnabled);
        }

        [Test]
        public void RelayCommandWithParamaterCanExecuteWhenToggling()
        {
            Assert.AreEqual(false, this.RelayCommandWithParamaterButton.Current.IsEnabled);
            this.CanExecuteCheckBox.State = ToggleState.On;
            Assert.AreEqual(true, this.RelayCommandWithParamaterButton.Current.IsEnabled);
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
        }
    }
}