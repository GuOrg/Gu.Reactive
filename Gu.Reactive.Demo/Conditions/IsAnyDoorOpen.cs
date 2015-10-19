namespace Gu.Reactive.Demo
{
    public class IsAnyDoorOpen : OrCondition
    {
        public IsAnyDoorOpen()
            : base(new IsLeftDoorOpen(), new IsRightDoorOpen(), new IsBackDoorOpen())
        {
        }
    }
}