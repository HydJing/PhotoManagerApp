using PhotoManager.App.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PhotoManager.App.Services
{
    public class PhotoLoaderService
    {
        private readonly string[] _extensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };

        public IEnumerable<Photo> LoadPhotos(string folderPath, HashSet<string> favorites)
        {
            if (!Directory.Exists(folderPath)) yield break;

            var files = Directory.EnumerateFiles(folderPath, "*.*", SearchOption.AllDirectories)
                .Where(f => _extensions.Contains(Path.GetExtension(f).ToLower()));

            foreach (var file in files)
            {
                var photo = new Photo
                {
                    FilePath = file,
                    FileName = Path.GetFileName(file),
                    IsFavorite = favorites.Contains(file)
                };
                photo.LoadThumbnail();
                yield return photo;
            }
        }
    }
}
