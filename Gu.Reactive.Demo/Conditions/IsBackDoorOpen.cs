namespace Gu.Reactive.Demo
{
    public class IsBackDoorOpen : Condition
    {
        public IsBackDoorOpen()
            : base(
                ConditionState.Instance.ObservePropertyChangedSlim(x => x.IsBackDoorOpen),
                () => ConditionState.Instance.IsBackDoorOpen)
        {
        }
    }
}
