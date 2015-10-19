namespace Gu.Reactive.Demo
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