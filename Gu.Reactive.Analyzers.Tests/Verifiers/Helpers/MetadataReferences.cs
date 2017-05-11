﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Gu.Reactive.Analyzers.Tests.Verifiers.Helpers
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Gu.Wpf.Reactive;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    /// <summary>
    /// Metadata references used to create test projects.
    /// </summary>
    internal static class MetadataReferences
    {
        internal static readonly MetadataReference MsCorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location).WithAliases(ImmutableArray.Create("global", "corlib"));
        internal static readonly MetadataReference System = MetadataReference.CreateFromFile(typeof(System.Diagnostics.Debug).Assembly.Location).WithAliases(ImmutableArray.Create("global", "system"));
        internal static readonly MetadataReference SystemCore = MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location);
        internal static readonly MetadataReference SystemReactive = MetadataReference.CreateFromFile(typeof(System.Reactive.Disposables.SerialDisposable).Assembly.Location);
        internal static readonly MetadataReference SystemReactiveInterfaces = MetadataReference.CreateFromFile(typeof(System.Reactive.Disposables.ICancelable).Assembly.Location);
        internal static readonly MetadataReference SystemReactiveLinq = MetadataReference.CreateFromFile(typeof(System.Reactive.Linq.Observable).Assembly.Location);
        internal static readonly MetadataReference SystemXml = MetadataReference.CreateFromFile(typeof(System.Xml.Serialization.XmlSerializer).Assembly.Location);
        internal static readonly MetadataReference CSharpSymbols = MetadataReference.CreateFromFile(typeof(CSharpCompilation).Assembly.Location);
        internal static readonly MetadataReference CodeAnalysis = MetadataReference.CreateFromFile(typeof(Compilation).Assembly.Location);
        internal static readonly MetadataReference NUnit = MetadataReference.CreateFromFile(typeof(Assert).Assembly.Location);
        internal static readonly MetadataReference GuReactive = MetadataReference.CreateFromFile(typeof(Condition).Assembly.Location);
        internal static readonly MetadataReference GuWpfReactive = MetadataReference.CreateFromFile(typeof(ConditionControl).Assembly.Location);

        internal static IReadOnlyList<MetadataReference> All =>
                new[]
                    {
                        MsCorlib,
                        System,
                        SystemCore,
                        SystemReactive,
                        SystemReactiveInterfaces,
                        SystemReactiveLinq,
                        SystemXml,
                        CSharpSymbols,
                        CodeAnalysis,
                        NUnit,
                        GuReactive,
                        GuWpfReactive,
                    };
    }
}
