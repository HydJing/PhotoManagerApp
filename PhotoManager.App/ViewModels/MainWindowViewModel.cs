using CommunityToolkit.Mvvm.ComponentModel;

namespace PhotoManager.App.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        public SidebarViewModel Sidebar { get; }
        public ToolbarViewModel Toolbar { get; }
        public PhotoGalleryViewModel PhotoGallery { get; }
        public StatusBarViewModel StatusBar { get; }

        public MainWindowViewModel()
        {
            // Shared gallery & status so they talk to each other
            PhotoGallery = new PhotoGalleryViewModel();
            StatusBar = new StatusBarViewModel(PhotoGallery);

            // Sidebar needs access to PhotoGallery for filtering
            Sidebar = new SidebarViewModel(PhotoGallery);

            // Toolbar controls loading/deleting
            Toolbar = new ToolbarViewModel(PhotoGallery);
        }
    }
}
