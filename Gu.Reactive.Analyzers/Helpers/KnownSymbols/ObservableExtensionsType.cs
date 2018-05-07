namespace Gu.Reactive.Analyzers
{
    using Gu.Roslyn.AnalyzerExtensions;

    internal class ObservableExtensionsType : QualifiedType
    {
        internal readonly QualifiedMethod Subscribe;

        internal ObservableExtensionsType()
            : base("System.ObservableExtensions")
        {
            this.Subscribe = new QualifiedMethod(this, nameof(this.Subscribe));
        }
    }
}
