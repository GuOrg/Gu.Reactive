namespace Gu.Reactive.Demo
{
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for NinjaBindingDemo.xaml
    /// </summary>
    public partial class NinjaBindingDemo : UserControl
    {
        public NinjaBindingDemo()
        {
            InitializeComponent();
            DataContext = new NinjaBindingViewModel();
        }
    }
}
