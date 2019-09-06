namespace Gu.Reactive.Analyzers
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal sealed class InvocationExecutionWalker : ExecutionWalker<InvocationExecutionWalker>, IReadOnlyList<InvocationExpressionSyntax>
    {
        private readonly List<InvocationExpressionSyntax> invocations = new List<InvocationExpressionSyntax>();

        public int Count => this.invocations.Count;

        internal IReadOnlyList<InvocationExpressionSyntax> Invocations => this.invocations;

        public InvocationExpressionSyntax this[int index] => this.invocations[index];

        public IEnumerator<InvocationExpressionSyntax> GetEnumerator() => this.invocations.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)this.invocations).GetEnumerator();

        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            this.invocations.Add(node);
            base.VisitInvocationExpression(node);
        }

        internal static InvocationExecutionWalker Borrow(SyntaxNode node, SearchScope search, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            return BorrowAndVisit(node, search, semanticModel, cancellationToken, () => new InvocationExecutionWalker());
        }

        protected override void Clear()
        {
            this.invocations.Clear();
            base.Clear();
        }
    }
}
