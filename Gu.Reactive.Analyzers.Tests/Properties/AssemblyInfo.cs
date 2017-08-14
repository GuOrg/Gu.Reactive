using System.Reflection;
using System.Runtime.InteropServices;
using Gu.Roslyn.Asserts;

[assembly: AssemblyTitle("Gu.Reactive.Analyzers.Tests")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Gu.Reactive.Analyzers.Tests")]
[assembly: AssemblyCopyright("Copyright ©  2017")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]

[assembly: Guid("f154810d-9d97-4efe-8a86-1312a780802c")]

// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: MetadataReference(typeof(object), new[] { "global", "mscorlib" })]
[assembly: MetadataReference(typeof(System.Diagnostics.Debug), new[] { "global", "system" })]
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
