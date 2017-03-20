namespace Gu.Wpf.Reactive.UiTests
{
    using System.Collections.Generic;
    using System.Linq;

    using FlaUI.Core.AutomationElements;
    using FlaUI.Core.AutomationElements.Infrastructure;
    using FlaUI.Core.Definitions;

    using NUnit.Framework;

    public class FilteredDispatchingWindowTests : WindowTests
    {
        protected override string WindowName { get; } = "FilteredDispatchingWindow";

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

        private Button AddTenButton => this.Window
                                           .FindFirstDescendant(x => x.ByText("AddTen"))
                                           .AsButton();

        private Button TriggerButton => this.Window
                                            .FindFirstDescendant(x => x.ByText("Trigger"))
                                            .AsButton();

        private Button TriggerOnOtherThreadButton => this.Window
                                                         .FindFirstDescendant(x => x.ByText("TriggerOnOtherThread"))
                                                         .AsButton();

        private TextBox FilterTextBox => this.Window
                                             .FindFirstDescendant(x => x.ByAutomationId("FilterText"))
                                             .AsTextBox();

        private IEnumerable<Label> ViewChanges => this.Window
                                                      .FindFirstDescendant(x => x.ByText("ViewChanges"))
                                                      .FindAllChildren()
                                                      .Skip(1)
                                                      .Select(x => x.AsLabel());

        private IEnumerable<Label> SourceChanges => this.Window
                                                        .FindFirstDescendant(x => x.ByText("SourceChanges"))
                                                        .FindAllChildren()
                                                        .Skip(1)
                                                        .Select(x => x.AsLabel());

        [SetUp]
        public void SetUp()
        {
            this.FilterTextBox.Text = "5";
            this.AddTenButton.Click(false);
            this.ClearButton.Click(false);
        }

        [Test]
        public void Initializes()
        {
            this.Restart();
            CollectionAssert.AreEqual(new[] { "1", "2", "3", string.Empty }, this.ListBox.Rows.Select(x => x.Cells[0].AsLabel().Text));
            CollectionAssert.AreEqual(new[] { "1", "2", "3", "{NewItemPlaceholder}" }, this.DataGrid.ColumnValues(0));

            CollectionAssert.AreEqual(new[] { "Reset" }, this.ViewChanges.Select(x => x.Text));
            CollectionAssert.AreEqual(new[] { "Reset" }, this.SourceChanges.Select(x => x.Text));
        }

        [Test]
        public void AddOne()
        {
            this.AddOneButton.Click(false);
            CollectionAssert.AreEqual(new[] { "1", string.Empty }, this.ListBox.Rows.Select(x => x.Cells[0].AsLabel().Text));
            CollectionAssert.AreEqual(new[] { "1", "{NewItemPlaceholder}" }, this.DataGrid.ColumnValues(0));

            CollectionAssert.AreEqual(new[] { "Reset", "Add" }, this.ViewChanges.Select(x => x.Text));
            CollectionAssert.AreEqual(new[] { "Reset", "Add" }, this.SourceChanges.Select(x => x.Text));
        }

        [Test]
        public void AddTen()
        {
            this.AddTenButton.Click(false);
            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4", string.Empty }, this.ListBox.Rows.Select(x => x.Cells[0].AsLabel().Text));
            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4", "{NewItemPlaceholder}" }, this.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { "Reset" }.Concat(Enumerable.Repeat("Reset", 1)), this.ViewChanges.Select(x => x.Text));
            CollectionAssert.AreEqual(new[] { "Reset" }.Concat(Enumerable.Repeat("Add", 10)), this.SourceChanges.Select(x => x.Text));
        }

        [Test]
        public void FilterThenTrigger()
        {
            this.AddTenButton.Click(false);
            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4", string.Empty }, this.ListBox.Rows.Select(x => x.Cells[0].AsLabel().Text));
            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4", "{NewItemPlaceholder}" }, this.DataGrid.ColumnValues(0));

            this.FilterTextBox.Text = "2";
            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4", string.Empty }, this.ListBox.Rows.Select(x => x.Cells[0].AsLabel().Text));
            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4", "{NewItemPlaceholder}" }, this.DataGrid.ColumnValues(0));

            this.TriggerButton.Click(false);
            CollectionAssert.AreEqual(new[] { "1", string.Empty }, this.ListBox.Rows.Select(x => x.Cells[0].AsLabel().Text));
            CollectionAssert.AreEqual(new[] { "1", "{NewItemPlaceholder}" }, this.DataGrid.ColumnValues(0));
        }

        [Test]
        public void FilterThenTriggerOnOtherThread()
        {
            this.AddTenButton.Click(false);
            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4", string.Empty }, this.ListBox.Rows.Select(x => x.Cells[0].AsLabel().Text));
            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4", "{NewItemPlaceholder}" }, this.DataGrid.ColumnValues(0));

            this.FilterTextBox.Text = "2";
            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4", string.Empty }, this.ListBox.Rows.Select(x => x.Cells[0].AsLabel().Text));
            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4", "{NewItemPlaceholder}" }, this.DataGrid.ColumnValues(0));

            this.TriggerOnOtherThreadButton.Click(false);
            CollectionAssert.AreEqual(new[] { "1", string.Empty }, this.ListBox.Rows.Select(x => x.Cells[0].AsLabel().Text));
            CollectionAssert.AreEqual(new[] { "1", "{NewItemPlaceholder}" }, this.DataGrid.ColumnValues(0));
        }

        [Test]
        public void EditDataGrid()
        {
            this.AddTenButton.Click(false);
            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4", string.Empty }, this.ListBox.Rows.Select(x => x.Cells[0].AsLabel().Text));
            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4", "{NewItemPlaceholder}" }, this.DataGrid.ColumnValues(0));

            var cell = this.DataGrid.Rows[0].Cells[0];
            cell.Click(false);
            cell.AsTextBox().Text = "5";
            this.ListBox.Focus();

            CollectionAssert.AreEqual(new[] { "5", "2", "3", "4", string.Empty }, this.ListBox.Rows.Select(x => x.Cells[0].AsLabel().Text));
            CollectionAssert.AreEqual(new[] { "5", "2", "3", "4", "{NewItemPlaceholder}" }, this.DataGrid.ColumnValues(0));
        }
    }
}