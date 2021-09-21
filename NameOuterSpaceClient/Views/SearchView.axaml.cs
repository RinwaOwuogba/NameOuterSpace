using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using NameOuterSpace;
using System.Collections.Generic;
using System.Linq;

namespace NameOuterSpaceClient.Views
{
    public class SearchView : UserControl
    {

        Autocomplete autoComplete;
        public SearchView()
        {
            InitializeComponent();

            AutoCompleteBox searchBox = this.FindControl<AutoCompleteBox>("SearchBox");

            searchBox.TextFilter = this.AutoCompleteFilter;
            searchBox.TextSelector = this.AppendWord;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private bool AutoCompleteFilter(string searchText, string item)
        {
            return true;
        }

        private string AppendWord(string text, string item)
        {
            string[] parts = text.Split(' ');
            if (parts.Length == 0)
                return item;

            parts[parts.Length - 1] = item;
            return string.Join(" ", parts);
        }
    }
}