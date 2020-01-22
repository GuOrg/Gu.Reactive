namespace Gu.Reactive.Demo
{
    public class IsRightDoorOpen : Condition
    {
        public IsRightDoorOpen()
            : base(
                ConditionState.Instance.ObservePropertyChangedSlim(x => x.IsRightDoorOpen),
                () => ConditionState.Instance.IsRightDoorOpen)
        {
        }
    }
}
