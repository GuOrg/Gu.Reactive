namespace Gu.Wpf.Reactive.UiTests
{
    using FlaUI.Core.AutomationElements;
    using NUnit.Framework;

    public class AsyncCommandsWindowTests : WindowTests
    {
        protected override string WindowName { get; } = "AsyncCommandsWindow";

        private TextBox DelatTextBox => this.Window
                                         .FindFirstDescendant(x => x.ByAutomationId("Delay"))
                                         .AsTextBox();

        private Button AsyncCommandButton => this.Window
                                                 .FindFirstDescendant(x => x.ByText("AsyncCommand"))
                                                 .FindFirstDescendant(x => x.ByText("Run"))
                                                 .AsButton();

        private Button AsyncThrowCommandButton => this.Window
                                         .FindFirstDescendant(x => x.ByText("AsyncThrowCommand"))
                                         .FindFirstDescendant(x => x.ByText("Run"))
                                         .AsButton();

        private Button AsyncParameterCommandButton => this.Window
                                 .FindFirstDescendant(x => x.ByText("AsyncParameterCommand"))
                                 .FindFirstDescendant(x => x.ByText("Run"))
                                 .AsButton();

        private Button AsyncCancelableCommandButton => this.Window
                         .FindFirstDescendant(x => x.ByText("AsyncCancelableCommand"))
                         .FindFirstDescendant(x => x.ByText("Run"))
                         .AsButton();

        private Button AsyncCancelableParameterCommandButton => this.Window
                 .FindFirstDescendant(x => x.ByText("AsyncCancelableParameterCommand"))
                 .FindFirstDescendant(x => x.ByText("Run"))
                 .AsButton();

        [SetUp]
        public void SetUp()
        {
            this.DelatTextBox.Enter("100");
        }

        [Test]
        public void AsyncCommand()
        {
            this.AsyncCommandButton.Click();
            this.AsyncCommandButton.Click();
        }

        [Test]
        public void AsyncThrowCommand()
        {
            this.AsyncThrowCommandButton.Click();
            this.AsyncThrowCommandButton.Click();
        }

        [Test]
        public void AsyncParameterCommand()
        {
            this.AsyncParameterCommandButton.Click();
            this.AsyncParameterCommandButton.Click();
        }

        [Test]
        public void AsyncCancelableCommand()
        {
            this.AsyncCancelableCommandButton.Click();
            this.AsyncCancelableCommandButton.Click();
        }

        [Test]
        public void AsyncCancelableParameterCommand()
        {
            this.AsyncCancelableParameterCommandButton.Click();
            this.AsyncCancelableParameterCommandButton.Click();
        }
    }
}