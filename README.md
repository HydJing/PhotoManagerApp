# PhotoManager.App

## Best Practices for Building a WPF Photo Manager in .NET 8+
### 1. Target Modern .NET

 Use .NET 8 (LTS) (or .NET 9 once stable).

 Use Visual Studio 2022+ for tooling and hot reload.

 Avoid legacy WPF libraries; prefer CommunityToolkit.Mvvm over heavy frameworks unless you need advanced features.

 Use async/await and Task instead of raw threads for concurrency. Use Thread only when necessary (e.g., background image processing).

### 2. Adopt MVVM Architecture

Models: Represent your photo entities (metadata, file path, tags, ratings).

ViewModels: Handle business logic, expose observable properties, commands, and async operations.

Views (XAML): Only define UI. Use DataBinding and DataTemplates for separation.

It provides:

[ObservableProperty] for auto INotifyPropertyChanged

[RelayCommand] for ICommand without boilerplate

Dependency injection support

### 3. Follow OOP & SOLID

Single Responsibility → Each class should do one thing (e.g., PhotoLoader, TagService, ThumbnailGenerator).

Open/Closed → Add new photo formats via interfaces (e.g., IPhotoImporter).

Liskov Substitution → Derived classes (e.g., LocalPhoto vs CloudPhoto) should work wherever Photo is expected.

Interface Segregation → Don’t force classes to implement unused methods (e.g., split IImageProcessor and IMetadataExtractor).

Dependency Inversion → Depend on abstractions, not implementations. Use DI container (e.g., Microsoft.Extensions.DependencyInjection).

### 4. Threading & Performance

Use async/await for I/O (loading, saving, scanning photo directories).

Use Task.Run for CPU-bound work (e.g., generating thumbnails).

Use ObservableCollection<T> bound to UI for dynamic photo lists.

Use cancellation tokens for long-running tasks (scanning large directories).

### 5. Modern WPF Practices

Use DataTemplates for displaying photos (e.g., ListView or ItemsControl with virtualization).

Use Dependency Injection (IHostBuilder in .NET 8).

Use Configuration (appsettings.json for settings like default photo directory).

Use Logging (Microsoft.Extensions.Logging).

Support Theming (Light/Dark mode via XAML resource dictionaries).

Ensure Accessibility (Keyboard navigation, screen reader support).

### 6. Testing

Unit test ViewModels with xUnit or NUnit.

Mock file system/photo services with Moq.

Avoid UI logic in code-behind so tests stay clean.

### 7. Suggested Project Structure
PhotoManager/
 ├── PhotoManager.App/            (WPF project)
 │    ├── Views/                  (XAML UI)
 │    ├── ViewModels/             (MVVM logic)
 │    ├── App.xaml                (Resources, Themes)
 │    └── MainWindow.xaml
 ├── PhotoManager.Core/           (Models, Interfaces, Services)
 │    ├── Models/                 (Photo.cs, Album.cs)
 │    ├── Services/               (IPhotoService, ITagService)
 │    └── Utils/                  (ImageProcessor, MetadataReader)
 ├── PhotoManager.Infrastructure/ (Implementations: File, DB, Cloud)
 ├── PhotoManager.Tests/          (Unit tests for ViewModels/Services)
