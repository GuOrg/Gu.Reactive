namespace Gu.Reactive
{
    using System.ComponentModel;

    public class PropertyChangedEventArgs<TSource, TProperty> : PropertyChangedEventArgs
    {
        public PropertyChangedEventArgs(TSource sender, TProperty currentValue, string propertyName)
            : base(propertyName)
        {
            this.Sender = sender;
            this.CurrentValue = currentValue;
        }
        public TSource Sender { get; private set; }
        public TProperty CurrentValue { get; private set; }
    }
}