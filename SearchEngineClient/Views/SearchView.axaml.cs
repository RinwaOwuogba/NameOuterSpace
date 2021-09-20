using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;

namespace SearchEngineClient.Views
{
    public class SearchView : UserControl
    {
        // public string[] Names = new string[] { "john", "jike", "jet" };

        public SearchView()
        {
            InitializeComponent();

            AutoCompleteBox searchBox = this.FindControl<AutoCompleteBox>("SearchBox");

            if (searchBox.Items != null)
            {
                foreach (string item in searchBox.Items)
                {
                    Console.WriteLine("item: " + item);
                }
            }

            // searchBox.Items = this.Names;
            // searchBox.Items = engine.GetMetaInfo().Lexicon;
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


            // var words = searchText.Split(' ');
            // var options = Sentences.Select(x => x.First).ToArray();
            // for (var i = 0; i < words.Length; ++i)
            // {
            //     var word = words[i];
            //     for (var j = 0; j < options.Length; ++j)
            //     {
            //         var option = options[j];
            //         if (option == null)
            //             continue;

            //         if (i == words.Length - 1)
            //         {
            //             options[j] = option.Value.ToLower().Contains(word.ToLower()) ? option : null;
            //         }
            //         else
            //         {
            //             options[j] = option.Value.Equals(word, StringComparison.InvariantCultureIgnoreCase) ? option.Next : null;
            //         }
            //     }
            // }

            // return options.Any(x => x != null && x.Value == item);
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