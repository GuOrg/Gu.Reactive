namespace Gu.Reactive.Analyzers
{
    using Gu.Roslyn.AnalyzerExtensions;

    internal class ObservableType : QualifiedType
    {
        internal readonly QualifiedMethod Merge;
        internal readonly QualifiedMethod FromEvent;
        internal readonly QualifiedMethod Interval;

        internal ObservableType()
            : base("System.Reactive.Linq.Observable")
        {
            this.Merge = new QualifiedMethod(this, nameof(this.Merge));
            this.FromEvent = new QualifiedMethod(this, nameof(this.FromEvent));
            this.Interval = new QualifiedMethod(this, nameof(this.Interval));
        }
    }
}
