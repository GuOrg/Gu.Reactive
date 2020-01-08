#pragma warning disable IDISP001 // Dispose created.
#pragma warning disable IDISP004 // Don't ignore return value of type IDisposable.
#pragma warning disable IDISP006 // Implement IDisposable.
#pragma warning disable IDISP008 // Don't assign member with injected and created disposables.
namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Disposables;
    using Gu.Reactive;

    internal struct ConditionAndDisposable
    {
#pragma warning disable IDISP002 // Dispose member.
        internal readonly ICondition Condition;
        internal readonly IDisposable Disposable;
#pragma warning restore IDISP002 // Dispose member.

        private ConditionAndDisposable(ICondition condition, IDisposable disposable)
        {
            this.Condition = condition;
            this.Disposable = disposable;
        }

        internal static ConditionAndDisposable Create(ICondition canRunCondition, IReadOnlyList<ICondition> conditions)
        {
            if (conditions is null || conditions.Count == 0)
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
