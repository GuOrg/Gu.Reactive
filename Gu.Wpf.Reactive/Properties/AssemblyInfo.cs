using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

[assembly: AssemblyTitle("Gu.Wpf.Reactive")]
[assembly: AssemblyDescription("Wpf related components for Gu.Reactive")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Johan Larsson")]
[assembly: AssemblyProduct("Gu.Wpf.Reactive")]
[assembly: AssemblyCopyright("Copyright ©  2014")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)]

[assembly: AssemblyVersion("2.0.2.0")]
[assembly: AssemblyFileVersion("2.0.2.0")]
[assembly: NeutralResourcesLanguage("en")]
[assembly: Guid("1E55FADA-8E21-45DC-B416-224C1956AD39")]
[assembly: InternalsVisibleTo("Gu.Wpf.Reactive.Tests", AllInternalsVisible = true)]

#pragma warning disable WPF0052 // XmlnsDefinitions does not map all namespaces with public types.
#pragma warning disable WPF0051 // XmlnsDefinition must map to existing namespace.
[assembly: XmlnsDefinition("http://Gu.com/Reactive", "Gu.Reactive")]
#pragma warning restore WPF0051 // XmlnsDefinition must map to existing namespace.
#pragma warning restore WPF0052 // XmlnsDefinitions does not map all namespaces with public types.
[assembly: XmlnsDefinition("http://Gu.com/Reactive", "Gu.Wpf.Reactive")]
[assembly: XmlnsPrefix("http://Gu.com/Reactive", "reactive")]

#pragma warning disable SA1649 // File name must match first type name
//// ReSharper disable once UnusedMember.Global
//// ReSharper disable once CheckNamespace
namespace Gu.Wpf.Reactive.Properties
{
    internal class References
    {
        // Touching Gu.Wpf.ToolTips so that it gets copied to output dir
        // http://stackoverflow.com/a/24828522/1069200
#pragma warning disable 649
#pragma warning disable 169
        public static readonly Gu.Wpf.ToolTips.PopupButton ToolTipsReference;
#pragma warning restore 169
#pragma warning restore 649
    }
}
#pragma warning restore SA1649 // File name must match first type name
