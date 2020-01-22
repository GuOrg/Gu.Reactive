namespace Gu.Reactive.Demo
{
    public class SyncErrorCondition : Condition
    {
        public SyncErrorCondition()
#pragma warning disable GUREA02 // Observable and criteria must match.
            : base(
                ConditionState.Instance.ObservePropertyChangedSlim(x => x.IsMotorRunning),
                () => ConditionState.Instance.IsRightDoorOpen) // notifying for the wrong property
#pragma warning restore GUREA02 // Observable and criteria must match.
        {
        }
    }
}
