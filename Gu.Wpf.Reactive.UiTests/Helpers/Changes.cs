namespace Gu.Wpf.Reactive.UiTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Automation;
    using Gu.Wpf.UiAutomation;

    public class ChangesGroupBox : GroupBox
    {
        public ChangesGroupBox(AutomationElement automationElement)
            : base(automationElement)
        {
        }

        public IReadOnlyList<string> Texts => this.ContentElements(x => new TextBlock(x))
                                                  .Select(x => x.Text)
                                                  .ToArray();
    }
}
