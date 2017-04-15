namespace Gu.Wpf.Reactive.UiTests
{
    using FlaUI.Core.AutomationElements;

    public static class GridCellExt
    {
        public const string NewItemplaceholder = "{NewItemPlaceholder}";

        public static string Text(this GridCell cell)
        {
            if (cell.Patterns.Value.IsSupported)
            {
                return cell.Value;
            }

            var text = cell.AsLabel().Text;
            if (text.Contains(NewItemplaceholder))
            {
                return NewItemplaceholder;
            }

            return text;
        }
    }
}