namespace Gu.Reactive.Demo
{
    public class IsBackDoorOpen : Condition
    {
        public IsBackDoorOpen()
            : base(
                ConditionState.Instance.ObservePropertyChanged(x => x.IsBackDoorOpen),
                () => ConditionState.Instance.IsBackDoorOpen)
        {
        }
    }
}