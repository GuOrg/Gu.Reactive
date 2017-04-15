namespace Gu.Wpf.Reactive.UiTests
{
    using System.Collections.Generic;
    using System.Linq;

    using FlaUI.Core.AutomationElements;

    public static class GridExt
    {
        public static IReadOnlyList<string> RowValues(this Grid table)
        {
            return ColumnValues(table, 0);
        }

        public static IReadOnlyList<string> ColumnValues(this Grid grid, int column)
        {
            return grid.Rows.Select(x => x.Cells[column].Text())
                       .ToArray();
        }
    }
}