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
        [ObservableProperty] private string fileName = string.Empty;
        [ObservableProperty] private string fileSize = string.Empty;
        [ObservableProperty] private string dimensions = string.Empty;
        [ObservableProperty] private bool isFavorite;

        public void LoadThumbnail()
        {
            if (!File.Exists(FilePath)) return;

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad; // closes file after load
            bitmap.UriSource = new Uri(FilePath);
            bitmap.DecodePixelWidth = 200;
            bitmap.EndInit();
            bitmap.Freeze();

            Thumbnail = bitmap;

            FileName = Path.GetFileName(FilePath);

            var info = new FileInfo(FilePath);
            FileSize = $"{info.Length / 1024.0:0.0} KB";

            Dimensions = $"{bitmap.PixelWidth}×{bitmap.PixelHeight}";
        }
    }
}
