namespace Gu.Reactive.Demo
{
    public class CanStart : AndCondition
    {
        public CanStart()
#pragma warning disable IDISP004 // Don't ignore created IDisposable.
            : base(new IsAnyDoorOpen().Negate(), new HasFuel(), new IsMotorRunning().Negate())
#pragma warning restore IDISP004 // Don't ignore created IDisposable.
        {
        }
    }
}
