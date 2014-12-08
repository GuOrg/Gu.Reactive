namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// Typed CollectionView
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICollectionView<T> : ICollectionView, IEnumerable<T>, IDisposable
    {
        Predicate<T> Filter { get; set; }
    }
}