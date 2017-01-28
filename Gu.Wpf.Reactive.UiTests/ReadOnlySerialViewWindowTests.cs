namespace Gu.Wpf.Reactive.UiTests
{
    using System.Collections.Generic;
    using System.Linq;

    using FlaUI.Core.AutomationElements;
    using FlaUI.Core.AutomationElements.Infrastructure;
    using FlaUI.Core.Definitions;

    using NUnit.Framework;

    public class ReadOnlySerialViewWindowTests : WindowTests
    {
        protected override string WindowName { get; } = "ReadOnlySerialViewWindow";

        private Table ListBox => this.Window
                                     .FindFirstDescendant(x => x.ByText("ListBox"))
                                     .FindFirstDescendant(x => x.ByControlType(ControlType.List))
                                     .AsTable();

        private Grid DataGrid => this.Window
                                     .FindFirstDescendant(x => x.ByText("DataGrid"))
                                     .FindFirstDescendant(x => x.ByControlType(ControlType.DataGrid))
                                     .AsGrid();

        private Button ClearButton => this.Window
                                          .FindFirstDescendant(x => x.ByText("Clear"))
                                          .AsButton();

        private TextBox ItemsTextBox => this.Window
                                   .FindFirstDescendant(x => x.ByAutomationId("Items"))
                                   .AsTextBox();

        private Button UpdateButton => this.Window
                                           .FindFirstDescendant(x => x.ByText("Update"))
                                           .AsButton();

        private Button ClearSourceButton => this.Window
                                            .FindFirstDescendant(x => x.ByText("ClearSource"))
                                            .AsButton();

        private IEnumerable<Label> ViewChanges => this.Window
                                                      .FindFirstDescendant(x => x.ByText("ViewChanges"))
                                                      .FindAllChildren()
                                                      .Skip(1)
                                                      .Select(x => x.AsLabel());

        [SetUp]
        public void SetUp()
        {
            this.ClearButton.Click(false);
            FlaUI.Core.Input.Helpers.WaitUntilInputIsProcessed();
            this.ClearButton.Click(false);
            FlaUI.Core.Input.Helpers.WaitUntilInputIsProcessed();
        }

        [Test]
        public void Initializes()
        {
            this.Restart();
            CollectionAssert.IsEmpty(this.ListBox.RowValues());
            CollectionAssert.IsEmpty(this.DataGrid.ColumnValues(0));
            CollectionAssert.IsEmpty(this.ViewChanges.Select(x => x.Text));
        }

        [Test]
        public void UpdateWithOne()
        {
            this.ItemsTextBox.Enter("1");
            this.UpdateButton.Click(false);
            CollectionAssert.AreEqual(new[] { "1" }, this.ListBox.Rows.Select(x => x.Cells[0].AsLabel().Text));
            CollectionAssert.AreEqual(new[] { "1" }, this.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { "Add" }, this.ViewChanges.Select(x => x.Text));
        }

        [Test]
        public void UpdateWithTwo()
        {
            this.ItemsTextBox.Enter("1,2");
            this.UpdateButton.Click(false);
            CollectionAssert.AreEqual(new[] { "1", "2" }, this.ListBox.Rows.Select(x => x.Cells[0].AsLabel().Text));
            CollectionAssert.AreEqual(new[] { "1", "2" }, this.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { "Reset" }, this.ViewChanges.Select(x => x.Text));
        }

        [Test]
        public void UpdateWithOneThenClearSourceThenUpdateOne()
        {
            this.ItemsTextBox.Enter("1");
            this.UpdateButton.Click(false);
            CollectionAssert.AreEqual(new[] { "1" }, this.ListBox.Rows.Select(x => x.Cells[0].AsLabel().Text));
            CollectionAssert.AreEqual(new[] { "1" }, this.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { "Add" }, this.ViewChanges.Select(x => x.Text));

            this.ClearSourceButton.Click(false);
            CollectionAssert.IsEmpty(this.ListBox.RowValues());
            CollectionAssert.IsEmpty(this.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { "Add", "Remove" }, this.ViewChanges.Select(x => x.Text));

            this.ItemsTextBox.Enter("2");
            this.UpdateButton.Click(false);
            CollectionAssert.AreEqual(new[] { "2" }, this.ListBox.Rows.Select(x => x.Cells[0].AsLabel().Text));
            CollectionAssert.AreEqual(new[] { "2" }, this.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { "Add", "Remove", "Add" }, this.ViewChanges.Select(x => x.Text));
        }
    }
}