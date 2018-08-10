namespace Gu.Reactive.Demo
{
    using System;
    using System.Linq;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for FilteredViewView.xaml.
    /// </summary>
    public partial class FilteredViewView : UserControl, IDisposable
    {
        private readonly FilteredViewViewModel vm;
        private bool disposed;

        public FilteredViewView()
        {
            this.InitializeComponent();
            this.DataContext = this.vm = new FilteredViewViewModel();
        }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.vm.Dispose();
        }

        private void OnTagsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.vm.SelectedTags = this.TagBox.SelectedItems.Cast<int>();
        }
    }
}
