// MainWindow.xaml.cs
using System.Windows;
using PhotoManager.App.ViewModels;

namespace PhotoManager.App
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Create a single PhotoGalleryViewModel instance to share across all child viewmodels
            var photoGalleryViewModel = new PhotoGalleryViewModel();

            DataContext = new MainWindowViewModel(
                new SidebarViewModel(photoGalleryViewModel),
                photoGalleryViewModel,
                new ToolbarViewModel(photoGalleryViewModel),
                new StatusBarViewModel(photoGalleryViewModel)
            );
        }
    }
}
