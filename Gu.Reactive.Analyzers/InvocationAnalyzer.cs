namespace Gu.Reactive.Analyzers
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class InvocationAnalyzer : DiagnosticAnalyzer
    {
        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
            GUREA01DontObserveMutableProperty.Descriptor,
            GUREA03PathMustNotify.Descriptor,
            GUREA05FullPathMustHaveMoreThanOneItem.Descriptor,
            GUREA07DontNegateCondition.Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(HandleInvocation, SyntaxKind.InvocationExpression);
        }

        private static void HandleInvocation(SyntaxNodeAnalysisContext context)
        {
            if (context.IsExcludedFromAnalysis())
            {
                return;
            }

            if (context.Node is InvocationExpressionSyntax invocation &&
                context.SemanticModel.GetSymbolSafe(invocation, context.CancellationToken) is IMethodSymbol method)
            {
                if (method == KnownSymbol.NotifyPropertyChangedExt.ObservePropertyChanged ||
                    method == KnownSymbol.NotifyPropertyChangedExt.ObservePropertyChangedSlim ||
                    method == KnownSymbol.NotifyPropertyChangedExt.ObserveFullPropertyPathSlim)
                {
                    if (invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
                        memberAccess.Expression is MemberAccessExpressionSyntax member)
                    {
                        var symbol = context.SemanticModel.GetSymbolSafe(member, context.CancellationToken);
                        if (IsMutable(symbol))
                        {
                            context.ReportDiagnostic(Diagnostic.Create(GUREA01DontObserveMutableProperty.Descriptor, memberAccess.Name.GetLocation()));
                        }
                    }

                    if (TryGetSilentNode(invocation, context, out var node))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(GUREA03PathMustNotify.Descriptor, node.GetLocation()));
                    }

                    if (method == KnownSymbol.NotifyPropertyChangedExt.ObserveFullPropertyPathSlim)
                    {
                        if (invocation.ArgumentList != null &&
                            invocation.ArgumentList.Arguments.TryGetFirst(out ArgumentSyntax argument))
                        {
                            if (argument.Expression is SimpleLambdaExpressionSyntax lambda &&
                                lambda.Body != null)
                            {
                                var firstMember = lambda.Body as MemberAccessExpressionSyntax;
                                if (firstMember == null ||
                                    !(firstMember.Expression is MemberAccessExpressionSyntax))
                                {
                                    context.ReportDiagnostic(Diagnostic.Create(GUREA05FullPathMustHaveMoreThanOneItem.Descriptor, lambda.Body.GetLocation()));
                                }
                            }
                        }
                    }
                }
                else if (method == KnownSymbol.ICondition.Negate)
                {
                    if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(GUREA07DontNegateCondition.Descriptor, memberAccess.Name.GetLocation()));
                    }
                    else
                    {
                        context.ReportDiagnostic(Diagnostic.Create(GUREA07DontNegateCondition.Descriptor, invocation.GetLocation()));
                    }
                }
            }
        }

        private static bool IsMutable(ISymbol symbol)
        {
            if (symbol is IPropertySymbol property &&
                !property.IsReadOnly)
            {
                return true;
            }

            if (symbol is IFieldSymbol field &&
                !field.IsReadOnly)
            {
                return true;
            }

            return false;
        }

        private static bool TryGetSilentNode(InvocationExpressionSyntax invocation, SyntaxNodeAnalysisContext context, out SyntaxNode node)
        {
            node = null;
            if (invocation.ArgumentList != null &&
                invocation.ArgumentList.Arguments.TryGetFirst(out ArgumentSyntax argument))
            {
                if (argument.Expression is SimpleLambdaExpressionSyntax lambda)
                {
                    var memberAccess = lambda.Body as MemberAccessExpressionSyntax;
                    if (memberAccess == null)
                    {
                        node = lambda;
                        return true;
                    }

                    while (memberAccess != null)
                    {
                        var symbol = context.SemanticModel.GetSymbolSafe(memberAccess, context.CancellationToken);
                        if (!(symbol is IPropertySymbol))
                        {
                            node = memberAccess.Name;
                            return true;
                        }

                        var containingType = context.SemanticModel.GetTypeInfoSafe(memberAccess.Expression, context.CancellationToken).Type;
                        if (containingType == null)
                        {
                            continue;
                        }

                        if (containingType.IsValueType ||
                            !containingType.Is(KnownSymbol.INotifyPropertyChanged))
                        {
                            node = memberAccess.Name;
                            return true;
                        }

                        if (symbol.DeclaringSyntaxReferences.Length > 0 &&
                            !symbol.IsAbstract)
                        {
                            foreach (var reference in symbol.DeclaringSyntaxReferences)
                            {
                                var propertyDeclaration = (PropertyDeclarationSyntax)reference.GetSyntax(context.CancellationToken);
                                if (propertyDeclaration.TryGetSetAccessorDeclaration(out AccessorDeclarationSyntax setter) &&
                                    setter.Body == null)
                                {
                                    node = memberAccess.Name;
                                    return true;
                                }
                            }
                        }

                        memberAccess = memberAccess.Expression as MemberAccessExpressionSyntax;
                    }
                }
            }

            return false;
        }
    }
}