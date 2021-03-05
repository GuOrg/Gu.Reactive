namespace Gu.Reactive.Analyzers.Tests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class Cs8620SuppressorTests
    {
        private static readonly DiagnosticAnalyzer Analyzer = new Cs8620Suppressor();

        [TestCase("M(IObservable<Maybe<ObservableCollection<C>>> observable) => observable.ItemPropertyChanged(x => x.P)")]
        [TestCase("M(IObservable<Maybe<ObservableCollection<C>>> observable) => NotifyCollectionChangedExt.ItemPropertyChanged(observable, x => x.P)")]
        [TestCase("M(IObservable<EventPattern<PropertyChangedAndValueEventArgs<ObservableCollection<C>>>> observable) => observable.ItemPropertyChanged(x => x.P)")]
        public static void Suppresses(string text)
        {
            var code = @"
#nullable enable
namespace N
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Reactive;
    using System.Runtime.CompilerServices;
    using Gu.Reactive;

    public class C : INotifyPropertyChanged
    {
        private string? p;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string? P
        {
            get => this.p;
            set
            {
                if (value == this.p)
                {
                    return;
                }

                this.p = value;
                this.OnPropertyChanged();
            }
        }

        public static object M(IObservable<Maybe<ObservableCollection<C>>> observable) => observable.ItemPropertyChanged(x => x.P);

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}".AssertReplace("M(IObservable<Maybe<ObservableCollection<C>>> observable) => observable.ItemPropertyChanged(x => x.P)", text);
            RoslynAssert.Valid(Analyzer, code);
        }
    }
}
