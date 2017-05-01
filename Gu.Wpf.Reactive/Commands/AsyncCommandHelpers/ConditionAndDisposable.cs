namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Disposables;
    using Gu.Reactive;

    internal struct ConditionAndDisposable
    {
#pragma warning disable GU0037 // Don't assign member with injected and created disposables.
#pragma warning disable GU0031 // Dispose member.
#pragma warning disable GU0035 // Implement IDisposable.
        internal readonly ICondition Condition;
        internal readonly IDisposable Disposable;
#pragma warning restore GU0035 // Implement IDisposable.
#pragma warning restore GU0037 // Don't assign member with injected and created disposables.
#pragma warning restore GU0031 // Dispose member.

        private ConditionAndDisposable(ICondition condition, IDisposable disposable)
        {
            this.Condition = condition;
            this.Disposable = disposable;
        }

        internal static ConditionAndDisposable Create(ICondition canRunCondition, IReadOnlyList<ICondition> conditions)
        {
            if (conditions == null || conditions.Count == 0)
            {
                return new ConditionAndDisposable(canRunCondition, null);
            }

            if (conditions.Count == 1)
            {
                var andCondition = new AndCondition(canRunCondition, conditions[0]);
                return new ConditionAndDisposable(andCondition, andCondition);
            }

            var inner = new AndCondition(conditions, leaveOpen: true);
            var condition = new AndCondition(canRunCondition, inner);
            return new ConditionAndDisposable(condition, new CompositeDisposable(2) { condition, inner });
        }
    }
}