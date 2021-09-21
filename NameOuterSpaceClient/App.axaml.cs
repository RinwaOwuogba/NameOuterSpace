using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using NameOuterSpaceClient.ViewModels;
using NameOuterSpaceClient.Views;
using NameOuterSpace;
using System.IO;

namespace NameOuterSpaceClient
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Engine engine = new Engine("../repository/", Directory.GetCurrentDirectory() + "stopwords.txt");
                Querier querier = new Querier(engine);
                Watcher watch = new Watcher(engine);

                // run file watcher in the background
                Watcher.IndexInBackGround(watch);

                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(querier, engine),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}