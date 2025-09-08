using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PhotoManager.App.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Windows.Input;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.VisualBasic.FileIO;
using System.Windows.Data;


namespace PhotoManager.App.ViewModels
{
    public partial class PhotoGalleryViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? selectedDirectory;

        [ObservableProperty]
        private string statusMessage = "Select a folder to load photos.";

        [ObservableProperty]
        public ObservableCollection<Photo> photos = new();

        public IRelayCommand LoadPhotosCommand { get; }
        public IRelayCommand DeletePhotosCommand { get; }

        public ObservableCollection<string> MenuItems { get; }
            = new ObservableCollection<string> { "Library", "Favorite" };

        [ObservableProperty]
        private string selectedMenu = "Library";  // default selected

        public ICollectionView PhotosView { get; }


        public PhotoGalleryViewModel()
        {
            Photos = new ObservableCollection<Photo>();
            LoadPhotosCommand = new RelayCommand(ChooseAndLoadPhotos);
            DeletePhotosCommand = new RelayCommand(DeleteSelectedPhotos, () => Photos.Any(p => p.IsSelected));

            // react to add/remove so we can hook IsSelected changes
            Photos.CollectionChanged += Photos_CollectionChanged;

            PhotosView = CollectionViewSource.GetDefaultView(Photos);
            PhotosView.Filter = FilterPhotos;
        }

        private bool CanDeleteSelectedPhotos() => Photos.Any(p => p.IsSelected);

        private void Photos_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (Photo p in e.NewItems)
                    p.PropertyChanged += Photo_PropertyChanged;

            if (e.OldItems != null)
                foreach (Photo p in e.OldItems)
                    p.PropertyChanged -= Photo_PropertyChanged;

            // reevaluate after collection changes
            DeletePhotosCommand.NotifyCanExecuteChanged();
        }

        private void Photo_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Photo.IsSelected))
                DeletePhotosCommand.NotifyCanExecuteChanged();

            if (e.PropertyName == nameof(Photo.IsFavorite))
                PhotosView.Refresh(); // re-run filter so Favorites view updates instantly
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

        private async void LoadPhotos(string directory)
        {
            if (!Directory.Exists(directory))
            {
                StatusMessage = "Directory does not exist.";
                return;
            }

            Photos.Clear();
            StatusMessage = "Loading photos...";

            try
            {
                var files = Directory.EnumerateFiles(directory, "*.*", System.IO.SearchOption.AllDirectories)
                    .Where(f => f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                                f.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                                f.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase) ||
                                f.EndsWith(".gif", StringComparison.OrdinalIgnoreCase));

                await Task.Run(() =>
                {
                    foreach (var file in files)
                    {
                        var photo = new Photo { FilePath = file };
                        photo.LoadThumbnail(); // load into memory, release file handle

                        // Add to collection on UI thread
                        App.Current.Dispatcher.Invoke(() => Photos.Add(photo));

                    }
                });
                

                StatusMessage = Photos.Count > 0
                    ? $"Loaded {Photos.Count} photos from {directory} (including subfolders)"
                    : "No photos found in the selected folder or subfolders.";
            }
            catch (UnauthorizedAccessException ex)
            {
                StatusMessage = $"Access denied: {ex.Message}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error while loading photos: {ex.Message}";
            }
        }

        private void DeleteSelectedPhotos()
        {
            var selected = Photos.Where(p => p.IsSelected).ToList();
            if (!selected.Any())
            {
                StatusMessage = "No photos selected for deletion.";
                return;
            }

            int deletedCount = 0;
            foreach (var photo in selected)
            {
                try
                {
                    if (File.Exists(photo.FilePath))
                    {
                        FileSystem.DeleteFile(
                            photo.FilePath,
                            UIOption.OnlyErrorDialogs,
                            RecycleOption.SendToRecycleBin);
                    }

                    Photos.Remove(photo);
                    deletedCount++;
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Error deleting {photo.FilePath}: {ex.Message}";
                }
            }

            StatusMessage = $"Moved {deletedCount} photo(s) to Recycle Bin.";
            DeletePhotosCommand.NotifyCanExecuteChanged();
        }

        partial void OnSelectedMenuChanged(string value)
        {
            PhotosView.Refresh(); // re-run filter when sidebar changes
        }

        private bool FilterPhotos(object obj)
        {
            if (obj is not Photo photo) return false;

            return SelectedMenu switch
            {
                "Favorite" => photo.IsFavorite,
                _ => true // Library shows all
            };
        }

    }
}
