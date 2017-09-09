namespace Gu.Wpf.Reactive.UiTests
{
    using System.Collections.Generic;
    using System.Linq;
    using Gu.Wpf.UiAutomation;

    public class ChangesGroupBox : GroupBox
    {
        public ChangesGroupBox(BasicAutomationElementBase basicAutomationElement)
            : base(basicAutomationElement)
        {
        }

        public IReadOnlyList<string> Texts => this.ContentElements(x => new TextBlock(x))
                                                  .Select(x => x.Text)
                                                  .ToArray();
    }
}
