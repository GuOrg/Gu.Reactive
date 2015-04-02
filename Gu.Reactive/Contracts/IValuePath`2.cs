namespace Gu.Reactive
{
    public interface IValuePath<in TSource,out TValue>
    {
        IMaybe<TValue> GetValue(TSource source);
    }
}