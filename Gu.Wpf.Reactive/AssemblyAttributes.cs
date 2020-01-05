using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Markup;

[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)]
[assembly: InternalsVisibleTo("Gu.Wpf.Reactive.Tests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100AB5BC1D032E9E344649AF43E93555B9F69733DCB59951999FEF8A5ADAB5985D5D27DFC91615F062B1AE47FACC549F07E182E0D1F247D587DB83ED87524E18E806336EAEAEB87271C60C790693A3BD6A199D896DD8ED367FD3B3E350B1B0DD78F8BD212872805939C337D96F104DAF36B79ABCEB6D3C7F0F951CC9880A66846AB", AllInternalsVisible = true)]

#pragma warning disable WPF0052 // XmlnsDefinitions does not map all namespaces with public types.
#pragma warning disable WPF0051 // XmlnsDefinition must map to existing namespace.
[assembly: XmlnsDefinition("http://Gu.com/Reactive", "Gu.Reactive")]
#pragma warning restore WPF0051 // XmlnsDefinition must map to existing namespace.
#pragma warning restore WPF0052 // XmlnsDefinitions does not map all namespaces with public types.
[assembly: XmlnsDefinition("http://Gu.com/Reactive", "Gu.Wpf.Reactive")]
[assembly: XmlnsPrefix("http://Gu.com/Reactive", "reactive")]
