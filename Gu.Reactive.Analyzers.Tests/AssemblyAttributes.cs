using Gu.Roslyn.Asserts;


[assembly: MetadataReference(typeof(object), new[] { "global", "mscorlib" })]
[assembly: MetadataReference(typeof(System.Diagnostics.Debug), new[] { "global", "System" })]
[assembly: MetadataReferences(
    typeof(System.Linq.Enumerable),
    typeof(System.Net.WebClient),
    typeof(System.Reactive.Disposables.SerialDisposable),
    typeof(System.Reactive.Disposables.ICancelable),
    typeof(System.Reactive.Linq.Observable),
    typeof(Gu.Reactive.Condition),
    typeof(Gu.Wpf.Reactive.ConditionControl),
    typeof(System.Xml.Serialization.XmlSerializer),
    typeof(System.Windows.Media.Matrix),
    typeof(Microsoft.CodeAnalysis.CSharp.CSharpCompilation),
    typeof(Microsoft.CodeAnalysis.Compilation),
    typeof(NUnit.Framework.Assert))]
