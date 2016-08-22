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
    public partial class CollectionViewDemo : UserControl
    {
        private readonly CollectionViewDemoViewModel _viewModel = new CollectionViewDemoViewModel();
        public CollectionViewDemo()
        {
            InitializeComponent();
            DataContext = _viewModel;
        }

        private void FilterOnClick(object sender, RoutedEventArgs e)
        {
            var isChecked = ((ToggleButton)sender).IsChecked == true;
            ToggleFilter(isChecked);
        }

        private async void FilterOnOtherThreadOnClick(object sender, RoutedEventArgs e)
        {
            var isChecked = ((ToggleButton)sender).IsChecked == true;
            await Task.Run(() => ToggleFilter(isChecked)).ConfigureAwait(false);
        }

        private void ToggleFilter(bool isChecked)
        {
            _viewModel.Filter = isChecked
                ? (Func<int, bool>)(x => x % 2 == 0)
                : (x => true);
        }

        private void AddOnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.ObservableCollection.Add(_viewModel.ObservableCollection.Count + 1);
        }

        private void RemoveOnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.ObservableCollection.RemoveAt(_viewModel.ObservableCollection.Count - 1);
        }
    }
}
