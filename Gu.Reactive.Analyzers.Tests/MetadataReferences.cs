namespace Gu.Reactive.Analyzers.Tests
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Net;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;

    internal static class MetadataReferences
    {
        internal static IReadOnlyList<MetadataReference> All => MetadataReferencesAttribute.GetMetadataReferences();
    }
}