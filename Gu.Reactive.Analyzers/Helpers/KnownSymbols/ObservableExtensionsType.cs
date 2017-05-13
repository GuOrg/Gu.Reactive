namespace Gu.Reactive.Analyzers
{
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