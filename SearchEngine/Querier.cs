using System;
using System.IO;
using System.Transactions;
using System.Linq;
using System.Collections.Generic;
using LiteDB;

namespace SearchEngine
{
    public class Querier
    {
        private Engine engine;
        private Autocomplete autocomplete;

        public Querier(Engine eng)
        {
            engine = eng;
        }

        public string[] GetCompletions(string word)
        {
            autocomplete = new Autocomplete(engine.GetAllWords());
            return autocomplete.auto(word);
        }

    }
}
