namespace Gu.Reactive.Tests.Helpers
{
    public class ConcreteFake1 : AbstractFake, IGeneric<int>
    {
        private int value;

        public int Value
        {
            get => this.value;

            set
            {
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
