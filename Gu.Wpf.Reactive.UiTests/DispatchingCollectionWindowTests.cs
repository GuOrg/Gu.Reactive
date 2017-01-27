namespace Gu.Wpf.Reactive.UiTests
{
    using System.Collections.Generic;
    using System.Linq;

    using FlaUI.Core.AutomationElements;
    using FlaUI.Core.AutomationElements.Infrastructure;
    using FlaUI.Core.Definitions;

    using NUnit.Framework;

    public class DispatchingCollectionWindowTests : WindowTests
    {
        protected override string WindowName { get; } = "DispatchingCollectionWindow";

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

        private Button AddOneButton => this.Window
                                           .FindFirstDescendant(x => x.ByText("AddOne"))
                                           .AsButton();

        private Button AddFourButton => this.Window
                                           .FindFirstDescendant(x => x.ByText("AddFour"))
                                           .AsButton();

        private Button AddOneOnOtherThreadButton => this.Window
                                                        .FindFirstDescendant(x => x.ByText("AddOneOnOtherThread"))
                                                        .AsButton();

        private IEnumerable<Label> SourceChanges => this.Window
                                                        .FindFirstDescendant(x => x.ByText("SourceChanges"))
                                                        .FindAllChildren()
                                                        .Skip(1)
                                                        .Select(x => x.AsLabel());

        [SetUp]
        public void SetUp()
        {
            this.ClearButton.Click(false);
        }

        [Test]
        public void Initializes()
        {
            this.Restart();
            CollectionAssert.AreEqual(new[] { "1", "2", "3", string.Empty }, this.ListBox.Rows.Select(x => x.Cells[0].AsLabel().Text));
            CollectionAssert.AreEqual(new[] { "1", "2", "3", "Item: {NewItemPlaceholder}, Column Display Index: 0" }, this.DataGrid.Rows.Select(x => x.Cells[0].AsLabel().Text));

            CollectionAssert.IsEmpty(this.SourceChanges.Select(x => x.Text));
        }

        [Test]
        public void AddOne()
        {
            this.AddOneButton.Click(false);
            CollectionAssert.AreEqual(new[] { "1", string.Empty }, this.ListBox.Rows.Select(x => x.Cells[0].AsLabel().Text));
            CollectionAssert.AreEqual(new[] { "1", "Item: {NewItemPlaceholder}, Column Display Index: 0" }, this.DataGrid.Rows.Select(x => x.Cells[0].AsLabel().Text));

            CollectionAssert.AreEqual(new[] { "Reset", "Add" }, this.SourceChanges.Select(x => x.Text));
        }

        [Test]
        public void AddFour()
        {
            this.AddFourButton.Click(false);
            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4", string.Empty }, this.ListBox.Rows.Select(x => x.Cells[0].AsLabel().Text));
            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4", "Item: {NewItemPlaceholder}, Column Display Index: 0" }, this.DataGrid.Rows.Select(x => x.Cells[0].AsLabel().Text));
            CollectionAssert.AreEqual(new[] { "Reset" }.Concat(Enumerable.Repeat("Add", 4)), this.SourceChanges.Select(x => x.Text));
        }

        [Test]
        public void AddOneOnOtherThread()
        {
            this.AddOneOnOtherThreadButton.Click(false);
            CollectionAssert.AreEqual(new[] { "1", string.Empty }, this.ListBox.Rows.Select(x => x.Cells[0].AsLabel().Text));
            CollectionAssert.AreEqual(new[] { "1", "Item: {NewItemPlaceholder}, Column Display Index: 0" }, this.DataGrid.Rows.Select(x => x.Cells[0].AsLabel().Text));
        }

        [Test]
        public void EditDataGrid()
        {
            this.AddFourButton.Click(false);
            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4", string.Empty }, this.ListBox.Rows.Select(x => x.Cells[0].AsLabel().Text));
            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4", "Item: {NewItemPlaceholder}, Column Display Index: 0" }, this.DataGrid.Rows.Select(x => x.Cells[0].AsLabel().Text));

            var cell = this.DataGrid.Rows[0].Cells[0];
            cell.Click(false);
            cell.AsTextBox().Text = "5";
            this.ListBox.Focus();

            CollectionAssert.AreEqual(new[] { "5", "2", "3", "4", string.Empty }, this.ListBox.Rows.Select(x => x.Cells[0].AsLabel().Text));
            CollectionAssert.AreEqual(new[] { "5", "2", "3", "4", "Item: {NewItemPlaceholder}, Column Display Index: 0" }, this.DataGrid.Rows.Select(x => x.Cells[0].AsLabel().Text));
        }
    }
}