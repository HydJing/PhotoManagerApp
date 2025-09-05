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
        private string? selectedDirectory;

        [ObservableProperty]
        private string statusMessage = "Select a folder to load photos.";

        [ObservableProperty]
        private ObservableCollection<Photo> photos = new();

        public IRelayCommand LoadPhotosCommand { get; }

        public PhotoGalleryViewModel()
        {
            LoadPhotosCommand = new RelayCommand(ChooseAndLoadPhotos);
        }

        private void ChooseAndLoadPhotos()
        {
            System.Diagnostics.Debug.WriteLine("Browse button clicked!");

            var dialog = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = "Select a folder containing photos"
            };

            var result = dialog.ShowDialog(System.Windows.Application.Current.MainWindow);

            if (result == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Ok)
            {
                SelectedDirectory = dialog.FileName;
                LoadPhotos(SelectedDirectory);
            }
        }

        private void LoadPhotos(string directory)
        {
            if (!Directory.Exists(directory))
            {
                StatusMessage = "Directory does not exist.";
                return;
            }

            Photos.Clear();

            foreach (var file in Directory.EnumerateFiles(directory, "*.*")
                         .Where(f => f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                     f.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                                     f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)))
            {
                Photos.Add(new Photo { FilePath = file });
            }

            StatusMessage = Photos.Count > 0
                ? $"Loaded {Photos.Count} photos from {directory}"
                : "No photos found in the selected folder.";
        }
    }
}
