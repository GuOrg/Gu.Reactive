﻿namespace Gu.Reactive.Analyzers
{
    using System.Diagnostics;

    internal class Break
    {
        [Conditional("DEBUG")]
        internal static void IfDebug()
        {
            Debugger.Break();
        }
    }
}
