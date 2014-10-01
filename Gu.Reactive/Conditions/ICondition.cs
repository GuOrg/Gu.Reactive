namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public interface ICondition : IDisposable, INotifyPropertyChanged
    {
        bool? IsSatisfied { get; }
        string Name { get; }
        IEnumerable<ICondition> Prerequisites { get; }
        IEnumerable<ConditionHistoryPoint> History { get; }
        ICondition Negate();
    }
}