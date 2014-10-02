using System.Windows;

namespace Gu.Reactive.Demo
{
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using Wpf.Reactive;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ViewModel();
        }

        private void OnAdornerButtonLoaded(object sender, RoutedEventArgs e)
        {
            var myAdornerLayer = AdornerLayer.GetAdornerLayer((ButtonBase)sender);
            var conditionInfoAdorner = new ConditionInfoAdorner((ButtonBase)sender);
            myAdornerLayer.Add(conditionInfoAdorner);
        }
    }
}
