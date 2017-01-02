namespace Gu.Reactive.Demo
{
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for Commands.xaml
    /// </summary>
    public partial class CommandsView : UserControl
    {
        public CommandsView()
        {
            this.InitializeComponent();
            this.DataContext = new CommandsViewModel();
        }
    }
}
