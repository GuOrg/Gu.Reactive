namespace Gu.Reactive.Analyzers
{
    internal class ObservableType : QualifiedType
    {
        internal readonly QualifiedMethod Merge;

        internal ObservableType()
            : base("System.Reactive.Linq.Observable")
        {
            this.Merge = new QualifiedMethod(this, nameof(this.Merge));
        }
    }
}