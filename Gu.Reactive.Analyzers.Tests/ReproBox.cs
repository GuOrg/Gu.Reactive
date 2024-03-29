﻿namespace Gu.Reactive.Analyzers.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    [Explicit]
    public class ReproBox
    {
        // ReSharper disable once UnusedMember.Local
        private static readonly IReadOnlyList<DiagnosticAnalyzer> AllAnalyzers =
            typeof(KnownSymbol).Assembly.GetTypes()
                               .Where(typeof(DiagnosticAnalyzer).IsAssignableFrom)
                               .Select(t => (DiagnosticAnalyzer)Activator.CreateInstance(t)!)
                               .ToArray();

        private static readonly Solution Solution = CodeFactory.CreateSolution(
            new FileInfo("C:\\Git\\Gu.State\\Gu.State.sln"));

        [TestCaseSource(nameof(AllAnalyzers))]
        public void SolutionRepro(DiagnosticAnalyzer analyzer)
        {
            RoslynAssert.Valid(analyzer, Solution);
        }

        [TestCaseSource(nameof(AllAnalyzers))]
        public void Repro(DiagnosticAnalyzer analyzer)
        {
            var code = @"
namespace N
{
    public sealed class C
    {
    }
}";

            RoslynAssert.Valid(analyzer, code);
        }
    }
}
