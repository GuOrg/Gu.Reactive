namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;

    using Gu.Reactive;

    /// <summary>
    /// A control for displaying conditions.
    /// </summary>
    public partial class ConditionControl : Control
    {
        /// <summary>Identifies the <see cref="Condition"/> dependency property.</summary>
        public static readonly DependencyProperty ConditionProperty = DependencyProperty.Register(
            nameof(Condition),
            typeof(ICondition),
            typeof(ConditionControl),
            new PropertyMetadata(default(ICondition), OnConditionChanged));

        private static readonly DependencyPropertyKey RootPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(Root),
            typeof(IEnumerable<ICondition>),
            typeof(ConditionControl),
            new PropertyMetadata(Array.Empty<ICondition>()));

        /// <summary>Identifies the <see cref="Root"/> dependency property.</summary>
        public static readonly DependencyProperty RootProperty = RootPropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey FlattenedPrerequisitesPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(FlattenedPrerequisites),
            typeof(IEnumerable<ICondition>),
            typeof(ConditionControl),
            new PropertyMetadata(Array.Empty<ICondition>()));

        private static readonly DependencyPropertyKey IsInSyncPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(IsInSync),
            typeof(bool),
            typeof(ConditionControl),
            new PropertyMetadata(defaultValue: true));

        /// <summary>Identifies the <see cref="IsInSync"/> dependency property.</summary>
        public static readonly DependencyProperty IsInSyncProperty = IsInSyncPropertyKey.DependencyProperty;

        /// <summary>Identifies the <see cref="FlattenedPrerequisites"/> dependency property.</summary>
        public static readonly DependencyProperty FlattenedPrerequisitesProperty = FlattenedPrerequisitesPropertyKey.DependencyProperty;

        static ConditionControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ConditionControl), new FrameworkPropertyMetadata(typeof(ConditionControl)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionControl"/> class.
        /// </summary>
        public ConditionControl()
        {
            this.IsVisibleChanged += (_, __) => this.OnIsVisibleChanged();
        }

        /// <summary>
        /// The condition exposed as an enumerable with one element for binding the root of <see cref="TreeView"/>.
        /// </summary>
        public IEnumerable<ICondition> Root
        {
            get => (IEnumerable<ICondition>)this.GetValue(RootProperty);
            protected set => this.SetValue(RootPropertyKey, value);
        }

        /// <summary>
        /// A flat list of all conditions.
        /// </summary>
        public IEnumerable<ICondition> FlattenedPrerequisites
        {
            get => (IEnumerable<ICondition>)this.GetValue(FlattenedPrerequisitesProperty);
            protected set => this.SetValue(FlattenedPrerequisitesPropertyKey, value);
        }

        /// <summary>
        /// True if all detected changes of ICondition.IsSatisfied have been notified.
        /// </summary>
        public bool IsInSync
        {
            get => (bool)this.GetValue(IsInSyncProperty);
            protected set => this.SetValue(IsInSyncPropertyKey, value);
        }

        /// <summary>
        /// The condition.
        /// </summary>
        public ICondition Condition
        {
            get => (ICondition)this.GetValue(ConditionProperty);
            set => this.SetValue(ConditionProperty, value);
        }

        /// <summary>This method is invoked when the <see cref="ConditionProperty"/> changes.</summary>
        /// <param name="oldCondition">The old value of <see cref="ConditionProperty"/>.</param>
        /// <param name="newCondition">The new value of <see cref="ConditionProperty"/>.</param>
        // ReSharper disable once UnusedParameter.Global
        protected virtual void OnConditionChanged(ICondition oldCondition, ICondition newCondition)
        {
            if (newCondition is null)
            {
                this.Root = Array.Empty<ICondition>();
                this.FlattenedPrerequisites = Array.Empty<ICondition>();
                return;
            }

            this.Root = new[] { newCondition };
            this.FlattenedPrerequisites = FlattenPrerequisites(newCondition);
            this.IsInSync = this.Condition.IsInSync();
        }

        private static void OnConditionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var conditionControl = (ConditionControl)o;
            conditionControl.OnConditionChanged((ICondition)e.OldValue, (ICondition)e.NewValue);
        }

        private static IReadOnlyList<ICondition> FlattenPrerequisites(ICondition condition, List<ICondition>? list = null)
        {
            if (list is null)
            {
                list = new List<ICondition>();
            }

            if (list.Contains(condition))
            {
                return list; // Break recursion
            }

            list.Add(condition);
            foreach (var pre in condition.Prerequisites)
            {
                _ = FlattenPrerequisites(pre, list);
            }

            return list;
        }

        private void OnIsVisibleChanged()
        {
            if (this.Condition != null &&
                this.Visibility == Visibility.Visible &&
                this.IsInSync)
            {
                this.IsInSync = this.Condition.IsInSync();
            }
        }
    }
}
