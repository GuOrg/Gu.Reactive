namespace Gu.Wpf.Reactive.UiTests
{
    using System.Collections.Generic;
    using System.Linq;

    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    public class ReadonlyFilteredDispatchingWindowTests : WindowTests
    {
        protected override string WindowName { get; } = "ReadonlyFilteredDispatchingWindow";

        private ListBox ListBox => this.Window
                                    .FindFirstDescendant(x => x.ByText("ListBox"))
                                    .FindListBox();

        private DataGrid DataGrid => this.Window
                                      .FindGroupBox("DataGrid")
                                      .FindDataGrid();

        private Button ClearButton => this.Window.FindButton("Clear");

        private Button AddOneButton => this.Window.FindButton("AddOne");

        private Button AddTenButton => this.Window.FindButton("AddTen");

        private Button AddOneOnOtherThreadButton => this.Window.FindButton("AddOneOnOtherThread");

        private Button TriggerButton => this.Window.FindButton("Trigger");

        private Button TriggerOnOtherThreadButton => this.Window.FindButton("TriggerOnOtherThread");

        private TextBox FilterTextBox => this.Window.FindTextBox("FilterText");

        [SetUp]
        public void SetUp()
        {
            this.FilterTextBox.Text = "5";
            this.AddTenButton.Click();
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
            CollectionAssert.AreEqual(new[] { "1", "2", "3" }, this.DataGrid.ColumnValues(0));

            CollectionAssert.AreEqual(new[] { "Reset" }, this.Window.FindChangesGroupBox("ViewChanges").Texts);
            CollectionAssert.AreEqual(new[] { "Reset" }, this.Window.FindChangesGroupBox("SourceChanges").Texts);
        }

        [Test]
        public void AddOne()
        {
            this.AddOneButton.Click();
            CollectionAssert.AreEqual(new[] { "1" }, this.ListBox.Items.Select(x => x.FindTextBlock().Text));
            CollectionAssert.AreEqual(new[] { "1" }, this.DataGrid.ColumnValues(0));

            CollectionAssert.AreEqual(new[] { "Reset", "Add" }, this.Window.FindChangesGroupBox("ViewChanges").Texts);
            CollectionAssert.AreEqual(new[] { "Reset", "Add" }, this.Window.FindChangesGroupBox("SourceChanges").Texts);
        }

        [Test]
        public void AddTen()
        {
            this.AddTenButton.Click();
            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4" }, this.ListBox.Items.Select(x => x.FindTextBlock().Text));
            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4" }, this.DataGrid.ColumnValues(0));
            CollectionAssert.AreEqual(new[] { "Reset" }.Concat(Enumerable.Repeat("Reset", 1)), this.Window.FindChangesGroupBox("ViewChanges").Texts);
            CollectionAssert.AreEqual(new[] { "Reset" }.Concat(Enumerable.Repeat("Add", 10)), this.Window.FindChangesGroupBox("SourceChanges").Texts);
        }

        [Test]
        public void FilterThenTrigger()
        {
            this.AddTenButton.Click();
            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4" }, this.ListBox.Items.Select(x => x.FindTextBlock().Text));
            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4" }, this.DataGrid.ColumnValues(0));

            this.FilterTextBox.Text = "2";
            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4" }, this.ListBox.Items.Select(x => x.FindTextBlock().Text));
            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4" }, this.DataGrid.ColumnValues(0));

            this.TriggerButton.Click();
            CollectionAssert.AreEqual(new[] { "1" }, this.ListBox.Items.Select(x => x.FindTextBlock().Text));
            CollectionAssert.AreEqual(new[] { "1" }, this.DataGrid.ColumnValues(0));
        }

        [Test]
        public void FilterThenTriggerOnOtherThread()
        {
            this.AddTenButton.Click();
            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4" }, this.ListBox.Items.Select(x => x.FindTextBlock().Text));
            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4" }, this.DataGrid.ColumnValues(0));

            this.FilterTextBox.Text = "2";
            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4" }, this.ListBox.Items.Select(x => x.FindTextBlock().Text));
            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4" }, this.DataGrid.ColumnValues(0));

            this.TriggerOnOtherThreadButton.Click();
            CollectionAssert.AreEqual(new[] { "1" }, this.ListBox.Items.Select(x => x.FindTextBlock().Text));
            CollectionAssert.AreEqual(new[] { "1" }, this.DataGrid.ColumnValues(0));
        }

        [Test]
        public void AddOneOnOtherThread()
        {
            this.AddOneOnOtherThreadButton.Click();
            CollectionAssert.AreEqual(new[] { "1" }, this.ListBox.Items.Select(x => x.FindTextBlock().Text));
            CollectionAssert.AreEqual(new[] { "1" }, this.DataGrid.ColumnValues(0));
        }

        [Test]
        public void EditDataGrid()
        {
            this.AddTenButton.Click();
            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4" }, this.ListBox.Items.Select(x => x.FindTextBlock().Text));
            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4" }, this.DataGrid.ColumnValues(0));

            var cell = this.DataGrid.Rows[0].Cells[0];
            cell.Click();
            cell.AsTextBox().Text = "5";
            this.ListBox.Focus();

            CollectionAssert.AreEqual(new[] { "5", "2", "3", "4" }, this.ListBox.Items.Select(x => x.FindTextBlock().Text));
            CollectionAssert.AreEqual(new[] { "5", "2", "3", "4" }, this.DataGrid.ColumnValues(0));
        }
    }
}