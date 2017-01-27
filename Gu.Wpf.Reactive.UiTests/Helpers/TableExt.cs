namespace Gu.Wpf.Reactive.UiTests
{
    using System.Collections.Generic;
    using System.Linq;

    using FlaUI.Core.AutomationElements;
    using FlaUI.Core.AutomationElements.Infrastructure;

    public static class TableExt
    {
        public static IReadOnlyList<string> RowValues(this Table table)
        {
            return ColumnValues(table, 0);
        }

        public static IReadOnlyList<string> ColumnValues(this Table grid, int column)
        {
            return grid.Rows.Select(x => AutomationElementConversionExtensions.AsLabel(x.Cells[column]).Text)
                       .ToArray();
        }
    }
}