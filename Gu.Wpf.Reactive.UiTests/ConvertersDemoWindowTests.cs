namespace Gu.Wpf.Reactive.UiTests
{
    using System;

    using FlaUI.Core.AutomationElements.Infrastructure;

    using NUnit.Framework;

    public class ConvertersDemoWindowTests : WindowTests
    {
        protected override string WindowName { get; } = "ConvertersDemoWindow";

        [Test]
        public void StringComparisonToBoolConverter()
        {
            var groupBox = this.Window.FindFirstDescendant(x => x.ByText("EnumToBool"));
            var currentCultureButton = groupBox.FindFirstDescendant(x => x.ByText(StringComparison.CurrentCulture.ToString())).AsRadioButton();
            var ordinalIgnoreCaseButton = groupBox.FindFirstDescendant(x => x.ByText(StringComparison.OrdinalIgnoreCase.ToString())).AsRadioButton();
            Assert.AreEqual("CurrentCulture", groupBox.FindFirstDescendant(x => x.ByText("StringComparison")).AsLabel().Text);
            Assert.AreEqual(true, currentCultureButton.IsSelected);
            Assert.AreEqual(false, ordinalIgnoreCaseButton.IsSelected);

            ordinalIgnoreCaseButton.Click();
            Assert.AreEqual("OrdinalIgnoreCase", groupBox.FindFirstDescendant(x => x.ByText("StringComparison")).AsLabel().Text);
            Assert.AreEqual(false, currentCultureButton.IsSelected);
            Assert.AreEqual(true, ordinalIgnoreCaseButton.IsSelected);
        }
    }
}
