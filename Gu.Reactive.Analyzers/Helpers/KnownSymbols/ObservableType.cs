namespace Gu.Reactive.Analyzers
{
    internal class ObservableType : QualifiedType
    {
        internal readonly QualifiedMethod Merge;
        internal readonly QualifiedMethod FromEvent;

        internal ObservableType()
            : base("System.Reactive.Linq.Observable")
        {
            this.Merge = new QualifiedMethod(this, nameof(this.Merge));
            this.FromEvent = new QualifiedMethod(this, nameof(this.FromEvent));
        }
    }
}