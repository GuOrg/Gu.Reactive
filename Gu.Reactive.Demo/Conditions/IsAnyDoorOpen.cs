namespace Gu.Reactive.Demo.Conditions
{
    public class IsAnyDoorOpen : OrCondition
    {
        public IsAnyDoorOpen()
            : base(new IsLeftDoorOpen(), new IsRightDoorOpen(), new IsBackDoorOpen())
        {
        }
    }
}