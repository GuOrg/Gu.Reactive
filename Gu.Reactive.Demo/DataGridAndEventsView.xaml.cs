using System.Windows.Controls;

namespace Gu.Reactive.Demo
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Reactive.Linq;
    using System.Windows;

    /// <summary>
    /// Interaction logic for DataGridAndEventsView.xaml
    /// </summary>
    public partial class DataGridAndEventsView : UserControl
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            "Source", 
            typeof(IEnumerable),
            typeof(DataGridAndEventsView), 
            new PropertyMetadata(default(IEnumerable), OnSourceChanged));

        public static readonly DependencyProperty ChangesProperty = DependencyProperty.Register(
            "Changes", 
            typeof(ObservableCollection<NotifyCollectionChangedEventArgs>),
            typeof(DataGridAndEventsView),
            new PropertyMetadata(default(ObservableCollection<NotifyCollectionChangedEventArgs>)));

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            "Header",
            typeof(string),
            typeof(DataGridAndEventsView), 
            new PropertyMetadata(default(string)));


        public DataGridAndEventsView()
        {
            InitializeComponent();
            Changes = new ObservableCollection<NotifyCollectionChangedEventArgs>();
        }

        public IEnumerable Source
        {
            get { return (IEnumerable)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public ObservableCollection<NotifyCollectionChangedEventArgs> Changes
        {
            get { return (ObservableCollection<NotifyCollectionChangedEventArgs>)GetValue(ChangesProperty); }
            set { SetValue(ChangesProperty, value); }
        }

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dataGridAndEventsView = (DataGridAndEventsView)d;
            var notifyCollectionChanged = e.NewValue as INotifyCollectionChanged;
            notifyCollectionChanged.ObserveCollectionChangedSlim(false)
                                   .ObserveOnDispatcher()
                                   .Subscribe(x => dataGridAndEventsView.Changes.Add(x));
        }
    }
}
