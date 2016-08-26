﻿namespace Gu.Wpf.Reactive
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;

    using Gu.Reactive;

    /// <summary>
    /// A control for displaying conditions
    /// </summary>
    public partial class ConditionControl : Control
    {
        private static IEnumerable<ICondition> Empty = new ICondition[0];

        public static readonly DependencyProperty ConditionProperty = DependencyProperty.Register(
            "Condition",
            typeof(ICondition),
            typeof(ConditionControl),
            new PropertyMetadata(default(ICondition), OnConditionChanged));

        private static readonly DependencyPropertyKey RootPropertyKey = DependencyProperty.RegisterReadOnly(
            "Root",
            typeof(IEnumerable<ICondition>),
            typeof(ConditionControl),
            new PropertyMetadata(Empty));

        public static readonly DependencyProperty RootProperty = RootPropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey FlattenedPrerequisitesPropertyKey = DependencyProperty.RegisterReadOnly(
            "FlattenedPrerequisites",
            typeof(IEnumerable<ICondition>),
            typeof(ConditionControl),
            new PropertyMetadata(Empty));

        private static readonly DependencyPropertyKey IsInSyncPropertyKey = DependencyProperty.RegisterReadOnly(
            "IsInSync",
            typeof(bool),
            typeof(ConditionControl),
            new PropertyMetadata(true));

        public static readonly DependencyProperty IsInSyncProperty = IsInSyncPropertyKey.DependencyProperty;

        public static readonly DependencyProperty FlattenedPrerequisitesProperty = FlattenedPrerequisitesPropertyKey.DependencyProperty;

        static ConditionControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ConditionControl), new FrameworkPropertyMetadata(typeof(ConditionControl)));
        }

        public ConditionControl()
        {
            this.IsVisibleChanged += (_, __) => this.OnIsVisibleChanged();
        }

        public ICondition Condition
        {
            get { return (ICondition)this.GetValue(ConditionProperty); }
            set { this.SetValue(ConditionProperty, value); }
        }

        public IEnumerable<ICondition> Root
        {
            get { return (IEnumerable<ICondition>)this.GetValue(RootProperty); }
            protected set { this.SetValue(RootPropertyKey, value); }
        }

        /// <summary>
        /// A flat list of all conditions
        /// </summary>
        public IEnumerable<ICondition> FlattenedPrerequisites
        {
            get { return (IEnumerable<ICondition>)this.GetValue(FlattenedPrerequisitesProperty); }
            protected set { this.SetValue(FlattenedPrerequisitesPropertyKey, value); }
        }

        public bool IsInSync
        {
            get { return (bool)this.GetValue(IsInSyncProperty); }
            protected set { this.SetValue(IsInSyncPropertyKey, value); }
        }

        protected virtual void OnConditionChanged(ICondition oldCondition, ICondition newCondition)
        {
            if (newCondition == null)
            {
                this.Root = Empty;
                this.FlattenedPrerequisites = Empty;
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

        private static IReadOnlyList<ICondition> FlattenPrerequisites(ICondition condition, List<ICondition> list = null)
        {
            if (list == null)
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
                FlattenPrerequisites(pre, list);
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