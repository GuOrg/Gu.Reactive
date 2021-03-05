namespace Gu.Wpf.Reactive.Tests.FakesAndHelpers
{
    using System;

    public class CommandListener
    {
#pragma warning disable CA2109 // Review visible event handlers
        public void React(object? sender, EventArgs e)
#pragma warning restore CA2109 // Review visible event handlers
        {
        }
    }
}
