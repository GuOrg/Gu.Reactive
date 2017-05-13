namespace Gu.Reactive.Analyzers
{
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