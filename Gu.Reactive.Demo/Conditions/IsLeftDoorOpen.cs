namespace Gu.Reactive.Demo
{
    public class IsLeftDoorOpen : Condition
    {
        public IsLeftDoorOpen()
            : base(
                ConditionState.Instance.ObservePropertyChanged(x => x.IsLeftDoorOpen),
                () => ConditionState.Instance.IsLeftDoorOpen)
        {
        }
    }
}