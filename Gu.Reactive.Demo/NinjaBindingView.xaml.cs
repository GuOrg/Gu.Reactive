namespace Gu.Reactive.Demo
{
    using System.Windows.Controls;

    public partial class NinjaBindingView : UserControl
    {
        public NinjaBindingView()
        {
            this.InitializeComponent();
            this.DataContext = new NinjaBindingViewModel();
        }
    }
}
