// MainWindow.xaml.cs
using System.Windows;
using PhotoManager.App.ViewModels;

namespace PhotoManager.App
{
    public partial class MainWindow : Window
    {
        // DI will call this constructor and supply the MainWindowViewModel
        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            // Debug: show what DataContext is
            Loaded += (s, e) =>
            {
                if (DataContext is MainWindowViewModel vm)
                {
                    MessageBox.Show($"DataContext OK. Sidebar Menu Count: {vm.Sidebar.MenuItems.Count}");
                }
                else
                {
                    MessageBox.Show("DataContext is NULL!");
                }
            };
        }
    }
}
