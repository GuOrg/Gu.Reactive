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
            this.AsyncCommandButton.Click(false);
            this.AsyncCommandButton.Click(false);
        }

        [Test]
        public void AsyncThrowCommand()
        {
            this.AsyncThrowCommandButton.Click(false);
            this.AsyncThrowCommandButton.Click(false);
        }

        [Test]
        public void AsyncParameterCommand()
        {
            this.AsyncParameterCommandButton.Click(false);
            this.AsyncParameterCommandButton.Click(false);
        }

        [Test]
        public void AsyncCancelableCommand()
        {
            this.AsyncCancelableCommandButton.Click(false);
            this.AsyncCancelableCommandButton.Click(false);
        }

        [Test]
        public void AsyncCancelableParameterCommand()
        {
            this.AsyncCancelableParameterCommandButton.Click(false);
            this.AsyncCancelableParameterCommandButton.Click(false);
        }
    }
}