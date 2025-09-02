namespace PhotoManager.App.Models
{
    public class Photo
    {
        public string FilePath { get; set; } = string.Empty; // initialized to avoid null warning
        public string FileName => System.IO.Path.GetFileName(FilePath);
    }
}
