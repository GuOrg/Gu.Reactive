namespace Gu.Reactive.Demo.Conditions
{
    public class SyncErrorCondition : Condition
    {
        public SyncErrorCondition()
            : base(
                ConditionState.Instance.ObservePropertyChanged(x => x.IsMotorRunning),
                () => ConditionState.Instance.IsRightDoorOpen) // notifying for the wrong property
        {
        }
    }
}