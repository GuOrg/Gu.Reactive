namespace Gu.Reactive.Analyzers.Tests.GUREA11PreferObservableFromEventTests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static partial class CodeFix
    {
        public static class MiscEventHandlers
        {
            [Test]
            public static void WhenNotUsingSenderNorArgLambda()
            {
                var before = @"
namespace N
{
    using System;
    using System.IO;

    internal class C
    {
        public C()
        {
            var watcher = new FileSystemWatcher();
            ↓watcher.Created += (sender, args) => { };
        }
    }
}";

                var after = @"
namespace N
{
    using System;
    using System.IO;
    using System.Reactive.Linq;

    internal class C
    {
        public C()
        {
            var watcher = new FileSystemWatcher();
            Observable.FromEvent<FileSystemEventHandler, FileSystemEventArgs>(
                h => (_, e) => h(e),
                h => watcher.Created += h,
                h => watcher.Created -= h)
                      .Subscribe(_ => { });
        }
    }
}";
                RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, before, after);
            }
        }
    }
}
