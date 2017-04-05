namespace Gu.Reactive.Tests.Helpers
{
    public class WithShadowing<T> : With<T>
    {
        private T value;

        public new T Value
        {
            get => this.value;

            set
            {
                if (System.Collections.Generic.EqualityComparer<T>.Default.Equals(value, this.value))
                {
                    return;
                }

                this.value = value;
                this.OnPropertyChanged();
            }
        }
    }
}