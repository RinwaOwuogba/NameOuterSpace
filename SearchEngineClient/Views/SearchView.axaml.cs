using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SearchEngineClient.Views
{
    public class SearchView : UserControl
    {
        public SearchView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}