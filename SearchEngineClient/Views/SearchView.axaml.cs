using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using SearchEngine;
using System.Collections.Generic;
using System.Linq;

namespace SearchEngineClient.Views
{
    public class SearchView : UserControl
    {

        Autocomplete autoComplete;
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