using System.Windows.Controls;
using PhotoManager.App.ViewModels;

namespace PhotoManager.App.Views
{
    public partial class SidebarView : UserControl
    {
        public SidebarView()
        {
            InitializeComponent();
            DataContext = new SidebarViewModel(new PhotoGalleryViewModel());
        }
    }
}