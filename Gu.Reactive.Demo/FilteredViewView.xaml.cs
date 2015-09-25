namespace Gu.Reactive.Demo
{
    using System.Linq;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for FilteredViewView.xaml
    /// </summary>
    public partial class FilteredViewView : UserControl
    {
        private readonly FilteredViewViewModel _vm;

        public FilteredViewView()
        {
            InitializeComponent();
            DataContext = _vm = new FilteredViewViewModel();
        }

        private void OnTagsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _vm.SelectedTags = TagBox.SelectedItems.Cast<int>();
        }
    }
}
