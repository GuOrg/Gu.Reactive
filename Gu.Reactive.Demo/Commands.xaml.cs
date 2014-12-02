using System.Windows.Controls;

namespace Gu.Reactive.Demo
{
    /// <summary>
    /// Interaction logic for Commands.xaml
    /// </summary>
    public partial class Commands : UserControl
    {
        public Commands()
        {
            InitializeComponent();
            DataContext = new CommandsViewModel();
        }
    }
}
