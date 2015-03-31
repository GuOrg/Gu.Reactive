namespace Gu.Reactive
{
    using System.ComponentModel;

    public class TrackingEventArgs<TProperty> : PropertyChangedEventArgs
    {
        public TrackingEventArgs(string propertyName, TProperty currentValue, TProperty previousValue)
            : base(propertyName)
        {
            CurrentValue = currentValue;
            PreviousValue = previousValue;
        }

        public TProperty CurrentValue { get; private set; }

        public TProperty PreviousValue { get; private set; }
    }
}