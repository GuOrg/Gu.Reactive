namespace Gu.Reactive.Demo
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for DataGridAndEventsView.xaml.
    /// </summary>
    public sealed partial class DataGridAndEventsView : UserControl, IDisposable
    {
        /// <summary>Identifies the <see cref="Source"/> dependency property.</summary>
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            nameof(Source),
            typeof(IEnumerable),
            typeof(DataGridAndEventsView),
            new PropertyMetadata(
                default(IEnumerable),
                OnSourceChanged,
                CoerceSource));

        /// <summary>Identifies the <see cref="Changes"/> dependency property.</summary>
        public static readonly DependencyProperty ChangesProperty = DependencyProperty.Register(
            nameof(Changes),
            typeof(ObservableCollection<NotifyCollectionChangedEventArgs>),
            typeof(DataGridAndEventsView),
            new PropertyMetadata(default(ObservableCollection<NotifyCollectionChangedEventArgs>)));

        /// <summary>Identifies the <see cref="Header"/> dependency property.</summary>
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            nameof(Header),
            typeof(string),
            typeof(DataGridAndEventsView),
            new PropertyMetadata(default(string)));

        private readonly SerialDisposable disposable = new SerialDisposable();
        private bool disposed;

        public DataGridAndEventsView()
        {
            this.InitializeComponent();
            this.Changes = new ObservableCollection<NotifyCollectionChangedEventArgs>();
        }

        public IEnumerable? Source
        {
            get => (IEnumerable?)this.GetValue(SourceProperty);
            set => this.SetValue(SourceProperty, value);
        }

#pragma warning disable CA2227 // Collection properties should be read only
        public ObservableCollection<NotifyCollectionChangedEventArgs>? Changes
#pragma warning restore CA2227 // Collection properties should be read only
        {
            get => (ObservableCollection<NotifyCollectionChangedEventArgs>?)this.GetValue(ChangesProperty);
            set => this.SetValue(ChangesProperty, value);
        }

        public string? Header
        {
            get => (string?)this.GetValue(HeaderProperty);
            set => this.SetValue(HeaderProperty, value);
        }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.disposable.Dispose();
        }

        private static object CoerceSource(DependencyObject d, object? baseValue)
        {
            ((DataGridAndEventsView)d).Changes?.Clear();
            return baseValue;
        }

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dataGridAndEventsView = (DataGridAndEventsView)d;
            if (e.NewValue is INotifyCollectionChanged notifyCollectionChanged)
            {
                dataGridAndEventsView.disposable.Disposable = notifyCollectionChanged
                                                              .ObserveCollectionChangedSlim(signalInitial: false)
                                                              .ObserveOnDispatcher()
                                                              .Subscribe(x => dataGridAndEventsView.Changes.Add(x));
            }
            else
            {
                dataGridAndEventsView.disposable.Disposable = null;
            }
        }
    }
}
