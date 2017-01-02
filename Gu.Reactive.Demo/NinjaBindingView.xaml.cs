namespace Gu.Reactive.Demo
{
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for NinjaBindingDemo.xaml
    /// </summary>
    public partial class NinjaBindingView : UserControl
    {
        public NinjaBindingView()
        {
            this.InitializeComponent();
            this.DataContext = new NinjaBindingViewModel();
        }
    }
}
