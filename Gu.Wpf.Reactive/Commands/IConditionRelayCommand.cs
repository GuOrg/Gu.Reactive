namespace Gu.Wpf.Reactive
{
    using System;

    using Gu.Reactive;

    public interface IConditionRelayCommand : IToolTipCommand, IDisposable
    {
        ICondition Condition { get; }
    }
}