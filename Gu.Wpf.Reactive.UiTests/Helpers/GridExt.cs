namespace Gu.Wpf.Reactive.UiTests
{
    using System.Collections.Generic;
    using System.Linq;
    using Gu.Wpf.UiAutomation;

    public static class GridExt
    {
        public static IReadOnlyList<string> ColumnValues(this DataGrid dataGrid, int column)
        {
            return dataGrid.Rows.Select(x => x.Cells[column].Value)
                           .ToArray();
        }
    }
}