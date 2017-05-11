namespace Gu.Reactive.Analyzers.Tests.GUREA01DontObserveMutablePropertyTests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    internal class HappyPath
    {
        [Test]
        public void SubscribingToMutablePropertyInSelf()
        {
            var fooCode = @"
namespace RoslynSandbox
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    internal class Foo : INotifyPropertyChanged
    {
        private int _value;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Value
        {
            get
            {
                return _value;
            }

            set
            {
                if (value == _value)
                {
                    return;
                }

                _value = value;
                OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}";
            var testCode = @"namespace RoslynSandbox
{
    using System;
    using Gu.Reactive;

    internal class Meh
    {
        public Meh()
        {
            foo = new Foo();
            foo.ObserveFullPropertyPathSlim(x => x.Value)
                    .Subscribe(_ => Console.WriteLine(""meh""));
        }
    }
}";
            AnalyzerAssert.NoDiagnostics<GUREA01DontObserveMutableProperty>(fooCode, testCode);
        }
    }
}