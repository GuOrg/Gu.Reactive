namespace Gu.Reactive
{
    public class PropertyChangedTrackingEventArgs<TSource, TProperty> : PropertyChangedEventArgs<TSource, TProperty>
    {
        public PropertyChangedTrackingEventArgs(TSource sender, TProperty currentValue, TProperty previousValue, string propertyName)
            : base(sender, currentValue, propertyName)
        {
            this.PreviousValue = previousValue;
        }
        public TProperty PreviousValue { get; private set; }
    }
}