namespace Gu.Wpf.Reactive.UiTests
{
    using FlaUI.Core.AutomationElements;
    using FlaUI.Core.AutomationElements.Infrastructure;
    using FlaUI.Core.Definitions;

    using NUnit.Framework;

    public class MappingViewWindowTests : WindowTests
    {
        protected override string WindowName { get; } = "MappingViewWindow";

        private Grid ListBox => this.Window
                                    .FindFirstDescendant(x => x.ByText("Source"))
                                    .FindFirstDescendant(x => x.ByControlType(ControlType.List))
                                    .AsGrid();

        private DataGridAndEvents MappedInts => new DataGridAndEvents(this.Window.FindFirstDescendant(x => x.ByText("MappedInts")));

        private DataGridAndEvents MappedIndexedInts => new DataGridAndEvents(this.Window.FindFirstDescendant(x => x.ByText("MappedIndexedInts")));

        private DataGridAndEvents MappedMapped => new DataGridAndEvents(this.Window.FindFirstDescendant(x => x.ByText("MappedMapped")));

        private DataGridAndEvents MappedMappedIndexed => new DataGridAndEvents(this.Window.FindFirstDescendant(x => x.ByText("MappedMappedIndexed")));

        private DataGridAndEvents MappedMappedUpdateIndexed => new DataGridAndEvents(this.Window.FindFirstDescendant(x => x.ByText("MappedMappedUpdateIndexed")));

        private DataGridAndEvents MappedMappedUpdateNewIndexed => new DataGridAndEvents(this.Window.FindFirstDescendant(x => x.ByText("MappedMappedUpdateNewIndexed")));

        private Button ClearButton => this.Window
                                          .FindFirstDescendant(x => x.ByText("Clear"))
                                          .AsButton();

        private Button AddOneButton => this.Window
                                           .FindFirstDescendant(x => x.ByText("Add to source"))
                                           .AsButton();

        private Button AddOneOnOtherThreadButton => this.Window
                                                        .FindFirstDescendant(x => x.ByText("Add to source on thread"))
                                                        .AsButton();

        [SetUp]
        public void SetUp()
        {
            this.AddOneButton.Click();
            this.AddOneButton.Click();
            this.ClearButton.Click();
        }

        [Test]
        public void Initializes()
        {
            this.Restart();
            CollectionAssert.AreEqual(new[] { "1", "2", "3" }, this.ListBox.RowValues());

            CollectionAssert.AreEqual(new[] { "1", "2", "3" }, this.MappedInts.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { string.Empty, string.Empty, string.Empty }, this.MappedInts.DataGrid.ColumnValues(1));
            CollectionAssert.IsEmpty(this.MappedInts.Events.RowValues());

            CollectionAssert.AreEqual(new[] { "1", "2", "3" }, this.MappedIndexedInts.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { "0", "1", "2" }, this.MappedIndexedInts.DataGrid.ColumnValues(1));
            CollectionAssert.IsEmpty(this.MappedIndexedInts.Events.RowValues());

            CollectionAssert.AreEqual(new[] { "2", "4", "6" }, this.MappedMapped.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { string.Empty, string.Empty, string.Empty }, this.MappedMapped.DataGrid.ColumnValues(1));
            CollectionAssert.IsEmpty(this.MappedMapped.Events.RowValues());

            CollectionAssert.AreEqual(new[] { "2", "4", "6" }, this.MappedMappedIndexed.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { "0", "1", "2" }, this.MappedMappedIndexed.DataGrid.ColumnValues(1));
            CollectionAssert.IsEmpty(this.MappedMappedIndexed.Events.RowValues());

            CollectionAssert.AreEqual(new[] { "2", "4", "6" }, this.MappedMappedUpdateIndexed.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { "0", "1", "2" }, this.MappedMappedUpdateIndexed.DataGrid.ColumnValues(1));
            CollectionAssert.IsEmpty(this.MappedMappedUpdateIndexed.Events.RowValues());

            CollectionAssert.AreEqual(new[] { "2", "4", "6" }, this.MappedMappedUpdateNewIndexed.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { "0", "1", "2" }, this.MappedMappedUpdateNewIndexed.DataGrid.ColumnValues(1));
            CollectionAssert.IsEmpty(this.MappedMappedUpdateNewIndexed.Events.RowValues());
        }

