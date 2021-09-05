using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using Avalonia.Styling;
using System.Reactive;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using SearchEngineClient.Models;
using SearchEngine;
using System.Diagnostics;


namespace SearchEngineClient.ViewModels
{
    public class SearchViewModel : ViewModelBase
    {
        string keyword = "";
        public string Keyword
        {
            get => keyword;
            set => this.RaiseAndSetIfChanged(ref keyword, value);
        }

        ObservableCollection<DocumentResultModel> results =
            new ObservableCollection<DocumentResultModel>();
        public ObservableCollection<DocumentResultModel> Results
        {
            get => results;
            private set => this.RaiseAndSetIfChanged(ref results, value);
        }

        public SearchViewModel(Querier querier)
        {
            this.Search = ReactiveCommand.Create(
                () => this.Keyword
            );

            this.OpenFile = ReactiveCommand.Create<string>(
                (filePath) =>
            {
                Console.WriteLine("filePath before: " + filePath);
                try
                {
                    // Process.Start("https://www.google.com");
                    // open url in default web browser
                    System.Diagnostics.Process.Start("http://www.csharp-examples.net");

                    // Process.Start(filePath);
                    // Process.Start("/home/bolarinwa/ComputerScience/C#/SearchEngine/repository/greg.txt");
                }
                catch (System.Exception ex)
                {

                    Console.WriteLine("ex.Message: " + ex.Message);
                }
                Console.WriteLine("filePath after: " + filePath);
                // Process.Start("./ViewModelBase.cs");
                // Process.Start(filePath);
            });

            this.Search
                .Subscribe(keyword =>
                {
                    if (keyword != null)
                    {
                        List<DocumentResultModel> queryResults = querier.Query(keyword);

                        this.Results = new ObservableCollection<DocumentResultModel>(queryResults);
                    }
                });
        }

        public ReactiveCommand<Unit, string> Search { get; }
        public ReactiveCommand<string, Unit> OpenFile { get; }


    }
}