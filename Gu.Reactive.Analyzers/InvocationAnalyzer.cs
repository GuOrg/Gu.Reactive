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
            GUREA04PreferSlim.Descriptor,
            GUREA05FullPathMustHaveMoreThanOneItem.Descriptor,
            GUREA07DontNegateCondition.Descriptor,
            GUREA12ObservableFromEventDelegateType.Descriptor);

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
                if (method == KnownSymbol.NotifyPropertyChangedExt.ObserveFullPropertyPathSlim &&
                    TryGetInvalidFullPropertyPath(invocation, context, out var location))
                {
                    context.ReportDiagnostic(Diagnostic.Create(GUREA05FullPathMustHaveMoreThanOneItem.Descriptor, location.GetLocation()));
                }

                if (method == KnownSymbol.NotifyPropertyChangedExt.ObservePropertyChanged &&
                    TryGetCanBeSlim(invocation, context, out location))
                {
                    context.ReportDiagnostic(Diagnostic.Create(GUREA04PreferSlim.Descriptor, location.GetLocation()));
                }

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
                else if (method == KnownSymbol.Observable.FromEvent &&
                     method.Parameters.Length == 2 &&
                     IsForEventHandler(method.Parameters[0]))
                {
                    context.ReportDiagnostic(Diagnostic.Create(GUREA12ObservableFromEventDelegateType.Descriptor, invocation.GetLocation()));
                }
            }
        }

        private static bool TryGetInvalidFullPropertyPath(InvocationExpressionSyntax invocation, SyntaxNodeAnalysisContext context, out SyntaxNode node)
        {
            node = null;
            if (invocation.ArgumentList != null &&
                invocation.ArgumentList.Arguments.TryGetFirst(out ArgumentSyntax argument))
            {
                if (argument.Expression is SimpleLambdaExpressionSyntax lambda &&
                    lambda.Body != null)
                {
                    var firstMember = lambda.Body as MemberAccessExpressionSyntax;
                    if (!(firstMember?.Expression is MemberAccessExpressionSyntax))
                    {
                        node = lambda.Body;
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool TryGetCanBeSlim(InvocationExpressionSyntax invocation, SyntaxNodeAnalysisContext context, out SyntaxNode path)
        {
            path = null;
            var argument = invocation.FirstAncestor<ArgumentSyntax>();
            if (argument != null)
            {
                if (argument.TryGetParameter(context.SemanticModel, context.CancellationToken, out var parameter) &&
                    parameter.Type == KnownSymbol.IObservableOfT)
                {
                    if (parameter.Type is INamedTypeSymbol namedType &&
                        namedType.TypeArguments[0] == KnownSymbol.Object)
                    {
                        path = invocation.Expression is MemberAccessExpressionSyntax memberAccess
                            ? (SyntaxNode)memberAccess.Name
                            : invocation;
                        return true;
                    }
                }
            }

            var parentInvocation = invocation.FirstAncestor<InvocationExpressionSyntax>();
            if (parentInvocation != null)
            {
                var parentMethod = (IMethodSymbol)context.SemanticModel.GetSymbolSafe(parentInvocation, context.CancellationToken);
                if (parentMethod == KnownSymbol.ObservableExtensions.Subscribe &&
                    parentInvocation.ArgumentList?.Arguments.TryGetSingle(out argument) == true)
                {
                    if (argument.Expression is SimpleLambdaExpressionSyntax lambda)
                    {
                        using (var pooled = IdentifierNameWalker.Create(lambda.Body))
                        {
                            if (pooled.Item.IdentifierNames.TryGetFirst(x => x.Identifier.ValueText == lambda.Parameter.Identifier.ValueText, out IdentifierNameSyntax _))
                            {
                                return false;
                            }

                            path = invocation.Expression is MemberAccessExpressionSyntax memberAccess
                                ? (SyntaxNode)memberAccess.Name
                                : invocation;
                            return true;
                        }
                    }
                }
            }

            return false;
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

        private static bool IsForEventHandler(IParameterSymbol parameter)
        {
            if (parameter.Type is INamedTypeSymbol namedType &&
                namedType.Name == "Action" &&
                namedType.TypeArguments.Length == 1 &&
                namedType.TypeArguments[0] is INamedTypeSymbol argType)
            {
                return argType.Name == "EventHandler";
            }

            return false;
        }
    }
}