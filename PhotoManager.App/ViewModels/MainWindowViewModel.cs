// ViewModels/MainWindowViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;

namespace PhotoManager.App.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        public SidebarViewModel Sidebar { get; }
        public PhotoGalleryViewModel PhotoGallery { get; }
        public ToolbarViewModel Toolbar { get; }
        public StatusBarViewModel StatusBar { get; }

        // DI will provide the child viewmodels
        public MainWindowViewModel(
            SidebarViewModel sidebar,
            PhotoGalleryViewModel photoGallery,
            ToolbarViewModel toolbar,
            StatusBarViewModel statusBar)
        {
            Sidebar = sidebar;
            PhotoGallery = photoGallery;
            Toolbar = toolbar;
            StatusBar = statusBar;
        }
    }
}
