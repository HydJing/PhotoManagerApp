using CommunityToolkit.Mvvm.ComponentModel;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PhotoManager.App.Models
{
    public partial class Photo : ObservableObject
    {
        [ObservableProperty] private string filePath = string.Empty;
        [ObservableProperty] private bool isSelected;
        [ObservableProperty] private ImageSource? thumbnail;

        public void LoadThumbnail()
        {
            if (!File.Exists(FilePath)) return;

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad; // ensures file is closed after loading
            bitmap.UriSource = new Uri(FilePath);
            bitmap.DecodePixelWidth = 200; // scale down for thumbnails
            bitmap.EndInit();
            bitmap.Freeze(); // make it cross-thread & read-only

            Thumbnail = bitmap;
        }
    }
}
