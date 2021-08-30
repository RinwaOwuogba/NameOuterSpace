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
        private ParsedQuery parsedquery;
        private Indexer indexer;


        public Querier(Engine eng)
        {
            engine = eng;
            this.indexer = new Indexer(this.engine.GetMetaInfo().stopWords.ToHashSet<string>());
        }

        public string[] GetCompletions(string word)
        {
            autocomplete = new Autocomplete(engine.GetAllWords());
            return autocomplete.auto(word);
        }

        public List<FileDocument> Query(string query)
        {
            parsedquery = new ParsedQuery(query, this.indexer);

            var ranker = new Ranker(parsedquery, this.engine);

            ranker.Rank();
            var ranks = ranker.documentRanks;

            return engine.GetDocuments(ranks.Select(x => x.Key).ToList());

        }


    }
}
