namespace Gu.Reactive.Analyzers
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ConstructorAnalyzer : DiagnosticAnalyzer
    {
        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
            GUREA02ObservableAndCriteriaMustMatch.Descriptor,
            GUREA06DontNewCondition.Descriptor,
            GUREA08InlineSingleLine.Descriptor,
            GUREA09ObservableBeforeCriteria.Descriptor,
            GUREA10DontMergeInObservable.Descriptor,
            GUREA13SyncParametersAndArgs.Descriptor);

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(Handle, SyntaxKind.BaseConstructorInitializer, SyntaxKind.ObjectCreationExpression);
        }

        private static void Handle(SyntaxNodeAnalysisContext context)
        {
            if (context.IsExcludedFromAnalysis())
            {
                return;
            }

            if (context.Node is ConstructorInitializerSyntax initializer &&
                context.SemanticModel.GetSymbolSafe(initializer, context.CancellationToken) is IMethodSymbol baseCtor)
            {
                if (baseCtor.ContainingType == KnownSymbol.Condition)
                {
                    if (TryGetObservableAndCriteriaMismatch(initializer.ArgumentList, baseCtor, context, out var observedText, out var criteriaText, out var missingText))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(GUREA02ObservableAndCriteriaMustMatch.Descriptor, initializer.GetLocation(), observedText, criteriaText, missingText));
                    }

                    if (TryGetObservableArgument(initializer.ArgumentList, baseCtor, out var observableArgument) &&
                        CanBeInlined(observableArgument, context, out _))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(GUREA08InlineSingleLine.Descriptor, observableArgument.GetLocation()));
                    }

                    if (TryGetCriteriaArgument(initializer.ArgumentList, baseCtor, out var criteriaArgument) &&
                        CanBeInlined(criteriaArgument, context, out var inline))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(GUREA08InlineSingleLine.Descriptor, inline.GetLocation()));
                    }

                    if (IsObservableBeforeCriteria(baseCtor))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(GUREA09ObservableBeforeCriteria.Descriptor, initializer.ArgumentList.GetLocation()));
                    }

                    if (MergesObservable(initializer.ArgumentList, baseCtor, context, out var location))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(GUREA10DontMergeInObservable.Descriptor, location.GetLocation()));
                    }
                }
                else if (baseCtor.ContainingType.IsEither(KnownSymbol.AndCondition, KnownSymbol.OrCondition) &&
                         HasMatchingArgumentAndParameterPositions(initializer, context) == false)
                {
                    context.ReportDiagnostic(Diagnostic.Create(GUREA13SyncParametersAndArgs.Descriptor, initializer.ArgumentList.GetLocation()));
                }
            }
            else if (context.Node is ObjectCreationExpressionSyntax objectCreation &&
                     context.SemanticModel.GetSymbolSafe(objectCreation, context.CancellationToken) is IMethodSymbol ctor)
            {
                if (ctor.ContainingType == KnownSymbol.Condition)
                {
                    if (TryGetObservableAndCriteriaMismatch(objectCreation.ArgumentList, ctor, context, out var observedText, out var criteriaText, out var missingText))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(GUREA02ObservableAndCriteriaMustMatch.Descriptor, objectCreation.GetLocation(), observedText, criteriaText, missingText));
                    }

                    if (IsObservableBeforeCriteria(ctor))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(GUREA09ObservableBeforeCriteria.Descriptor, objectCreation.ArgumentList.GetLocation()));
                    }
                }

                if (ctor.ContainingType.IsAssignableTo(KnownSymbol.Condition, context.Compilation))
                {
                    context.ReportDiagnostic(Diagnostic.Create(GUREA06DontNewCondition.Descriptor, objectCreation.GetLocation()));
                }
            }
        }

        private static bool IsObservableBeforeCriteria(IMethodSymbol ctor)
        {
            return ctor.Parameters.TryElementAt(0, out var parameter0) &&
                   parameter0.Type == KnownSymbol.FuncOfT &&
                   ctor.Parameters.TryElementAt(1, out var parameter1) &&
                   parameter1.Type == KnownSymbol.IObservableOfT;
        }

        private static bool CanBeInlined(ArgumentSyntax argument, SyntaxNodeAnalysisContext context, out InvocationExpressionSyntax result)
        {
            result = null;
            if (argument == null)
            {
                return false;
            }

            if (argument.Expression is InvocationExpressionSyntax argumentInvocation &&
                CanBeInlined(argumentInvocation))
            {
                result = argumentInvocation;
                return true;
            }

            if (argument.Expression is LambdaExpressionSyntax lambda &&
                lambda.Body is InvocationExpressionSyntax lambdaInvocation &&
                CanBeInlined(lambdaInvocation))
            {
                result = lambdaInvocation;
                return true;
            }

            return false;

            bool CanBeInlined(InvocationExpressionSyntax invocation)
            {
                if (context.SemanticModel.GetSymbolSafe(invocation, context.CancellationToken) is IMethodSymbol method &&
                    method.IsStatic &&
                    method.TrySingleDeclaration<MethodDeclarationSyntax>(context.CancellationToken, out var declaration))
                {
                    if (declaration.ExpressionBody != null)
                    {
                        return true;
                    }

                    if (declaration.Body is BlockSyntax block &&
                        block.Statements.TrySingle(out StatementSyntax statement))
                    {
                        return statement is ReturnStatementSyntax;
                    }
                }

                return false;
            }
        }

        private static bool TryGetObservableAndCriteriaMismatch(ArgumentListSyntax argumentList, IMethodSymbol ctor, SyntaxNodeAnalysisContext context, out string observedText, out string criteriaText, out string missingText)
        {
            if (TryGetObservableArgument(argumentList, ctor, out var observableArg) &&
                TryGetCriteriaArgument(argumentList, ctor, out var criteriaArg))
            {
                using (var observableIdentifiers = IdentifierNameExecutionWalker.Create(observableArg, SearchScope.Recursive, context.SemanticModel, context.CancellationToken))
                {
                    using (var criteriaIdentifiers = IdentifierNameExecutionWalker.Create(criteriaArg, SearchScope.Recursive, context.SemanticModel, context.CancellationToken))
                    {
                        bool observesInterval = false;
                        using (var observed = PooledSet<IPropertySymbol>.Borrow())
                        {
                            foreach (var name in observableIdentifiers.IdentifierNames)
                            {
                                if (context.SemanticModel.TryGetSymbol(name, context.CancellationToken, out ISymbol symbol))
                                {
                                    if (symbol is IPropertySymbol property)
                                    {
                                        observed.Add(property);
                                    }

                                    if (symbol is IMethodSymbol method &&
                                        method == KnownSymbol.Observable.Interval)
                                    {
                                        observesInterval = true;
                                    }
                                }
                            }

                            using (var usedInCriteria = PooledSet<IPropertySymbol>.Borrow())
                            {
                                foreach (var name in criteriaIdentifiers.IdentifierNames)
                                {
                                    if (context.SemanticModel.TryGetSymbol(name, context.CancellationToken, out IPropertySymbol property) &&
                                        !property.ContainingType.IsValueType &&
                                        !property.IsGetOnly() &&
                                        !property.IsPrivateSetAssignedInCtorOnly(context.SemanticModel, context.CancellationToken))
                                    {
                                        usedInCriteria.Add(property);
                                    }
                                }

                                using (var missing = PooledSet<IPropertySymbol>.Borrow())
                                {
                                    missing.UnionWith(usedInCriteria);
                                    missing.ExceptWith(observed);
                                    if (observesInterval)
                                    {
                                        missing.ExceptWith(missing.Where(x => x.Name == "Now").ToArray());
                                    }

                                    if (missing.Count != 0)
                                    {
                                        observedText = string.Join(Environment.NewLine, observed.Select(p => $"  {p}"));
                                        criteriaText = string.Join(Environment.NewLine, usedInCriteria.Select(p => $"  {p}"));
                                        missingText = string.Join(Environment.NewLine, missing.Select(p => $"  {p}"));
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            observedText = null;
            criteriaText = null;
            missingText = null;
            return false;
        }

        private static bool MergesObservable(ArgumentListSyntax argumentList, IMethodSymbol ctor, SyntaxNodeAnalysisContext context, out SyntaxNode location)
        {
            if (TryGetObservableArgument(argumentList, ctor, out var argument))
            {
                using (var pooled = InvocationExecutionWalker.Borrow(argument, SearchScope.Recursive, context.SemanticModel, context.CancellationToken))
                {
                    foreach (var invocation in pooled.Invocations)
                    {
                        if (context.SemanticModel.GetSymbolSafe(invocation, context.CancellationToken) == KnownSymbol.Observable.Merge)
                        {
                            location = argument;
                            return true;
                        }
                    }
                }
            }

            location = null;
            return false;
        }

        private static bool? HasMatchingArgumentAndParameterPositions(ConstructorInitializerSyntax initializer, SyntaxNodeAnalysisContext context)
        {
            if (initializer?.ArgumentList == null)
            {
                return null;
            }

            if (context.SemanticModel.GetDeclaredSymbolSafe(initializer.Parent, context.CancellationToken) is IMethodSymbol ctor)
            {
                if (ctor.Parameters.Length != initializer.ArgumentList.Arguments.Count)
                {
                    return null;
                }

                for (var i = 0; i < initializer.ArgumentList.Arguments.Count; i++)
                {
                    var argument = initializer.ArgumentList.Arguments[i];
                    if (argument.Expression is IdentifierNameSyntax argName &&
                        argName.Identifier.ValueText != ctor.Parameters[i].Name)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool TryGetObservableArgument(ArgumentListSyntax argumentList, IMethodSymbol ctor, out ArgumentSyntax argument)
        {
            if (ctor.Parameters[0].Type == KnownSymbol.IObservableOfT &&
                ctor.Parameters[1].Type == KnownSymbol.FuncOfT)
            {
                argument = argumentList.Arguments[0];
                return true;
            }

            if (ctor.Parameters[0].Type == KnownSymbol.FuncOfT &&
                ctor.Parameters[1].Type == KnownSymbol.IObservableOfT)
            {
                argument = argumentList.Arguments[1];
                return true;
            }

            argument = null;
            return false;
        }

        private static bool TryGetCriteriaArgument(ArgumentListSyntax argumentList, IMethodSymbol ctor, out ArgumentSyntax argument)
        {
            if (ctor.Parameters[0].Type == KnownSymbol.IObservableOfT &&
                ctor.Parameters[1].Type == KnownSymbol.FuncOfT)
            {
                argument = argumentList.Arguments[1];
                return true;
            }

            if (ctor.Parameters[0].Type == KnownSymbol.FuncOfT &&
                ctor.Parameters[1].Type == KnownSymbol.IObservableOfT)
            {
                argument = argumentList.Arguments[0];
                return true;
            }

            argument = null;
            return false;
        }
    }
}
