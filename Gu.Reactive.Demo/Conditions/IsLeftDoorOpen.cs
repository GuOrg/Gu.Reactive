namespace Gu.Reactive.Demo
{
    public class IsLeftDoorOpen : Condition
    {
        public IsLeftDoorOpen()
            : base(
                ConditionState.Instance.ObservePropertyChangedSlim(x => x.IsLeftDoorOpen),
                () => ConditionState.Instance.IsLeftDoorOpen)
        {
        }
    }
}
