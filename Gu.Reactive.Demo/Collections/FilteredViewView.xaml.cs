namespace Gu.Reactive.Demo
{
    using System.Linq;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for FilteredViewView.xaml
    /// </summary>
    public partial class FilteredViewView : UserControl
    {
        private readonly FilteredViewViewModel vm;

        public FilteredViewView()
        {
            this.InitializeComponent();
            this.DataContext = this.vm = new FilteredViewViewModel();
        }

        private void OnTagsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.vm.SelectedTags = this.TagBox.SelectedItems.Cast<int>();
        }
    }
}
