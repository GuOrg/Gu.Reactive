namespace Gu.Reactive.Analyzers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal sealed class MutationWalker : PooledWalker<MutationWalker>, IReadOnlyList<SyntaxNode>
    {
        private readonly List<SyntaxNode> mutations = new List<SyntaxNode>();

        private MutationWalker()
        {
        }

        public int Count => this.mutations.Count;

        public SyntaxNode this[int index] => this.mutations[index];

        public IEnumerator<SyntaxNode> GetEnumerator() => this.mutations.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            this.mutations.Add(node);
            base.VisitAssignmentExpression(node);
        }

        public override void VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.LogicalNotExpression:
                    break;
                default:
                    this.mutations.Add(node);
                    break;
            }

            base.VisitPrefixUnaryExpression(node);
        }

        public override void VisitPostfixUnaryExpression(PostfixUnaryExpressionSyntax node)
        {
            this.mutations.Add(node);
            base.VisitPostfixUnaryExpression(node);
        }

        public override void VisitArgument(ArgumentSyntax node)
        {
            if (node.RefOrOutKeyword.IsKind(SyntaxKind.RefKeyword) ||
                node.RefOrOutKeyword.IsKind(SyntaxKind.OutKeyword))
            {
                this.mutations.Add(node);
            }

            base.VisitArgument(node);
        }

        internal static MutationWalker Borrow(SyntaxNode node) => BorrowAndVisit(node, () => new MutationWalker());

        internal static MutationWalker Borrow(SyntaxNode node, ISymbol symbol, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            var walker = BorrowAndVisit(node, () => new MutationWalker());
            walker.mutations.RemoveAll(x => !IsForSymbol(x, symbol, semanticModel, cancellationToken))
                            .IgnoreReturnValue();
            return walker;
        }

        protected override void Clear()
        {
            this.mutations.Clear();
        }

        private static bool IsForSymbol(SyntaxNode mutation, ISymbol symbol, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (mutation is AssignmentExpressionSyntax assignmentExpression)
            {
                return Equals(semanticModel.GetSymbolSafe(assignmentExpression.Left, cancellationToken), symbol);
            }

            if (mutation is PrefixUnaryExpressionSyntax prefix)
            {
                return Equals(semanticModel.GetSymbolSafe(prefix.Operand, cancellationToken), symbol);
            }

            if (mutation is PostfixUnaryExpressionSyntax postfix)
            {
                return Equals(semanticModel.GetSymbolSafe(postfix.Operand, cancellationToken), symbol);
            }

            throw new InvalidOperationException($"Not handling {mutation}");
        }
    }
}
