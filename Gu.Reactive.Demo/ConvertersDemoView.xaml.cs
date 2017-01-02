namespace Gu.Reactive.Demo
{
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for ConvertersDemoView.xaml
    /// </summary>
    public partial class ConvertersDemoView : UserControl
    {
        public ConvertersDemoView()
        {
            this.InitializeComponent();
            this.DataContext = new ConverterDemoViewmodel();
        }
    }
}
