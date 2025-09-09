using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace PhotoManager.App.Services
{
    public class FavoriteService
    {
        private readonly string _favoritesFile;

        public FavoriteService()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appFolder = Path.Combine(appData, "PhotoManager");
            Directory.CreateDirectory(appFolder);

            _favoritesFile = Path.Combine(appFolder, "favorites.json");
        }

        public HashSet<string> LoadFavorites()
        {
            if (!File.Exists(_favoritesFile))
                return new HashSet<string>();

            string json = File.ReadAllText(_favoritesFile);
            return JsonSerializer.Deserialize<HashSet<string>>(json) ?? new HashSet<string>();
        }

        public void SaveFavorites(HashSet<string> favorites)
        {
            string json = JsonSerializer.Serialize(favorites, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_favoritesFile, json);
        }
    }
}
