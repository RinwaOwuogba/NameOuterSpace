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

            ///
            /// 
            var wordList = this.engine.GetWordDocuments(
                new List<string>(this.parsedquery.QueryIndex.Keys)
            );

            foreach (WordDocument item in wordList)
            {
                Console.WriteLine("item: " + item.Word + " item.TotalOccurrence: " + item.TotalOccurrence);
                Console.WriteLine("item.Documents.Count " + item.Documents.Count);
                Console.WriteLine("``````````````````` ");
            }
            ///

            return engine.GetDocuments(ranks.Select(x => x.Key).ToList());

        }


    }
}