        [Test]
        public void AddOne()
        {
            this.AddOneButton.Click();
            CollectionAssert.AreEqual(new[] { "1" }, this.ListBox.RowValues());

            CollectionAssert.AreEqual(new[] { "1" }, this.MappedInts.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { string.Empty }, this.MappedInts.DataGrid.ColumnValues(1));
            CollectionAssert.AreEqual(new[] { "Reset", "Add" }, this.MappedInts.Events.RowValues());

            CollectionAssert.AreEqual(new[] { "1" }, this.MappedIndexedInts.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { "0" }, this.MappedIndexedInts.DataGrid.ColumnValues(1));
            CollectionAssert.AreEqual(new[] { "Reset", "Add" }, this.MappedIndexedInts.Events.RowValues());

            CollectionAssert.AreEqual(new[] { "2" }, this.MappedMapped.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { string.Empty }, this.MappedMapped.DataGrid.ColumnValues(1));
            CollectionAssert.AreEqual(new[] { "Reset", "Add" }, this.MappedMapped.Events.RowValues());

            CollectionAssert.AreEqual(new[] { "2" }, this.MappedMappedIndexed.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { "0" }, this.MappedMappedIndexed.DataGrid.ColumnValues(1));
            CollectionAssert.AreEqual(new[] { "Reset", "Add" }, this.MappedMappedIndexed.Events.RowValues());

            CollectionAssert.AreEqual(new[] { "2" }, this.MappedMappedUpdateIndexed.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { "0" }, this.MappedMappedUpdateIndexed.DataGrid.ColumnValues(1));
            CollectionAssert.AreEqual(new[] { "Reset", "Add" }, this.MappedMappedUpdateIndexed.Events.RowValues());

            CollectionAssert.AreEqual(new[] { "2" }, this.MappedMappedUpdateNewIndexed.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { "0" }, this.MappedMappedUpdateNewIndexed.DataGrid.ColumnValues(1));
            CollectionAssert.AreEqual(new[] { "Reset", "Add" }, this.MappedMappedUpdateNewIndexed.Events.RowValues());
        }

        [Test]
        public void AddOneOnOtherThread()
        {
            this.AddOneOnOtherThreadButton.Click();
            FlaUI.Core.Input.Helpers.WaitUntilInputIsProcessed();
            CollectionAssert.AreEqual(new[] { "1" }, this.ListBox.RowValues());

            CollectionAssert.AreEqual(new[] { "1" }, this.MappedInts.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { string.Empty }, this.MappedInts.DataGrid.ColumnValues(1));
            CollectionAssert.AreEqual(new[] { "Reset", "Add" }, this.MappedInts.Events.RowValues());

            CollectionAssert.AreEqual(new[] { "1" }, this.MappedIndexedInts.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { "0" }, this.MappedIndexedInts.DataGrid.ColumnValues(1));
            CollectionAssert.AreEqual(new[] { "Reset", "Add" }, this.MappedIndexedInts.Events.RowValues());

            CollectionAssert.AreEqual(new[] { "2" }, this.MappedMapped.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { string.Empty }, this.MappedMapped.DataGrid.ColumnValues(1));
            CollectionAssert.AreEqual(new[] { "Reset", "Add" }, this.MappedMapped.Events.RowValues());

            CollectionAssert.AreEqual(new[] { "2" }, this.MappedMappedIndexed.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { "0" }, this.MappedMappedIndexed.DataGrid.ColumnValues(1));
            CollectionAssert.AreEqual(new[] { "Reset", "Add" }, this.MappedMappedIndexed.Events.RowValues());

            CollectionAssert.AreEqual(new[] { "2" }, this.MappedMappedUpdateIndexed.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { "0" }, this.MappedMappedUpdateIndexed.DataGrid.ColumnValues(1));
            CollectionAssert.AreEqual(new[] { "Reset", "Add" }, this.MappedMappedUpdateIndexed.Events.RowValues());

            CollectionAssert.AreEqual(new[] { "2" }, this.MappedMappedUpdateNewIndexed.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { "0" }, this.MappedMappedUpdateNewIndexed.DataGrid.ColumnValues(1));
            CollectionAssert.AreEqual(new[] { "Reset", "Add" }, this.MappedMappedUpdateNewIndexed.Events.RowValues());
        }

        private class DataGridAndEvents
        {
            private readonly AutomationElement groupBox;

            public DataGridAndEvents(AutomationElement groupBox)
            {
                this.groupBox = groupBox;
            }

            public Grid DataGrid
            {
                get
                {
                    return this.groupBox
                               .FindFirstDescendant(x => x.ByControlType(ControlType.DataGrid))
                               .AsGrid();
                }
            }

            public Grid Events
            {
                get
                {
                    return this.groupBox
                               .FindFirstDescendant(x => x.ByAutomationId("ListBox"))
                               .AsGrid();
                }
            }
        }
    }
}