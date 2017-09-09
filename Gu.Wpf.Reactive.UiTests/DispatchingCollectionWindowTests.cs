namespace Gu.Wpf.Reactive.UiTests
{
    using System.Linq;

    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    public class DispatchingCollectionWindowTests
    {
        private static readonly string WindowName = "DispatchingCollectionWindow";

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
                var dataGrid = window.FindGroupBox("DataGrid").FindDataGrid();
                var listBox = window.FindGroupBox("ListBox").FindListBox();
                var sourceChanges = window.FindGroupBox("SourceChanges");
                CollectionAssert.AreEqual(new[] { "1", "2", "3", string.Empty }, listBox.Items.Select(x => x.FindTextBlock().Text));
                CollectionAssert.AreEqual(new[] { "1", "2", "3", string.Empty }, dataGrid.ColumnValues(0));
                CollectionAssert.IsEmpty(sourceChanges.ContentCollection);
            }
        }

        [Test]
        public void AddOne()
        {
            using (var app = Application.AttachOrLaunch(Info.ExeFileName, WindowName))
            {
                var window = app.MainWindow;
                window.FindButton("Reset").Invoke();
                var dataGrid = window.FindGroupBox("DataGrid").FindDataGrid();
                var listBox = window.FindGroupBox("ListBox").FindListBox();
                var sourceChanges = window.FindGroupBox("SourceChanges");

                window.FindButton("AddOne").Invoke();

                CollectionAssert.AreEqual(new[] { "1", string.Empty }, listBox.Items.Select(x => x.FindTextBlock().Text));
                CollectionAssert.AreEqual(new[] { "1", string.Empty }, dataGrid.ColumnValues(0));
                CollectionAssert.AreEqual(new[] { "Add" }, sourceChanges.ContentElements(x => new TextBlock(x)).Select(x => x.Text));
            }
        }

        [Test]
        public void AddFour()
        {
            using (var app = Application.AttachOrLaunch(Info.ExeFileName, WindowName))
            {
                var window = app.MainWindow;
                window.FindButton("Reset").Invoke();
                var dataGrid = window.FindGroupBox("DataGrid").FindDataGrid();
                var listBox = window.FindGroupBox("ListBox").FindListBox();
                var sourceChanges = window.FindGroupBox("SourceChanges");

                window.FindButton("AddFour").Invoke();

                CollectionAssert.AreEqual(new[] { "1", "2", "3", "4", string.Empty }, listBox.Items.Select(x => x.FindTextBlock().Text));
                CollectionAssert.AreEqual(new[] { "1", "2", "3", "4", string.Empty }, dataGrid.ColumnValues(0));
                CollectionAssert.AreEqual(new[] { "Add", "Add", "Add", "Add" }, sourceChanges.ContentElements(x => new TextBlock(x)).Select(x => x.Text));
            }
        }

        [Test]
        public void AddOneOnOtherThread()
        {
            using (var app = Application.AttachOrLaunch(Info.ExeFileName, WindowName))
            {
                var window = app.MainWindow;
                window.FindButton("Reset").Invoke();
                var dataGrid = window.FindGroupBox("DataGrid").FindDataGrid();
                var listBox = window.FindGroupBox("ListBox").FindListBox();
                var sourceChanges = window.FindGroupBox("SourceChanges");

                window.FindButton("AddOneOnOtherThread").Invoke();

                CollectionAssert.AreEqual(new[] { "1", string.Empty }, listBox.Items.Select(x => x.FindTextBlock().Text));
                CollectionAssert.AreEqual(new[] { "1", string.Empty }, dataGrid.ColumnValues(0));
                CollectionAssert.AreEqual(new[] { "Add" }, sourceChanges.ContentElements(x => new TextBlock(x)).Select(x => x.Text));
            }
        }

        [Test]
        public void EditDataGrid()
        {
            using (var app = Application.AttachOrLaunch(Info.ExeFileName, WindowName))
            {
                var window = app.MainWindow;
                window.FindButton("Reset").Invoke();
                var dataGrid = window.FindGroupBox("DataGrid").FindDataGrid();
                var listBox = window.FindGroupBox("ListBox").FindListBox();
                var sourceChanges = window.FindGroupBox("SourceChanges");

                window.FindButton("AddFour").Invoke();
                CollectionAssert.AreEqual(new[] { "1", "2", "3", "4", string.Empty }, listBox.Items.Select(x => x.FindTextBlock().Text));
                CollectionAssert.AreEqual(new[] { "1", "2", "3", "4", string.Empty }, dataGrid.ColumnValues(0));
                CollectionAssert.AreEqual(new[] { "Add", "Add", "Add", "Add" }, sourceChanges.ContentElements(x => new TextBlock(x)).Select(x => x.Text));

                dataGrid[0, 0].Value = "5";
                listBox.Focus();

                CollectionAssert.AreEqual(new[] { "5", "2", "3", "4", string.Empty }, listBox.Items.Select(x => x.FindTextBlock().Text));
                CollectionAssert.AreEqual(new[] { "5", "2", "3", "4", string.Empty }, dataGrid.ColumnValues(0));
                CollectionAssert.AreEqual(new[] { "Add", "Add", "Add", "Add" }, sourceChanges.ContentElements(x => new TextBlock(x)).Select(x => x.Text));
            }
        }
    }
}