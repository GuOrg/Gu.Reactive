namespace Gu.Reactive.Analyzers
{
    using Gu.Roslyn.AnalyzerExtensions;

    internal class NullableOfTType : QualifiedType
    {
        internal readonly QualifiedProperty Value;
        internal readonly QualifiedProperty HasValue;

        internal NullableOfTType()
            : base("System.Nullable`1")
        {
            this.Value = new QualifiedProperty(this, nameof(this.Value));
            this.HasValue = new QualifiedProperty(this, nameof(this.HasValue));
        }
    }
}
