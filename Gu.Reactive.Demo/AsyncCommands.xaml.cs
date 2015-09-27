using System.Windows.Controls;

namespace Gu.Reactive.Demo
{
    /// <summary>
    /// Interaction logic for AsyncCommands.xaml
    /// </summary>
    public partial class AsyncCommands : UserControl
    {
        public AsyncCommands()
        {
            InitializeComponent();
            DataContext = new AsyncCommandsViewModel();
        }
    }
}
