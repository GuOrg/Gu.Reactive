namespace Gu.Reactive.Analyzers
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal sealed class IdentifierNameWalker : ExecutionWalker, IReadOnlyList<IdentifierNameSyntax>
    {
        private static readonly Pool<IdentifierNameWalker> Cache = new Pool<IdentifierNameWalker>(
            () => new IdentifierNameWalker(),
            x =>
            {
                x.identifierNames.Clear();
                x.Clear();
            });

        private readonly List<IdentifierNameSyntax> identifierNames = new List<IdentifierNameSyntax>();

        private IdentifierNameWalker()
        {
        }

        public IReadOnlyList<IdentifierNameSyntax> IdentifierNames => this.identifierNames;

        public int Count => this.identifierNames.Count;

        public IdentifierNameSyntax this[int index] => this.identifierNames[index];

        public static Pool<IdentifierNameWalker>.Pooled Create(SyntaxNode node)
        {
            var pooled = Cache.GetOrCreate();
            pooled.Item.Visit(node);
            return pooled;
        }

        public static Pool<IdentifierNameWalker>.Pooled Create(SyntaxNode node, Search search, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            var pooled = Cache.GetOrCreate();
            pooled.Item.Search = search;
            pooled.Item.SemanticModel = semanticModel;
            pooled.Item.CancellationToken = cancellationToken;
            pooled.Item.Visit(node);
            return pooled;
        }

        public IEnumerator<IdentifierNameSyntax> GetEnumerator() => this.identifierNames.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)this.identifierNames).GetEnumerator();

        public override void Visit(SyntaxNode node)
        {
            // proper nasty inheritance right here
            this.VisitCore(node);
        }

        public override void VisitIdentifierName(IdentifierNameSyntax node)
        {
            this.identifierNames.Add(node);
            base.VisitIdentifierName(node);
        }
    }
}