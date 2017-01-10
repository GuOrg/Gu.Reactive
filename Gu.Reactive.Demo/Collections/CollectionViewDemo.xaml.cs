namespace Gu.Reactive.Demo
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// Interaction logic for CollectionViewDemo.xaml
    /// </summary>
    public sealed partial class CollectionViewDemo : UserControl, IDisposable
    {
        private readonly CollectionViewDemoViewModel viewModel = new CollectionViewDemoViewModel();
        private bool disposed;

        public CollectionViewDemo()
        {
            this.InitializeComponent();
            this.DataContext = this.viewModel;
        }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.viewModel.Dispose();
        }

        private void FilterOnClick(object sender, RoutedEventArgs e)
        {
            var isChecked = ((ToggleButton)sender).IsChecked == true;
            this.ToggleFilter(isChecked);
        }

        private async void FilterOnOtherThreadOnClick(object sender, RoutedEventArgs e)
        {
            var isChecked = ((ToggleButton)sender).IsChecked == true;
            await Task.Run(() => this.ToggleFilter(isChecked)).ConfigureAwait(false);
        }

        private void ToggleFilter(bool isChecked)
        {
            this.viewModel.Filter = isChecked
                ? (Func<int, bool>)(x => x % 2 == 0)
                : (x => true);
        }

        private void AddOnClick(object sender, RoutedEventArgs e)
        {
            this.viewModel.ObservableCollection.Add(this.viewModel.ObservableCollection.Count + 1);
        }

        private void RemoveOnClick(object sender, RoutedEventArgs e)
        {
            this.viewModel.ObservableCollection.RemoveAt(this.viewModel.ObservableCollection.Count - 1);
        }
    }
}
