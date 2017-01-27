namespace Gu.Reactive.Demo.Conditions
{
    public class IsRightDoorOpen : Condition
    {
        public IsRightDoorOpen()
            : base(
                ConditionState.Instance.ObservePropertyChanged(x => x.IsRightDoorOpen),
                () => ConditionState.Instance.IsRightDoorOpen)
        {
        }
    }
}