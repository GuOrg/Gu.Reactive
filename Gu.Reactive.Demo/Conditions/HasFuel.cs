namespace Gu.Reactive.Demo
{
    public class HasFuel : Condition
    {
        public HasFuel()
            : base(
                ConditionState.Instance.ObservePropertyChangedSlim(x => x.FuelLevel),
                () => ConditionState.Instance.FuelLevel > 0)
        {
        }
    }
}
