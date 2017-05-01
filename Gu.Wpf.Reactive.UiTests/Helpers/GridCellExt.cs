namespace Gu.Wpf.Reactive.UiTests
{
    using FlaUI.Core.AutomationElements;

    public static class GridCellExt
    {
        public const string NewItemPlaceholder = "{NewItemPlaceholder}";

        public static string Text(this GridCell cell)
        {
            if (cell.Patterns.Value.IsSupported)
            {
                if (cell.Value.StartsWith("Item:"))
                {
                    return string.Empty;
                }

                return cell.Value;
            }

            var text = cell.AsLabel().Text;
            if (text.Contains(NewItemPlaceholder))
            {
                return NewItemPlaceholder;
            }

            return text;
        }
    }
}