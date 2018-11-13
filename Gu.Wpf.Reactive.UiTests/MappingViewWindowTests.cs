namespace Gu.Wpf.Reactive.UiTests
{
    using System.Linq;
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    public class MappingViewWindowTests
    {
        private const string WindowName = "MappingViewWindow";

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Application.KillLaunched(Info.ExeFileName);
        }

        [TestCase("MappedInts", new[] { "1", "2", "3" }, new[] { "", "", "" })]
        [TestCase("MappedIndexedInts", new[] { "1", "2", "3" }, new[] { "0", "1", "2" })]
        [TestCase("MappedMapped", new[] { "2", "4", "6" }, new[] { "", "", "" })]
        [TestCase("MappedMappedIndexed", new[] { "2", "4", "6" }, new[] { "0", "1", "2" })]
        [TestCase("MappedMappedUpdateIndexed", new[] { "2", "4", "6" }, new[] { "0", "1", "2" })]
        [TestCase("MappedMappedUpdateNewIndexed", new[] { "2", "4", "6" }, new[] { "0", "1", "2" })]
        public void Initializes(string header, string[] col1, string[] col2)
        {
            using (var app = Application.Launch(Info.ExeFileName, WindowName))
            {
                var window = app.MainWindow;
                CollectionAssert.AreEqual(new[] { "1", "2", "3" }, window.FindGroupBox("Source").FindListBox().Items.Select(x => x.FindTextBlock().Text));

                var dataGridAndEVents = new DataGridAndEvents(window.FindGroupBox(header));
                CollectionAssert.AreEqual(col1, dataGridAndEVents.DataGrid.ColumnValues(0));
                CollectionAssert.AreEqual(col2, dataGridAndEVents.DataGrid.ColumnValues(1));
                CollectionAssert.IsEmpty(dataGridAndEVents.Events.Items);
            }
        }

        [TestCase("MappedInts", "1", "")]
        [TestCase("MappedIndexedInts", "1", "0")]
        [TestCase("MappedMapped", "2", "")]
        [TestCase("MappedMappedIndexed", "2", "0")]
        [TestCase("MappedMappedUpdateIndexed", "2", "0")]
        [TestCase("MappedMappedUpdateNewIndexed", "2", "0")]
        public void AddOne(string header, string col1, string col2)
        {
            using (var app = Application.Launch(Info.ExeFileName, WindowName))
            {
                var window = app.MainWindow;
                window.FindButton("Clear").Invoke();
                window.FindButton("Add to source").Invoke();
                CollectionAssert.AreEqual(new[] { "1" }, window.FindGroupBox("Source").FindListBox().Items.Select(x => x.FindTextBlock().Text));

                var dataGridAndEVents = new DataGridAndEvents(window.FindGroupBox(header));
                CollectionAssert.AreEqual(new[] { col1 }, dataGridAndEVents.DataGrid.ColumnValues(0));
                CollectionAssert.AreEqual(new[] { col2 }, dataGridAndEVents.DataGrid.ColumnValues(1));
                CollectionAssert.AreEqual(new[] { "Reset", "Add" }, dataGridAndEVents.Events.Items.Select(x => x.FindTextBlock().Text));
            }
        }

        [TestCase("MappedInts", "1", "")]
        [TestCase("MappedIndexedInts", "1", "0")]
        [TestCase("MappedMapped", "2", "")]
        [TestCase("MappedMappedIndexed", "2", "0")]
        [TestCase("MappedMappedUpdateIndexed", "2", "0")]
        [TestCase("MappedMappedUpdateNewIndexed", "2", "0")]
        public void AddOneOnOtherThread(string header, string col1, string col2)
        {
            using (var app = Application.Launch(Info.ExeFileName, WindowName))
            {
                var window = app.MainWindow;
                window.FindButton("Clear").Invoke();
                window.FindButton("Add to source on thread").Invoke();
                CollectionAssert.AreEqual(new[] { "1" }, window.FindGroupBox("Source").FindListBox().Items.Select(x => x.FindTextBlock().Text));

                var dataGridAndEVents = new DataGridAndEvents(window.FindGroupBox(header));
                CollectionAssert.AreEqual(new[] { col1 }, dataGridAndEVents.DataGrid.ColumnValues(0));
                CollectionAssert.AreEqual(new[] { col2 }, dataGridAndEVents.DataGrid.ColumnValues(1));
                CollectionAssert.AreEqual(new[] { "Reset", "Add" }, dataGridAndEVents.Events.Items.Select(x => x.FindTextBlock().Text));
            }
        }

        private class DataGridAndEvents
        {
            private readonly GroupBox groupBox;

            public DataGridAndEvents(GroupBox groupBox)
            {
                this.groupBox = groupBox;
            }

            public DataGrid DataGrid => this.groupBox.FindDataGrid();

            public ListBox Events => this.groupBox.FindListBox();
        }
    }
}
