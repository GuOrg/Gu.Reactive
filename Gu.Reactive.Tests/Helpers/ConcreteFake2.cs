namespace Gu.Reactive.Tests.Helpers
{
    public class ConcreteFake2 : AbstractFake, IGeneric<double>
    {
        private double value;

        public double Value
        {
            get => this.value;

            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (value == this.value)
                {
                    return;
                }

                this.value = value;
                this.OnPropertyChanged();
            }
        }
    }
}
