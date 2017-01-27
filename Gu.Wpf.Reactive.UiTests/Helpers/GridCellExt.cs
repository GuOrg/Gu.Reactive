namespace Gu.Wpf.Reactive.UiTests
{
    using FlaUI.Core.AutomationElements;

    public static class GridCellExt
    {
        public static string Text(this GridCell cell)
        {
            var textPattern = cell.PatternFactory.GetValuePattern();
            return textPattern?.Current.Value ?? "{NewItemPlaceholder}";
        }
    }
}