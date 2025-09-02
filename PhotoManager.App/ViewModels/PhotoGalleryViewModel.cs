using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PhotoManager.App.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace PhotoManager.App.ViewModels
{
    public partial class PhotoGalleryViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Photo> photos = new();  // initialized (never null)

        [ObservableProperty]
        private string statusMessage = string.Empty;         // initialized (never null)

        [RelayCommand]
        private async Task LoadPhotosAsync(string directoryPath)
        {
            if (string.IsNullOrWhiteSpace(directoryPath) || !Directory.Exists(directoryPath))
            {
                StatusMessage = "Directory not found.";
                return;
            }

            StatusMessage = "Loading photos...";
            Photos.Clear();

            var files = Directory.GetFiles(directoryPath, "*.*", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                if (file.EndsWith(".jpg", System.StringComparison.OrdinalIgnoreCase) ||
                    file.EndsWith(".jpeg", System.StringComparison.OrdinalIgnoreCase) ||
                    file.EndsWith(".png", System.StringComparison.OrdinalIgnoreCase) ||
                    file.EndsWith(".bmp", System.StringComparison.OrdinalIgnoreCase))
                {
                    var photo = await Task.Run(() => new Photo { FilePath = file });
                    Photos.Add(photo);
                }
            }

            StatusMessage = $"Loaded {Photos.Count} photos.";
        }
    }
}
