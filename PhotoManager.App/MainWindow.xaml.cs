using PhotoManager.App.ViewModels;
using System.Windows;

namespace PhotoManager.App
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }
    }
}
