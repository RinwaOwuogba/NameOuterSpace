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
using System.Diagnostics;

using SearchEngine;


namespace SearchEngineClient.ViewModels
{
    public class SearchViewModel : ViewModelBase
    {
        string keyword = "";
        long queryTime = 0;
        Engine engine;


        public ObservableCollection<string> AutoCompleteList
        {
            get
            {
                // new List<string>() { "pert", "get", "met" };
                if (this.engine.GetMetaInfo().lexicon.Count > 0)
                {
                    return new ObservableCollection<string>(this.engine.GetMetaInfo().lexicon);
                }
                else
                {
                    return new ObservableCollection<string>();
                }
            }
            // get => this.engine.GetMetaInfo().lexicon;
        }
        public string Keyword
        {
            get => keyword;
            set => this.RaiseAndSetIfChanged(ref keyword, value);
        }

        public long QueryTime
        {
            get => queryTime;
            private set => this.RaiseAndSetIfChanged(ref queryTime, value);
        }

        ObservableCollection<DocumentResultModel> results =
            new ObservableCollection<DocumentResultModel>();
        public ObservableCollection<DocumentResultModel> Results
        {
            get => results;
            private set => this.RaiseAndSetIfChanged(ref results, value);
        }

        public SearchViewModel(Querier querier, Engine engine)
        {
            this.engine = engine;

            // command to get search query
            this.Search = ReactiveCommand.Create(
                () => this.Keyword
            );

            // commmand to open selected file
            this.OpenFile = ReactiveCommand.Create<string>(
                (filePath) =>
            {
                try
                {
                    // TODO: Querier should return absolute file path
                    string modifiedFilePath = Directory.GetCurrentDirectory() + "/" + filePath;

                    // open file with default application
                    new Process
                    {
                        StartInfo = new ProcessStartInfo(modifiedFilePath)
                        {
                            UseShellExecute = true
                        }
                    }.Start();
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine("ex.Message: " + ex.Message);
                }
            });

            this.Search
                .Subscribe(keyword =>
                {
                    if (keyword != null)
                    {
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();
                        List<DocumentResultModel> queryResults = querier.Query(keyword);
                        stopwatch.Stop();

                        this.QueryTime = stopwatch.ElapsedMilliseconds;

                        this.Results = new ObservableCollection<DocumentResultModel>(queryResults);
                    }
                });
        }

        public ReactiveCommand<Unit, string> Search { get; }
        public ReactiveCommand<string, Unit> OpenFile { get; }


    }
}