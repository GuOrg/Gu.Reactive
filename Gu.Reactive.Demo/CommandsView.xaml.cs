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
#pragma warning disable GU0032 // Dispose before re-assigning.
            this.DataContext = new CommandsViewModel();
#pragma warning restore GU0032 // Dispose before re-assigning.
        }
    }
}
