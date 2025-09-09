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
using System.Windows.Data;
using PhotoManager.App.Services;
using System.Collections.Generic;
using PhotoManager.App.Helpers;


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

        private readonly FavoriteService _favoriteService;
        private readonly HashSet<string> _favorites;


        public PhotoGalleryViewModel()
        {
            Photos = new ObservableCollection<Photo>();
            LoadPhotosCommand = new RelayCommand(ChooseAndLoadPhotos);
            DeletePhotosCommand = new RelayCommand(DeleteSelectedPhotos, () => Photos.Any(p => p.IsSelected));

            // react to add/remove so we can hook IsSelected changes
            Photos.CollectionChanged += Photos_CollectionChanged;

            PhotosView = CollectionViewSource.GetDefaultView(Photos);
            PhotosView.Filter = FilterPhotos;

            _favoriteService = new FavoriteService();
            _favorites = _favoriteService.LoadFavorites();

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

            if (sender is not Photo photo) return;

            if (e.PropertyName == nameof(Photo.IsFavorite))
            {
                if (photo.IsFavorite)
                    _favorites.Add(photo.FilePath);
                else
                    _favorites.Remove(photo.FilePath);

                _favoriteService.SaveFavorites(_favorites);
                PhotosView.Refresh();
            }

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
                var supportedExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };

                var files = Directory.EnumerateFiles(directory, "*.*", System.IO.SearchOption.AllDirectories)
                    .Where(f => supportedExtensions.Contains(Path.GetExtension(f).ToLower()));

                await Task.Run(() =>
                {
                    foreach (var file in files)
                    {
                        var photo = new Photo
                        {
                            FilePath = file,
                            FileName = Path.GetFileName(file),
                            IsFavorite = _favorites.Contains(file)
                        };
                        photo.LoadThumbnail(); // load into memory, release file handle
                        photo.PropertyChanged += Photo_PropertyChanged;

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
                        RecycleBinHelper.MoveToRecycleBin(photo.FilePath);
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

        private void LoadPhotosFromDirectory(string folderPath)
        {
            // When loading, check if each photo path is in favorites
            foreach (var file in Directory.EnumerateFiles(folderPath, "*.*", SearchOption.AllDirectories))
            {
                var photo = new Photo
                {
                    FilePath = file,
                    FileName = Path.GetFileName(file),
                    // ... set FileSize, Dimensions, Thumbnail
                    IsFavorite = _favorites.Contains(file)   // Restore favorite state
                };

                photo.PropertyChanged += Photo_PropertyChanged;
                Photos.Add(photo);
            }
        }

    }
}
