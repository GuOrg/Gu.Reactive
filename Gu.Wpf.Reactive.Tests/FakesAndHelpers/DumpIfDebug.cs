namespace Gu.Wpf.Reactive.Tests.FakesAndHelpers
{
    using System;
    using System.Diagnostics;

    internal static class DumpIfDebug
    {
        [Conditional("DEBUG")]
        internal static void WriteLine(string text)
        {
            Console.WriteLine(text);
        }

        [Conditional("DEBUG")]
        internal static void WriteLine(string format, params object?[] args)
        {
            Console.WriteLine(format, args);
        }
    }
}
