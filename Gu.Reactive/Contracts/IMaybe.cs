namespace Gu.Reactive
{
    public interface IMaybe<out T>
    {
        bool HasValue { get; }
        T ValueOrDefault { get; }
    }
}