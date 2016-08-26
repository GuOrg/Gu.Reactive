using System.Windows.Controls;

namespace Gu.Reactive.Demo
{
    /// <summary>
    /// Interaction logic for AsyncCommands.xaml
    /// </summary>
    public partial class AsyncCommandsView : UserControl
    {
        public AsyncCommandsView()
        {
            InitializeComponent();
            DataContext = new AsyncCommandsViewModel();
        }
    }
}
