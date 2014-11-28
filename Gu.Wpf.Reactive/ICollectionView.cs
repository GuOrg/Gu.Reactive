namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;

    /// <summary>
    /// Typed ColelctionView
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICollectionView<T> : ICollectionView, IEnumerable<T>, IWeakEventListener
    {
        Predicate<T> Filter { get; set; }
    }
}