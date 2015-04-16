namespace Gu.Reactive.Demo
{
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for FilteredViewView.xaml
    /// </summary>
    public partial class FilteredViewView : UserControl
    {
        public FilteredViewView()
        {
            InitializeComponent();
            DataContext = new FilteredViewViewModel();
        }
    }
}
