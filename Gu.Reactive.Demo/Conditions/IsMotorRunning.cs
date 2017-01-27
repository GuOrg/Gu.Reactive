namespace Gu.Reactive.Demo.Conditions
{
    public class IsMotorRunning : Condition
    {
        public IsMotorRunning()
            : base(
                ConditionState.Instance.ObservePropertyChanged(x => x.IsMotorRunning),
                () => ConditionState.Instance.IsMotorRunning)
        {
        }
    }
}