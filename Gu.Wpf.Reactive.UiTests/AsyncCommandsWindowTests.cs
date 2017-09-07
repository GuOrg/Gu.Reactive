namespace Gu.Wpf.Reactive.UiTests
{
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    public class AsyncCommandsWindowTests : WindowTests
    {
        protected override string WindowName { get; } = "AsyncCommandsWindow";

        private TextBox DelatTextBox => this.Window.FindTextBox("Delay");

        private Button AsyncCommandButton => this.Window
                                                 .FindFirstDescendant(x => x.ByText("AsyncCommand"))
                                                 .FindButton("Run");

        private Button AsyncThrowCommandButton => this.Window
                                         .FindFirstDescendant(x => x.ByText("AsyncThrowCommand"))
                                         .FindButton("Run");

        private Button AsyncParameterCommandButton => this.Window
                                 .FindFirstDescendant(x => x.ByText("AsyncParameterCommand"))
                                 .FindButton("Run");

        private Button AsyncCancelableCommandButton => this.Window
                         .FindFirstDescendant(x => x.ByText("AsyncCancelableCommand"))
                         .FindButton("Run");

        private Button AsyncCancelableParameterCommandButton => this.Window
                 .FindFirstDescendant(x => x.ByText("AsyncCancelableParameterCommand"))
                 .FindButton("Run");

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