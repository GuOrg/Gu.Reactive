namespace Gu.Wpf.Reactive.UiTests
{
    using System.Linq;
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    public class MappingViewWindowTests : WindowTests
    {
        protected override string WindowName { get; } = "MappingViewWindow";

        private ListBox ListBox => this.Window
                                    .FindFirstDescendant(x => x.ByText("Source"))
                                    .FindListBox();

        private DataGridAndEvents MappedInts => new DataGridAndEvents(this.Window.FindFirstDescendant(x => x.ByText("MappedInts")));

        private DataGridAndEvents MappedIndexedInts => new DataGridAndEvents(this.Window.FindFirstDescendant(x => x.ByText("MappedIndexedInts")));

        private DataGridAndEvents MappedMapped => new DataGridAndEvents(this.Window.FindFirstDescendant(x => x.ByText("MappedMapped")));

        private DataGridAndEvents MappedMappedIndexed => new DataGridAndEvents(this.Window.FindFirstDescendant(x => x.ByText("MappedMappedIndexed")));

        private DataGridAndEvents MappedMappedUpdateIndexed => new DataGridAndEvents(this.Window.FindFirstDescendant(x => x.ByText("MappedMappedUpdateIndexed")));

        private DataGridAndEvents MappedMappedUpdateNewIndexed => new DataGridAndEvents(this.Window.FindFirstDescendant(x => x.ByText("MappedMappedUpdateNewIndexed")));

        private Button ClearButton => this.Window.FindButton("Clear");

        private Button AddOneButton => this.Window.FindButton("Add to source");

        private Button AddOneOnOtherThreadButton => this.Window.FindButton("Add to source on thread");

        [SetUp]
        public void SetUp()
        {
            this.AddOneButton.Click();
            this.AddOneButton.Click();
            this.ClearButton.Click();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Application.KillLaunched(Info.ExeFileName);
        }

        [Test]
        public void Initializes()
        {
            this.Restart();
            CollectionAssert.AreEqual(new[] { "1", "2", "3" }, this.ListBox.Items.Select(x => x.FindTextBlock().Text));

            CollectionAssert.AreEqual(new[] { "1", "2", "3" }, this.MappedInts.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { string.Empty, string.Empty, string.Empty }, this.MappedInts.DataGrid.ColumnValues(1));
            CollectionAssert.IsEmpty(this.MappedInts.Events.Items.Select(x => x.FindTextBlock().Text));

            CollectionAssert.AreEqual(new[] { "1", "2", "3" }, this.MappedIndexedInts.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { "0", "1", "2" }, this.MappedIndexedInts.DataGrid.ColumnValues(1));
            CollectionAssert.IsEmpty(this.MappedIndexedInts.Events.Items.Select(x => x.FindTextBlock().Text));

            CollectionAssert.AreEqual(new[] { "2", "4", "6" }, this.MappedMapped.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { string.Empty, string.Empty, string.Empty }, this.MappedMapped.DataGrid.ColumnValues(1));
            CollectionAssert.IsEmpty(this.MappedMapped.Events.Items.Select(x => x.FindTextBlock().Text));

            CollectionAssert.AreEqual(new[] { "2", "4", "6" }, this.MappedMappedIndexed.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { "0", "1", "2" }, this.MappedMappedIndexed.DataGrid.ColumnValues(1));
            CollectionAssert.IsEmpty(this.MappedMappedIndexed.Events.Items.Select(x => x.FindTextBlock().Text));

            CollectionAssert.AreEqual(new[] { "2", "4", "6" }, this.MappedMappedUpdateIndexed.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { "0", "1", "2" }, this.MappedMappedUpdateIndexed.DataGrid.ColumnValues(1));
            CollectionAssert.IsEmpty(this.MappedMappedUpdateIndexed.Events.Items.Select(x => x.FindTextBlock().Text));

            CollectionAssert.AreEqual(new[] { "2", "4", "6" }, this.MappedMappedUpdateNewIndexed.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { "0", "1", "2" }, this.MappedMappedUpdateNewIndexed.DataGrid.ColumnValues(1));
            CollectionAssert.IsEmpty(this.MappedMappedUpdateNewIndexed.Events.Items.Select(x => x.FindTextBlock().Text));
        }

        [Test]
        public void AddOne()
        {
            this.AddOneButton.Click();
            CollectionAssert.AreEqual(new[] { "1" }, this.ListBox.Items.Select(x => x.FindTextBlock().Text));

            CollectionAssert.AreEqual(new[] { "1" }, this.MappedInts.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { string.Empty }, this.MappedInts.DataGrid.ColumnValues(1));
            CollectionAssert.AreEqual(new[] { "Reset", "Add" }, this.MappedInts.Events.Items.Select(x => x.FindTextBlock().Text));

            CollectionAssert.AreEqual(new[] { "1" }, this.MappedIndexedInts.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { "0" }, this.MappedIndexedInts.DataGrid.ColumnValues(1));
            CollectionAssert.AreEqual(new[] { "Reset", "Add" }, this.MappedIndexedInts.Events.Items.Select(x => x.FindTextBlock().Text));

            CollectionAssert.AreEqual(new[] { "2" }, this.MappedMapped.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { string.Empty }, this.MappedMapped.DataGrid.ColumnValues(1));
            CollectionAssert.AreEqual(new[] { "Reset", "Add" }, this.MappedMapped.Events.Items.Select(x => x.FindTextBlock().Text));

            CollectionAssert.AreEqual(new[] { "2" }, this.MappedMappedIndexed.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { "0" }, this.MappedMappedIndexed.DataGrid.ColumnValues(1));
            CollectionAssert.AreEqual(new[] { "Reset", "Add" }, this.MappedMappedIndexed.Events.Items.Select(x => x.FindTextBlock().Text));

            CollectionAssert.AreEqual(new[] { "2" }, this.MappedMappedUpdateIndexed.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { "0" }, this.MappedMappedUpdateIndexed.DataGrid.ColumnValues(1));
            CollectionAssert.AreEqual(new[] { "Reset", "Add" }, this.MappedMappedUpdateIndexed.Events.Items.Select(x => x.FindTextBlock().Text));

            CollectionAssert.AreEqual(new[] { "2" }, this.MappedMappedUpdateNewIndexed.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { "0" }, this.MappedMappedUpdateNewIndexed.DataGrid.ColumnValues(1));
            CollectionAssert.AreEqual(new[] { "Reset", "Add" }, this.MappedMappedUpdateNewIndexed.Events.Items.Select(x => x.FindTextBlock().Text));
        }

        [Test]
        public void AddOneOnOtherThread()
        {
            this.AddOneOnOtherThreadButton.Click();
            Wait.UntilInputIsProcessed();
            CollectionAssert.AreEqual(new[] { "1" }, this.ListBox.Items.Select(x => x.FindTextBlock().Text));

            CollectionAssert.AreEqual(new[] { "1" }, this.MappedInts.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { string.Empty }, this.MappedInts.DataGrid.ColumnValues(1));
            CollectionAssert.AreEqual(new[] { "Reset", "Add" }, this.MappedInts.Events.Items.Select(x => x.FindTextBlock().Text));

            CollectionAssert.AreEqual(new[] { "1" }, this.MappedIndexedInts.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { "0" }, this.MappedIndexedInts.DataGrid.ColumnValues(1));
            CollectionAssert.AreEqual(new[] { "Reset", "Add" }, this.MappedIndexedInts.Events.Items.Select(x => x.FindTextBlock().Text));

            CollectionAssert.AreEqual(new[] { "2" }, this.MappedMapped.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { string.Empty }, this.MappedMapped.DataGrid.ColumnValues(1));
            CollectionAssert.AreEqual(new[] { "Reset", "Add" }, this.MappedMapped.Events.Items.Select(x => x.FindTextBlock().Text));

            CollectionAssert.AreEqual(new[] { "2" }, this.MappedMappedIndexed.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { "0" }, this.MappedMappedIndexed.DataGrid.ColumnValues(1));
            CollectionAssert.AreEqual(new[] { "Reset", "Add" }, this.MappedMappedIndexed.Events.Items.Select(x => x.FindTextBlock().Text));

            CollectionAssert.AreEqual(new[] { "2" }, this.MappedMappedUpdateIndexed.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { "0" }, this.MappedMappedUpdateIndexed.DataGrid.ColumnValues(1));
            CollectionAssert.AreEqual(new[] { "Reset", "Add" }, this.MappedMappedUpdateIndexed.Events.Items.Select(x => x.FindTextBlock().Text));

            CollectionAssert.AreEqual(new[] { "2" }, this.MappedMappedUpdateNewIndexed.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { "0" }, this.MappedMappedUpdateNewIndexed.DataGrid.ColumnValues(1));
            CollectionAssert.AreEqual(new[] { "Reset", "Add" }, this.MappedMappedUpdateNewIndexed.Events.Items.Select(x => x.FindTextBlock().Text));
        }

        private class DataGridAndEvents
        {
            private readonly AutomationElement groupBox;

            public DataGridAndEvents(AutomationElement groupBox)
            {
                this.groupBox = groupBox;
            }

            public DataGrid DataGrid => this.groupBox
                                            .FindDataGrid();

            public ListBox Events => this.groupBox
                                         .FindListBox();
        }
    }
}