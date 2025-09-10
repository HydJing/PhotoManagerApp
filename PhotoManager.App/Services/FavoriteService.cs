using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace PhotoManager.App.Services
{
    public class FavoriteService
    {
        private readonly string _filePath;

        public FavoriteService()
        {
            var appData = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "PhotoManager");
            Directory.CreateDirectory(appData);
            _filePath = Path.Combine(appData, "favorites.json");
        }

        public HashSet<string> LoadFavorites()
        {
            if (!File.Exists(_filePath)) return new HashSet<string>();
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<HashSet<string>>(json) ?? new HashSet<string>();
        }

        public void SaveFavorites(HashSet<string> favorites)
        {
            var json = JsonSerializer.Serialize(favorites, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
    }
}
