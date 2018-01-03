namespace Gu.Wpf.Reactive.UiTests
{
    using System.Windows.Automation;
    using Gu.Wpf.UiAutomation;

    public static class AutomationElementExt
    {
        public static ChangesGroupBox FindChangesGroupBox(this UiElement element, string name) => element.FindFirst(
            TreeScope.Descendants,
            new AndCondition(Conditions.GroupBox, Conditions.ByNameOrAutomationId(name)),
            x => new ChangesGroupBox(x));
    }
}
