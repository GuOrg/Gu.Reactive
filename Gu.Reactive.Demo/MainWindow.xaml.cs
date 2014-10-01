using System.Windows;

namespace Gu.Reactive.Demo
{
    using System.Windows.Documents;

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

        private void StartButton_OnLoaded(object sender, RoutedEventArgs e)
        {
            var myAdornerLayer = AdornerLayer.GetAdornerLayer(StartButton);
            myAdornerLayer.Add(new DisabledInfoAdorner(StartButton));
        }
    }
}
