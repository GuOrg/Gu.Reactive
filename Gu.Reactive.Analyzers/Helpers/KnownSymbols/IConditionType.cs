namespace Gu.Reactive.Analyzers
{
    using Gu.Roslyn.AnalyzerExtensions;

    internal class IConditionType : QualifiedType
    {
        internal readonly QualifiedMethod Negate;

        public IConditionType()
            : base("Gu.Reactive.ICondition")
        {
            this.Negate = new QualifiedMethod(this, nameof(this.Negate));
        }
    }
}
