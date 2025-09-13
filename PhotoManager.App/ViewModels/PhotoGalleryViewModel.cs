using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PhotoManager.App.Models;
using PhotoManager.App.Services;
using PhotoManager.App.Helpers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PhotoManager.App.ViewModels
{
    public partial class PhotoGalleryViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Photo> photos = new();

        [ObservableProperty]
        private string selectedDirectory;

        [ObservableProperty]
        private string statusMessage = "Select a folder to load photos.";

        public ICollectionView PhotosView { get; }

        public IRelayCommand LoadPhotosCommand { get; }
        public IRelayCommand DeletePhotosCommand { get; }

        private readonly FavoriteService _favoriteService;
        private readonly HashSet<string> _favorites;

        public PhotoGalleryViewModel()
        {
            _favoriteService = new FavoriteService();
            _favorites = _favoriteService.LoadFavorites();

            PhotosView = CollectionViewSource.GetDefaultView(Photos);
            PhotosView.Filter = FilterPhotos;

            LoadPhotosCommand = new RelayCommand(ChooseAndLoadPhotos);
            DeletePhotosCommand = new RelayCommand(DeleteSelectedPhotos, CanDeleteSelectedPhotos);

            Photos.CollectionChanged += Photos_CollectionChanged;
        }


        private void Photos_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (Photo p in e.NewItems)
                    p.PropertyChanged += Photo_PropertyChanged;

            if (e.OldItems != null)
                foreach (Photo p in e.OldItems)
                    p.PropertyChanged -= Photo_PropertyChanged;

            DeletePhotosCommand.NotifyCanExecuteChanged();
        }

        private void Photo_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is not Photo photo) return;

            switch (e.PropertyName)
            {
                case nameof(Photo.IsSelected):
                    DeletePhotosCommand.NotifyCanExecuteChanged();
                    break;

                case nameof(Photo.IsFavorite):
                    if (photo.IsFavorite)
                        _favorites.Add(photo.FilePath);
                    else
                        _favorites.Remove(photo.FilePath);

                    _favoriteService.SaveFavorites(_favorites);
                    PhotosView.Refresh(); // refresh filter if showing favorites
                    break;
            }
        }

        private bool CanDeleteSelectedPhotos() => Photos.Any(p => p.IsSelected);

 

        private void ChooseAndLoadPhotos()
        {
            var dialog = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = "Select a folder containing photos"
            };

            var result = dialog.ShowDialog(System.Windows.Application.Current.MainWindow);

            if (result == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Ok)
            {
                SelectedDirectory = dialog.FileName;
                LoadPhotosAsync(SelectedDirectory);
            }
        }

        private async void LoadPhotosAsync(string directory)
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
                var files = Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories)
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

                        photo.LoadThumbnail();
                        photo.PropertyChanged += Photo_PropertyChanged;

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


        public bool FilterPhotos(object obj)
        {
            if (obj is not Photo photo) return false;

            // Assume SidebarViewModel.SelectedMenu is passed in and triggers PhotosView.Refresh()
            var selectedMenu = (App.Current?.MainWindow?.DataContext is MainWindowViewModel mwvm)
                   ? mwvm.Sidebar.SelectedMenu
                   : "Library";

            return selectedMenu switch
            {
                "Favorite" => photo.IsFavorite,
                _ => true
            };
        }


    }
}
