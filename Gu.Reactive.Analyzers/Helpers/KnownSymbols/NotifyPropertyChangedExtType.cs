namespace Gu.Reactive.Analyzers
{
    using Gu.Roslyn.AnalyzerExtensions;

    internal class NotifyPropertyChangedExtType : QualifiedType
    {
        internal readonly QualifiedMethod ObservePropertyChanged;
        internal readonly QualifiedMethod ObservePropertyChangedSlim;
        internal readonly QualifiedMethod ObserveFullPropertyPathSlim;

        internal NotifyPropertyChangedExtType()
            : base("Gu.Reactive.NotifyPropertyChangedExt")
        {
            this.ObservePropertyChanged = new QualifiedMethod(this, nameof(this.ObservePropertyChanged));
            this.ObservePropertyChangedSlim = new QualifiedMethod(this, nameof(this.ObservePropertyChangedSlim));
            this.ObserveFullPropertyPathSlim = new QualifiedMethod(this, nameof(this.ObserveFullPropertyPathSlim));
        }
    }
}
