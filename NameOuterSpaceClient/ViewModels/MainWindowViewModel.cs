using System;
using System.Collections.Generic;
using System.Text;
using System.Reactive.Linq;
using ReactiveUI;
using NameOuterSpace;


namespace NameOuterSpaceClient.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        ViewModelBase content;

        public ViewModelBase Content
        {
            get => content;
            private set => this.RaiseAndSetIfChanged(ref content, value);
        }

        public MainWindowViewModel(Querier querier, Engine engine)
        {
            this.content = new SearchViewModel(querier, engine);
        }
    }
}
