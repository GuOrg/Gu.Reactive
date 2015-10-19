namespace Gu.Reactive.Demo
{

    public class HasFuel : Condition
    {
        public HasFuel()
            : base(
                ConditionState.Instance.ObservePropertyChanged(x => x.FuelLevel),
                () => ConditionState.Instance.FuelLevel > 0)
        {
        }
    }
}