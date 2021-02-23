namespace Gu.Reactive.Demo
{
    using System.Windows.Controls;

    public partial class CommandsView : UserControl
    {
        public CommandsView()
        {
            this.InitializeComponent();
            this.DataContext = new CommandsViewModel();
        }
    }
}
