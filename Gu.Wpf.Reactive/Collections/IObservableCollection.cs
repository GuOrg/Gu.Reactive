namespace Gu.Wpf.Reactive
{
    using System.Collections;
    using System.Collections.Generic;

    public interface IObservableCollection<T> : IList<T>, IList, IReadonlyObservableCollection<T>
    {
    }
}
