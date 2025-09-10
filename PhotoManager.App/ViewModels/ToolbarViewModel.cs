using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PhotoManager.App.ViewModels
{
    public partial class ToolbarViewModel : ObservableObject
    {
        private readonly PhotoGalleryViewModel _photoGallery;

        public IRelayCommand LoadPhotosCommand => _photoGallery.LoadPhotosCommand;
        public IRelayCommand DeletePhotosCommand => _photoGallery.DeletePhotosCommand;

        public ToolbarViewModel(PhotoGalleryViewModel photoGallery)
        {
            _photoGallery = photoGallery;
        }
    }
}
