namespace Gu.Reactive.Analyzers.Tests.GUREA11PreferObservableFromEventTests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public partial class CodeFix
    {
        public class MiscEventHandlers
        {
            [Test]
            public void WhenNotUsingSenderNorArgLambda()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System;
    using System.IO;

    internal class Foo
    {
        public Foo()
        {
            var watcher = new FileSystemWatcher();
            ↓watcher.Created += (sender, args) => { };
        }
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    using System;
    using System.IO;
    using System.Reactive.Linq;

    internal class Foo
    {
        public Foo()
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
                RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, testCode, fixedCode);
            }
        }
    }
}
