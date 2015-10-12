namespace Gu.Wpf.Reactive
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;

    using Gu.Reactive;

    /// <summary>
    /// A control for displaying conditions
    /// </summary>
    public class ConditionControl : Control
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

        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
            "ItemTemplate",
            typeof(DataTemplate),
            typeof(ConditionControl),
            new FrameworkPropertyMetadata(
                default(DataTemplate),
                FrameworkPropertyMetadataOptions.Inherits));

        public static readonly DependencyProperty FlattenedPrerequisitesProperty = FlattenedPrerequisitesPropertyKey.DependencyProperty;

        static ConditionControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ConditionControl), new FrameworkPropertyMetadata(typeof(ConditionControl)));
        }

        public ConditionControl()
        {
            this.IsVisibleChanged += OnIsVisibleChanged;
        }

        public ICondition Condition
        {
            get
            {
                return (ICondition)GetValue(ConditionProperty);
            }
            set
            {
                SetValue(ConditionProperty, value);
            }
        }

        public IEnumerable<ICondition> Root
        {
            get { return (IEnumerable<ICondition>)GetValue(RootProperty); }
            protected set { SetValue(RootPropertyKey, value); }
        }

        /// <summary>
        /// A flat list of all conditions
        /// </summary>
        public IEnumerable<ICondition> FlattenedPrerequisites
        {
            get
            {
                return (IEnumerable<ICondition>)GetValue(FlattenedPrerequisitesProperty);
            }
            protected set
            {
                SetValue(FlattenedPrerequisitesPropertyKey, value);
            }
        }

        public bool IsInSync
        {
            get { return (bool)GetValue(IsInSyncProperty); }
            protected set { SetValue(IsInSyncPropertyKey, value); }
        }

        public DataTemplate ItemTemplate
        {
            get
            {
                return (DataTemplate)GetValue(ItemTemplateProperty);
            }
            set
            {
                SetValue(ItemTemplateProperty, value);
            }
        }

        protected virtual void OnConditionChanged(ICondition oldCondition, ICondition newCondition)
        {
            if (newCondition == null)
            {
                Root = Empty;
                FlattenedPrerequisites = Empty;
                return;
            }

            Root = new[] { newCondition };
            FlattenedPrerequisites = FlattenPrerequisites(newCondition);
            IsInSync = Condition.IsInSync();
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

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (Condition != null &&
                Visibility == Visibility.Visible &&
                IsInSync)
            {
                IsInSync = Condition.IsInSync();
            }
        }
    }
}