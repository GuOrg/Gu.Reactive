namespace Gu.Wpf.Reactive.UiTests
{
    using FlaUI.Core.AutomationElements;
    using FlaUI.Core.AutomationElements.Infrastructure;

    public static class AutomationElementExt
    {
        public static TextBox FindTextBox(this AutomationElement element, string name)
        {
            return element.FindFirstDescendant(x => x.ByAutomationId(name))
                          .AsTextBox();
        }

        public static Button FindButton(this AutomationElement element, string name)
        {
            return (element.FindFirstDescendant(x => x.ByAutomationId(name)) ??
                    element.FindFirstDescendant(x => x.ByText(name)))
                .AsButton();
        }

        public static CheckBox FindCheckBox(this AutomationElement element, string name)
        {
            return (element.FindFirstDescendant(x => x.ByAutomationId(name)) ??
                    element.FindFirstDescendant(x => x.ByText(name)))
                .AsCheckBox();
        }
    }
}