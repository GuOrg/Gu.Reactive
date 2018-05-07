namespace Gu.Reactive.Analyzers
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal sealed class IdentifierNameExecutionWalker : ExecutionWalker<IdentifierNameExecutionWalker>, IReadOnlyList<IdentifierNameSyntax>
    {
        private readonly List<IdentifierNameSyntax> identifierNames = new List<IdentifierNameSyntax>();

        private IdentifierNameExecutionWalker()
        {
        }

        public IReadOnlyList<IdentifierNameSyntax> IdentifierNames => this.identifierNames;

        public int Count => this.identifierNames.Count;

        public IdentifierNameSyntax this[int index] => this.identifierNames[index];

        public static IdentifierNameExecutionWalker Create(SyntaxNode node, Scope search, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            return BorrowAndVisit(node, search, semanticModel, cancellationToken, () => new IdentifierNameExecutionWalker());
        }

        public IEnumerator<IdentifierNameSyntax> GetEnumerator() => this.identifierNames.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)this.identifierNames).GetEnumerator();

        public override void VisitIdentifierName(IdentifierNameSyntax node)
        {
            this.identifierNames.Add(node);
            base.VisitIdentifierName(node);
        }

        protected override void Clear()
        {
            this.identifierNames.Clear();
            base.Clear();
        }
    }
}
