namespace Gu.Reactive.Analyzers
{
    using System.Collections.Immutable;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class InvocationAnalyzer : DiagnosticAnalyzer
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
            if (!context.IsExcludedFromAnalysis() &&
                context.Node is InvocationExpressionSyntax invocation &&
                context.SemanticModel.GetSymbolSafe(invocation, context.CancellationToken) is { } method)
            {
                if (method == KnownSymbol.NotifyPropertyChangedExt.ObserveFullPropertyPathSlim &&
                    FindInvalidFullPropertyPath(invocation) is { } invalid)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptors.GUREA05FullPathMustHaveMoreThanOneItem, invalid.GetLocation()));
                }

                if (method == KnownSymbol.NotifyPropertyChangedExt.ObservePropertyChanged &&
                    FindCanBeSlim(invocation, context) is { } canBeSlim)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptors.GUREA04PreferSlimOverload, canBeSlim.GetLocation()));
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

                    if (FindSilentNode(invocation, context) is { } silentNode)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptors.GUREA03PathMustNotify, silentNode.GetLocation()));
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

        private static SyntaxNode? FindInvalidFullPropertyPath(InvocationExpressionSyntax invocation)
        {
            if (invocation is { ArgumentList: { Arguments: { } arguments } } &&
                arguments.TryFirst(out var argument) &&
                argument is { Expression: SimpleLambdaExpressionSyntax { Body: MemberAccessExpressionSyntax { Expression: not MemberAccessExpressionSyntax } body } })
            {
                return body;
            }

            return null;
        }

        private static SyntaxNode? FindCanBeSlim(InvocationExpressionSyntax invocation, SyntaxNodeAnalysisContext context)
        {
            if (invocation.FirstAncestor<ArgumentSyntax>() is { } argument &&
                FindContainingParameter() is { Type: INamedTypeSymbol { TypeArguments: { Length: 1 } } namedType } &&
                namedType == KnownSymbol.IObservableOfT &&
                namedType.TypeArguments[0] == KnownSymbol.Object)
            {
                return invocation.Expression is MemberAccessExpressionSyntax memberAccess
                    ? (SyntaxNode)memberAccess.Name
                    : invocation;
            }

            if (invocation.FirstAncestor<InvocationExpressionSyntax>() is { ArgumentList: { Arguments: { Count: 1 } arguments } } parentInvocation &&
                arguments[0] is { Expression: SimpleLambdaExpressionSyntax lambda } &&
                parentInvocation.IsSymbol(KnownSymbol.ObservableExtensions.Subscribe, context.SemanticModel, context.CancellationToken))
            {
                using var pooled = IdentifierNameWalker.Borrow(lambda.Body);
                if (pooled.IdentifierNames.TryFirst(x => x.Identifier.ValueText == lambda.Parameter.Identifier.ValueText, out var _))
                {
                    return null;
                }

                return invocation.Expression is MemberAccessExpressionSyntax memberAccess
                    ? (SyntaxNode)memberAccess.Name
                    : invocation;
            }

            return null;

            IParameterSymbol? FindContainingParameter()
            {
                if (argument is { Parent: ArgumentListSyntax { Parent: { } parent } } &&
                    context.SemanticModel.TryGetSymbol(parent, context.CancellationToken, out IMethodSymbol? method) &&
                    method.TryFindParameter(argument, out var result))
                {
                    return result;
                }

                return null;
            }
        }

        private static bool IsMutable(ISymbol symbol)
        {
            return symbol switch
            {
                IPropertySymbol { IsReadOnly: false } => true,
                IFieldSymbol { IsReadOnly: false, IsConst: false } => true,
                _ => false,
            };
        }

        private static SyntaxNode? FindSilentNode(InvocationExpressionSyntax invocation, SyntaxNodeAnalysisContext context)
        {
            if (invocation is { ArgumentList: { Arguments: { } arguments } } &&
                arguments.TryFirst(out var argument))
            {
                return argument.Expression switch
                {
                    SimpleLambdaExpressionSyntax { Body: MemberAccessExpressionSyntax memberAccess } => FindSilentNode(memberAccess),
                    SimpleLambdaExpressionSyntax { Body: InvocationExpressionSyntax call } => call,
                    InvocationExpressionSyntax { Expression: IdentifierNameSyntax { Identifier: { ValueText: "nameof" } }, ArgumentList: { Arguments: { Count: 1 } args } }
                    => args[0].Expression switch
                    {
                        MemberAccessExpressionSyntax memberAccess => FindSilentNode(memberAccess),
                        _ => null,
                    },
                    _ => null,
                };
            }

            return null;

            SyntaxNode? FindSilentNode(MemberAccessExpressionSyntax memberAccess)
            {
                return context.SemanticModel.GetSymbolSafe(memberAccess, context.CancellationToken) switch
                {
                    IPropertySymbol { ContainingType: { } containingType }
                    when containingType.IsValueType ||
                         !Notifies(containingType)
                    => memberAccess.Name,
                    IPropertySymbol { SetMethod: { }, IsAbstract: false } property
                    when property.IsAutoProperty()
                    => memberAccess.Name,
                    IPropertySymbol _
                    when memberAccess.Expression is MemberAccessExpressionSyntax next
                    => FindSilentNode(next),
                    IPropertySymbol _ => null,
                    _ => memberAccess.Name,
                };

                bool Notifies(INamedTypeSymbol containingType)
                {
                    if (containingType.IsAssignableTo(KnownSymbol.INotifyPropertyChanged, context.Compilation))
                    {
                        return true;
                    }

                    if (context.SemanticModel.TryGetType(memberAccess.Expression, context.CancellationToken, out var type))
                    {
                        return type.IsAssignableTo(KnownSymbol.INotifyPropertyChanged, context.Compilation);
                    }

                    return false;
                }
            }
        }

        private static bool IsForEventHandler(IParameterSymbol parameter)
        {
            return parameter.Type is INamedTypeSymbol { Name: "Action", TypeArguments: { Length: 1 } } namedType &&
                   namedType.TypeArguments[0] is INamedTypeSymbol { Name: "EventHandler" };
        }
    }
}
