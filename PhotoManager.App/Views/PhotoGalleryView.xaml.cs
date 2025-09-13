using System.Windows;
using System.Windows.Controls;
using PhotoManager.App.Models;
using PhotoManager.App.ViewModels;

namespace PhotoManager.App.Views
{
    public partial class PhotoGalleryView : UserControl
    {
        public PhotoGalleryView()
        {
            InitializeComponent();
            DataContext = new PhotoGalleryViewModel();
        }

        private void FavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Photo photo)
            {
                photo.IsFavorite = !photo.IsFavorite;
            }
        }
    }
}