namespace Gu.Wpf.Reactive.UiTests
{
    using System.Collections.Generic;
    using System.Linq;

    using FlaUI.Core.AutomationElements;
    using FlaUI.Core.AutomationElements.Infrastructure;
    using FlaUI.Core.Definitions;

    using NUnit.Framework;

    public class ReadonlyFilteredDispatchingWindowTests : WindowTests
    {
        protected override string WindowName { get; } = "ReadonlyFilteredDispatchingWindow";

        private Table ListBox => this.Window.FindFirstDescendant(x => x.ByText("ListBox"))
                                     .FindFirstDescendant(x => x.ByControlType(ControlType.List))
                                     .AsTable();

        private Table DataGrid => this.Window.FindFirstDescendant(x => x.ByText("DataGrid"))
                             .FindFirstDescendant(x => x.ByControlType(ControlType.DataGrid))
                             .AsTable();

        private IEnumerable<Label> ViewChanges => this.Window.FindFirstDescendant(x => x.ByText("ViewChanges"))
                                                             .FindAllChildren().Skip(1).Select(x => x.AsLabel());

        private IEnumerable<Label> SourceChanges => this.Window.FindFirstDescendant(x => x.ByText("SourceChanges"))
                                                               .FindAllChildren().Skip(1).Select(x => x.AsLabel());

        [Test]
        public void Initializes()
        {
            CollectionAssert.AreEqual(new[] { "1", "2", "3" }, this.ListBox.Rows.Select(x => x.Cells[0].AsLabel().Text));
            CollectionAssert.AreEqual(new[] { "1", "2", "3" }, this.DataGrid.Rows.Select(x => x.Cells[0].AsLabel().Text));

            CollectionAssert.AreEqual(new[] { "Reset" }, this.ViewChanges.Select(x => x.Text));
            CollectionAssert.AreEqual(new[] { "Reset" }, this.SourceChanges.Select(x => x.Text));
        }
    }
}