namespace Gu.Reactive
{
    using System;
    using System.Collections;

    public interface IReadonlyIListView<out T> : IReadOnlyObservableCollection<T>, IList, IDisposable
    {
    }
}