namespace Gu.Wpf.Reactive.UiTests
{
    using Gu.Wpf.UiAutomation;

    public static class AutomationElementExt
    {
        public static ChangesGroupBox FindChangesGroupBox(this AutomationElement element, string name) => element.FindFirst(
            TreeScope.Descendants,
            element.CreateCondition(ControlType.Group, name),
            x => new ChangesGroupBox(x));
    }
}