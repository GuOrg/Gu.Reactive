namespace Gu.Wpf.Reactive.UiTests
{
    using System.Linq;
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    public class ReadOnlyThrottledViewWindowTests
    {
        private const string WindowName = "ReadOnlyThrottledViewWindow";

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Application.KillLaunched(Info.ExeFileName);
        }

        [Test]
        public void Initializes()
        {
            using (var app = Application.Launch(Info.ExeFileName, WindowName))
            {
                var window = app.MainWindow;
                CollectionAssert.AreEqual(new[] { "1", "2", "3" }, window.FindGroupBox("ListBox").FindListBox().Items.Select(x => x.FindTextBlock().Text));
                CollectionAssert.AreEqual(new[] { "1", "2", "3" }, window.FindGroupBox("DataGrid").FindDataGrid().ColumnValues(0));
                CollectionAssert.AreEqual(new[] { "1", "2", "3" }, window.FindGroupBox("DataGridIList").FindDataGrid().ColumnValues(0));

                CollectionAssert.IsEmpty(window.FindChangesGroupBox("ViewChanges").Texts);
                CollectionAssert.IsEmpty(window.FindChangesGroupBox("SourceChanges").Texts);
            }
        }

        [Test]
        public void AddOne()
        {
            using (var app = Application.AttachOrLaunch(Info.ExeFileName, WindowName))
            {
                var window = app.MainWindow;
                window.FindButton("Reset").Invoke();
                window.FindButton("Reset").Invoke();
                window.FindButton("AddOne").Invoke();
                CollectionAssert.AreEqual(new[] { "1" }, window.FindGroupBox("ListBox").FindListBox().Items.Select(x => x.FindTextBlock().Text));
                CollectionAssert.AreEqual(new[] { "1" }, window.FindGroupBox("DataGrid").FindDataGrid().ColumnValues(0));
                CollectionAssert.AreEqual(new[] { "1" }, window.FindGroupBox("DataGridIList").FindDataGrid().ColumnValues(0));
                CollectionAssert.AreEqual(new[] { "Add" }, window.FindChangesGroupBox("ViewChanges").Texts);
                CollectionAssert.AreEqual(new[] { "Add" }, window.FindChangesGroupBox("SourceChanges").Texts);
            }
        }

        [Test]
        public void AddFour()
        {
            using (var app = Application.AttachOrLaunch(Info.ExeFileName, WindowName))
            {
                var window = app.MainWindow;
                window.FindButton("Reset").Invoke();
                window.FindButton("Reset").Invoke();
                window.FindButton("AddFour").Invoke();
                CollectionAssert.AreEqual(new[] { "1", "2", "3", "4" }, window.FindGroupBox("ListBox").FindListBox().Items.Select(x => x.FindTextBlock().Text));
                CollectionAssert.AreEqual(new[] { "1", "2", "3", "4" }, window.FindGroupBox("DataGrid").FindDataGrid().ColumnValues(0));
                CollectionAssert.AreEqual(new[] { "1", "2", "3", "4" }, window.FindGroupBox("DataGridIList").FindDataGrid().ColumnValues(0));
                CollectionAssert.AreEqual(new[] { "Reset" }, window.FindChangesGroupBox("ViewChanges").Texts);
                CollectionAssert.AreEqual(new[] { "Add", "Add", "Add", "Add" }, window.FindChangesGroupBox("SourceChanges").Texts);
            }
        }

        [Test]
        public void AddOneOnOtherThread()
        {
            using (var app = Application.AttachOrLaunch(Info.ExeFileName, WindowName))
            {
                var window = app.MainWindow;
                window.FindButton("Reset").Invoke();
                window.FindButton("Reset").Invoke();
                window.FindButton("AddOneOnOtherThread").Invoke();
                CollectionAssert.AreEqual(new[] { "1" }, window.FindGroupBox("ListBox").FindListBox().Items.Select(x => x.FindTextBlock().Text));
                CollectionAssert.AreEqual(new[] { "1" }, window.FindGroupBox("DataGrid").FindDataGrid().ColumnValues(0));
                CollectionAssert.AreEqual(new[] { "1" }, window.FindGroupBox("DataGridIList").FindDataGrid().ColumnValues(0));
                CollectionAssert.AreEqual(new[] { "Add" }, window.FindChangesGroupBox("ViewChanges").Texts);
                CollectionAssert.AreEqual(new[] { "Add" }, window.FindChangesGroupBox("SourceChanges").Texts);
            }
        }

        [Test]
        public void EditDataGrid()
        {
            using (var app = Application.AttachOrLaunch(Info.ExeFileName, WindowName))
            {
                var window = app.MainWindow;
                window.FindButton("Reset").Invoke();
                window.FindButton("Reset").Invoke();
                window.FindButton("AddFour").Invoke();
                var listBox = window.FindGroupBox("ListBox").FindListBox();
                var dataGrid = window.FindGroupBox("DataGrid").FindDataGrid();

                CollectionAssert.AreEqual(new[] { "1", "2", "3", "4" }, listBox.Items.Select(x => x.FindTextBlock().Text));
                CollectionAssert.AreEqual(new[] { "1", "2", "3", "4" }, dataGrid.ColumnValues(0));
                CollectionAssert.AreEqual(new[] { "1", "2", "3", "4" }, window.FindGroupBox("DataGridIList").FindDataGrid().ColumnValues(0));
                CollectionAssert.AreEqual(new[] { "Reset" }, window.FindChangesGroupBox("ViewChanges").Texts);
                CollectionAssert.AreEqual(new[] { "Add", "Add", "Add", "Add" }, window.FindChangesGroupBox("SourceChanges").Texts);

                dataGrid[0, 0].Value = "5";
                listBox.Focus();

                CollectionAssert.AreEqual(new[] { "5", "2", "3", "4" }, listBox.Items.Select(x => x.FindTextBlock().Text));
                CollectionAssert.AreEqual(new[] { "5", "2", "3", "4" }, dataGrid.ColumnValues(0));
                CollectionAssert.AreEqual(new[] { "5", "2", "3", "4" }, window.FindGroupBox("DataGridIList").FindDataGrid().ColumnValues(0));
                CollectionAssert.AreEqual(new[] { "Reset" }, window.FindChangesGroupBox("ViewChanges").Texts);
                CollectionAssert.AreEqual(new[] { "Add", "Add", "Add", "Add" }, window.FindChangesGroupBox("SourceChanges").Texts);
            }
        }
    }
}
