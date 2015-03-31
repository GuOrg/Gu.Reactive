namespace Gu.Reactive
{
    public interface IValuePath<in TSource,out TValue>
    {
        IMaybe<TValue> Value(TSource source);
    }
}