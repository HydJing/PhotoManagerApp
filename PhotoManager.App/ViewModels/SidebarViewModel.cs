using CommunityToolkit.Mvvm.ComponentModel;
using PhotoManager.App.Models;
using System.Collections.ObjectModel;

namespace PhotoManager.App.ViewModels
{
    public partial class SidebarViewModel : ObservableObject
    {
        public ObservableCollection<string> MenuItems { get; }
            = new ObservableCollection<string> { "Library", "Favorite" };

        [ObservableProperty] private string selectedMenu = "Library";

        private readonly PhotoGalleryViewModel _photoGallery;

        public SidebarViewModel(PhotoGalleryViewModel photoGallery)
        {
            _photoGallery = photoGallery;
        }

        partial void OnSelectedMenuChanged(string value)
        {
            // Tell photo gallery to refresh filter
            _photoGallery.PhotosView.Refresh();
        }

    }
}
