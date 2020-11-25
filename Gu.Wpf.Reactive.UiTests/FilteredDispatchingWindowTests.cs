namespace Gu.Wpf.Reactive.UiTests
{
    using System.Linq;
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    public class FilteredDispatchingWindowTests
    {
        private const string WindowName = "FilteredDispatchingWindow";

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Application.KillLaunched(Info.ExeFileName);
        }

        [Test]
        public void Initializes()
        {
            using var app = Application.Launch(Info.ExeFileName, WindowName);
            var window = app.MainWindow;
            CollectionAssert.AreEqual(new[] { "1", "2", "3", string.Empty }, window.FindGroupBox("ListBox").FindListBox().Items.Select(x => x.FindTextBlock().Text));
            CollectionAssert.AreEqual(new[] { "1", "2", "3", string.Empty }, window.FindGroupBox("DataGrid").FindDataGrid().ColumnValues(0));

            CollectionAssert.AreEqual(new[] { "Reset" }, window.FindChangesGroupBox("ViewChanges").Texts);
            CollectionAssert.AreEqual(new[] { "Reset" }, window.FindChangesGroupBox("SourceChanges").Texts);
        }

        [Test]
        public void AddOne()
        {
            using var app = Application.AttachOrLaunch(Info.ExeFileName, WindowName);
            var window = app.MainWindow;
            window.FindButton("Reset").Invoke();
            window.FindButton("AddOne").Invoke();
            CollectionAssert.AreEqual(new[] { "1", string.Empty }, window.FindGroupBox("ListBox").FindListBox().Items.Select(x => x.FindTextBlock().Text));
            CollectionAssert.AreEqual(new[] { "1", string.Empty }, window.FindGroupBox("DataGrid").FindDataGrid().ColumnValues(0));
            CollectionAssert.AreEqual(new[] { "Add" }, window.FindChangesGroupBox("ViewChanges").Texts);
            CollectionAssert.AreEqual(new[] { "Add" }, window.FindChangesGroupBox("SourceChanges").Texts);
        }

        [Test]
        public void AddTen()
        {
            using var app = Application.AttachOrLaunch(Info.ExeFileName, WindowName);
            var window = app.MainWindow;
            window.FindButton("Reset").Invoke();
            window.FindButton("AddTen").Invoke();
            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4", string.Empty }, window.FindGroupBox("ListBox").FindListBox().Items.Select(x => x.FindTextBlock().Text));
            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4", string.Empty }, window.FindGroupBox("DataGrid").FindDataGrid().ColumnValues(0));
            CollectionAssert.AreEqual(new[] { "Reset" }, window.FindChangesGroupBox("ViewChanges").Texts);
            CollectionAssert.AreEqual(Enumerable.Repeat("Add", 10), window.FindChangesGroupBox("SourceChanges").Texts);
        }

        [Test]
        public void FilterThenTrigger()
        {
            using var app = Application.AttachOrLaunch(Info.ExeFileName, WindowName);
            var window = app.MainWindow;
            window.FindButton("Reset").Invoke();
            window.FindButton("AddTen").Invoke();
            var listBox = window.FindGroupBox("ListBox").FindListBox();
            var dataGrid = window.FindGroupBox("DataGrid").FindDataGrid();

            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4", string.Empty }, listBox.Items.Select(x => x.FindTextBlock().Text));
            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4", string.Empty }, dataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { "Reset" }, window.FindChangesGroupBox("ViewChanges").Texts);
            CollectionAssert.AreEqual(Enumerable.Repeat("Add", 10), window.FindChangesGroupBox("SourceChanges").Texts);

            window.FindTextBox("FilterText").Text = "2";
            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4", string.Empty }, listBox.Items.Select(x => x.FindTextBlock().Text));
            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4", string.Empty }, dataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { "Reset" }, window.FindChangesGroupBox("ViewChanges").Texts);
            CollectionAssert.AreEqual(Enumerable.Repeat("Add", 10), window.FindChangesGroupBox("SourceChanges").Texts);

            window.FindButton("Trigger").Invoke();
            CollectionAssert.AreEqual(new[] { "1", string.Empty }, listBox.Items.Select(x => x.FindTextBlock().Text));
            CollectionAssert.AreEqual(new[] { "1", string.Empty }, dataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { "Reset", "Reset" }, window.FindChangesGroupBox("ViewChanges").Texts);
            CollectionAssert.AreEqual(Enumerable.Repeat("Add", 10), window.FindChangesGroupBox("SourceChanges").Texts);
        }

        [Test]
        public void FilterThenTriggerOnOtherThread()
        {
            using var app = Application.AttachOrLaunch(Info.ExeFileName, WindowName);
            var window = app.MainWindow;
            window.FindButton("Reset").Invoke();
            window.FindButton("AddTen").Invoke();
            var listBox = window.FindGroupBox("ListBox").FindListBox();
            var dataGrid = window.FindGroupBox("DataGrid").FindDataGrid();

            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4", string.Empty }, listBox.Items.Select(x => x.FindTextBlock().Text));
            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4", string.Empty }, dataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { "Reset" }, window.FindChangesGroupBox("ViewChanges").Texts);
            CollectionAssert.AreEqual(Enumerable.Repeat("Add", 10), window.FindChangesGroupBox("SourceChanges").Texts);

            window.FindTextBox("FilterText").Text = "2";
            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4", string.Empty }, listBox.Items.Select(x => x.FindTextBlock().Text));
            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4", string.Empty }, dataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { "Reset" }, window.FindChangesGroupBox("ViewChanges").Texts);
            CollectionAssert.AreEqual(Enumerable.Repeat("Add", 10), window.FindChangesGroupBox("SourceChanges").Texts);

            window.FindButton("TriggerOnOtherThread").Invoke();
            CollectionAssert.AreEqual(new[] { "1", string.Empty }, listBox.Items.Select(x => x.FindTextBlock().Text));
            CollectionAssert.AreEqual(new[] { "1", string.Empty }, dataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { "Reset", "Reset" }, window.FindChangesGroupBox("ViewChanges").Texts);
            CollectionAssert.AreEqual(Enumerable.Repeat("Add", 10), window.FindChangesGroupBox("SourceChanges").Texts);
        }

        [Test]
        public void EditDataGrid()
        {
            using var app = Application.AttachOrLaunch(Info.ExeFileName, WindowName);
            var window = app.MainWindow;
            window.FindButton("Reset").Invoke();
            window.FindButton("AddTen").Invoke();
            var listBox = window.FindGroupBox("ListBox").FindListBox();
            var dataGrid = window.FindGroupBox("DataGrid").FindDataGrid();

            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4", string.Empty }, listBox.Items.Select(x => x.FindTextBlock().Text));
            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4", string.Empty }, dataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { "Reset" }, window.FindChangesGroupBox("ViewChanges").Texts);
            CollectionAssert.AreEqual(Enumerable.Repeat("Add", 10), window.FindChangesGroupBox("SourceChanges").Texts);

            dataGrid[0, 0].Value = "5";
            listBox.Focus();

            CollectionAssert.AreEqual(new[] { "5", "2", "3", "4", string.Empty }, listBox.Items.Select(x => x.FindTextBlock().Text));
            CollectionAssert.AreEqual(new[] { "5", "2", "3", "4", string.Empty }, dataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { "Reset" }, window.FindChangesGroupBox("ViewChanges").Texts);
            CollectionAssert.AreEqual(Enumerable.Repeat("Add", 10), window.FindChangesGroupBox("SourceChanges").Texts);
        }
    }
}
