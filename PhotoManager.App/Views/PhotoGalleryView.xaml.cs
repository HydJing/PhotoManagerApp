using System.Windows.Controls;
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
    }
}