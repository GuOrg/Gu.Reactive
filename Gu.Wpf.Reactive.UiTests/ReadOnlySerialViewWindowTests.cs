namespace Gu.Wpf.Reactive.UiTests
{
    using System.Linq;
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    public class ReadOnlySerialViewWindowTests
    {
        private const string WindowName = "ReadOnlySerialViewWindow";

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
                CollectionAssert.IsEmpty(window.FindGroupBox("ListBox").FindListBox().Items);
                CollectionAssert.IsEmpty(window.FindGroupBox("DataGrid").FindDataGrid().ColumnValues(0));
                CollectionAssert.IsEmpty(window.FindChangesGroupBox("ViewChanges").Texts);
            }
        }

        [Test]
        public void UpdateWithOne()
        {
            using (var app = Application.AttachOrLaunch(Info.ExeFileName, WindowName))
            {
                var window = app.MainWindow;
                window.FindButton("Reset").Invoke();
                window.FindTextBox("Items").Text = "1";
                window.FindButton("Update").Invoke();
                CollectionAssert.AreEqual(new[] { "1" }, window.FindGroupBox("ListBox").FindListBox().Items.Select(x => x.FindTextBlock().Text));
                CollectionAssert.AreEqual(new[] { "1" }, window.FindGroupBox("DataGrid").FindDataGrid().ColumnValues(0));
                CollectionAssert.AreEqual(new[] { "Add" }, window.FindChangesGroupBox("ViewChanges").Texts.ToArray());
            }
        }

        [Test]
        public void UpdateWithTwo()
        {
            using (var app = Application.AttachOrLaunch(Info.ExeFileName, WindowName))
            {
                var window = app.MainWindow;
                window.FindButton("Reset").Invoke();
                window.FindTextBox("Items").Text = "1, 2";
                window.FindButton("Update").Invoke();
                CollectionAssert.AreEqual(new[] { "1", "2" }, window.FindGroupBox("ListBox").FindListBox().Items.Select(x => x.FindTextBlock().Text));
                CollectionAssert.AreEqual(new[] { "1", "2" }, window.FindGroupBox("DataGrid").FindDataGrid().ColumnValues(0));
                CollectionAssert.AreEqual(new[] { "Reset" }, window.FindChangesGroupBox("ViewChanges").Texts);
            }
        }

        [Test]
        public void UpdateWithOneThenClearSourceThenUpdateOne()
        {
            using (var app = Application.AttachOrLaunch(Info.ExeFileName, WindowName))
            {
                var window = app.MainWindow;
                window.FindButton("Reset").Invoke();
                window.FindTextBox("Items").Text = "1";
                window.FindButton("Update").Invoke();
                CollectionAssert.AreEqual(new[] { "1" }, window.FindGroupBox("ListBox").FindListBox().Items.Select(x => x.FindTextBlock().Text));
                CollectionAssert.AreEqual(new[] { "1" }, window.FindGroupBox("DataGrid").FindDataGrid().ColumnValues(0));
                CollectionAssert.AreEqual(new[] { "Add" }, window.FindChangesGroupBox("ViewChanges").Texts);

                window.FindButton("ClearSource").Invoke();
                CollectionAssert.IsEmpty(window.FindGroupBox("ListBox").FindListBox().Items.Select(x => x.FindTextBlock().Text));
                CollectionAssert.IsEmpty(window.FindGroupBox("DataGrid").FindDataGrid().ColumnValues(0));
                CollectionAssert.AreEqual(new[] { "Add", "Remove" }, window.FindChangesGroupBox("ViewChanges").Texts);

                window.FindTextBox("Items").Text = "2";
                window.FindButton("Update").Invoke();
                CollectionAssert.AreEqual(new[] { "2" }, window.FindGroupBox("ListBox").FindListBox().Items.Select(x => x.FindTextBlock().Text));
                CollectionAssert.AreEqual(new[] { "2" }, window.FindGroupBox("DataGrid").FindDataGrid().ColumnValues(0));
                CollectionAssert.AreEqual(new[] { "Add", "Remove", "Add" }, window.FindChangesGroupBox("ViewChanges").Texts);
            }
        }
    }
}
