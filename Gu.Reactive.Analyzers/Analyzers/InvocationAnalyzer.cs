namespace Gu.Reactive.Analyzers
{
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class InvocationAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
            Descriptors.GUREA01DoNotObserveMutableProperty,
            Descriptors.GUREA03PathMustNotify,
            Descriptors.GUREA04PreferSlimOverload,
            Descriptors.GUREA05FullPathMustHaveMoreThanOneItem,
            Descriptors.GUREA07DoNotNegateCondition,
            Descriptors.GUREA12ObservableFromEventDelegateType);

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
                context.SemanticModel.GetSymbolSafe(invocation, context.CancellationToken) is { } method)
            {
                if (method == KnownSymbol.NotifyPropertyChangedExt.ObserveFullPropertyPathSlim &&
                    TryGetInvalidFullPropertyPath(invocation, out var location))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptors.GUREA05FullPathMustHaveMoreThanOneItem, location.GetLocation()));
                }

                if (method == KnownSymbol.NotifyPropertyChangedExt.ObservePropertyChanged &&
                    TryGetCanBeSlim(invocation, context, out location))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptors.GUREA04PreferSlimOverload, location.GetLocation()));
                }

                if (method == KnownSymbol.NotifyPropertyChangedExt.ObservePropertyChanged ||
                    method == KnownSymbol.NotifyPropertyChangedExt.ObservePropertyChangedSlim ||
                    method == KnownSymbol.NotifyPropertyChangedExt.ObserveFullPropertyPathSlim)
                {
                    if (invocation.Expression is MemberAccessExpressionSyntax { Expression: MemberAccessExpressionSyntax member } memberAccess)
                    {
                        if (context.SemanticModel.GetSymbolSafe(member, context.CancellationToken) is { } symbol &&
                            IsMutable(symbol))
                        {
                            context.ReportDiagnostic(Diagnostic.Create(Descriptors.GUREA01DoNotObserveMutableProperty, memberAccess.Name.GetLocation()));
                        }
                    }

                    if (TryGetSilentNode(invocation, context, out var node))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptors.GUREA03PathMustNotify, node.GetLocation()));
                    }
                }
                else if (method == KnownSymbol.ICondition.Negate)
                {
                    if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptors.GUREA07DoNotNegateCondition, memberAccess.Name.GetLocation()));
                    }
                    else
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptors.GUREA07DoNotNegateCondition, invocation.GetLocation()));
                    }
                }
                else if (method == KnownSymbol.Observable.FromEvent &&
                     method.Parameters.Length == 2 &&
                     IsForEventHandler(method.Parameters[0]))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptors.GUREA12ObservableFromEventDelegateType, invocation.GetLocation()));
                }
            }
        }

        private static bool TryGetInvalidFullPropertyPath(InvocationExpressionSyntax invocation, [NotNullWhen(true)] out SyntaxNode? node)
        {
            node = null;
            if (invocation is { ArgumentList: { Arguments: { } arguments } } &&
                arguments.TryFirst(out ArgumentSyntax? argument) &&
                argument is { Expression: SimpleLambdaExpressionSyntax { Body: { } body } })
            {
                var firstMember = body as MemberAccessExpressionSyntax;
                if (!(firstMember?.Expression is MemberAccessExpressionSyntax))
                {
                    node = body;
                    return true;
                }
            }

            return false;
        }

        private static bool TryGetCanBeSlim(InvocationExpressionSyntax invocation, SyntaxNodeAnalysisContext context, [NotNullWhen(true)] out SyntaxNode? path)
        {
            path = null;
            if (invocation.FirstAncestor<ArgumentSyntax>() is { } argument)
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

            if (invocation.FirstAncestor<InvocationExpressionSyntax>() is { ArgumentList: { Arguments: { Count: 1 } arguments } } parentInvocation &&
                arguments[0] is { Expression: SimpleLambdaExpressionSyntax lambda } &&
                parentInvocation.IsSymbol(KnownSymbol.ObservableExtensions.Subscribe, context.SemanticModel, context.CancellationToken))
            {
                using var pooled = IdentifierNameWalker.Borrow(lambda.Body);
                if (pooled.IdentifierNames.TryFirst(x => x.Identifier.ValueText == lambda.Parameter.Identifier.ValueText, out IdentifierNameSyntax _))
                {
                    return false;
                }

                path = invocation.Expression is MemberAccessExpressionSyntax memberAccess
                    ? (SyntaxNode)memberAccess.Name
                    : invocation;
                return true;
            }

            return false;
        }

        private static bool IsMutable(ISymbol symbol)
        {
            return symbol switch
            {
                IPropertySymbol { IsReadOnly: false } => true,
                IFieldSymbol { IsReadOnly: false, IsConst: false } => true,
                _ => false
            };
        }

        private static bool TryGetSilentNode(InvocationExpressionSyntax invocation, SyntaxNodeAnalysisContext context, [NotNullWhen(true)] out SyntaxNode? node)
        {
            node = null;
            if (invocation is { ArgumentList: { Arguments: { } arguments } } &&
                arguments.TryFirst<ArgumentSyntax>(out ArgumentSyntax? argument))
            {
                if (argument.Expression is SimpleLambdaExpressionSyntax lambda)
                {
                    if (lambda.Body is MemberAccessExpressionSyntax _)
                    {
                        var memberAccess = lambda.Body as MemberAccessExpressionSyntax;
                        while (memberAccess != null)
                        {
                            var symbol = context.SemanticModel.GetSymbolSafe(memberAccess, context.CancellationToken);
                            if (!(symbol is IPropertySymbol))
                            {
                                node = memberAccess.Name;
                                return true;
                            }

                            var containingType = context.SemanticModel.GetTypeInfoSafe(
                                                            memberAccess.Expression,
                                                            context.CancellationToken)
                                                        .Type;
                            if (containingType == null)
                            {
                                continue;
                            }

                            if (containingType.IsValueType ||
                                !containingType.IsAssignableTo(KnownSymbol.INotifyPropertyChanged, context.Compilation))
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
                                    if (propertyDeclaration.TryGetSetter(out AccessorDeclarationSyntax? setter) &&
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
                    else
                    {
                        node = lambda;
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool IsForEventHandler(IParameterSymbol parameter)
        {
            return parameter.Type is INamedTypeSymbol { Name: "Action", TypeArguments: { Length: 1 } } namedType &&
                   namedType.TypeArguments[0] is INamedTypeSymbol { Name: "EventHandler" };
        }
    }
}
