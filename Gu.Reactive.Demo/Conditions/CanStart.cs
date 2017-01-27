namespace Gu.Reactive.Demo.Conditions
{
    public class CanStart : AndCondition
    {
        public CanStart()
            : base(new IsAnyDoorOpen().Negate(), new HasFuel(), new IsMotorRunning().Negate())
        {
        }
    }
}