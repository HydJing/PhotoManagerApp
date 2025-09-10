using CommunityToolkit.Mvvm.ComponentModel;

namespace PhotoManager.App.ViewModels
{
    public partial class StatusBarViewModel : ObservableObject
    {
        private readonly PhotoGalleryViewModel _photoGallery;

        [ObservableProperty] private string statusMessage;

        public StatusBarViewModel(PhotoGalleryViewModel photoGallery)
        {
            _photoGallery = photoGallery;
            StatusMessage = _photoGallery.StatusMessage;

            _photoGallery.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(PhotoGalleryViewModel.StatusMessage))
                    StatusMessage = _photoGallery.StatusMessage;
            };
        }
    }
}
