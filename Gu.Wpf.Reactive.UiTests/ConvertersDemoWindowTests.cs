namespace Gu.Wpf.Reactive.UiTests
{
    using System;

    using NUnit.Framework;

    using TestStack.White.UIItems;

    public class ConvertersDemoWindowTests : WindowTests
    {
        protected override string WindowName { get; } = "ConvertersDemoWindow";

        [Test]
        public void StringComparisonToBoolConverter()
        {
            var groupBox = this.Window.GetByText<GroupBox>("EnumToBool");
            var currentCultureButton = groupBox.GetByText<RadioButton>(StringComparison.CurrentCulture.ToString());
            var ordinalIgnoreCaseButton = groupBox.GetByText<RadioButton>(StringComparison.OrdinalIgnoreCase.ToString());
            Assert.AreEqual("CurrentCulture", groupBox.Get<Label>("StringComparison").Text);
            Assert.AreEqual(true, currentCultureButton.IsSelected);
            Assert.AreEqual(false, ordinalIgnoreCaseButton.IsSelected);

            ordinalIgnoreCaseButton.Click();
            Assert.AreEqual("OrdinalIgnoreCase", groupBox.Get<Label>("StringComparison").Text);
            Assert.AreEqual(false, currentCultureButton.IsSelected);
            Assert.AreEqual(true, ordinalIgnoreCaseButton.IsSelected);
        }
    }
}
