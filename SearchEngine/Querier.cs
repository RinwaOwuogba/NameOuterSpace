using System;
using System.IO;
using System.Transactions;
using System.Linq;
using System.Collections.Generic;
using LiteDB;

namespace SearchEngine
{
    public class Querier{
        private Engine engine;
        private Autocomplete autocomplete;
        private ParsedQuery parser;
        private Indexer indexer;


        public Querier(Engine eng){
            engine = eng;
        }

        public string[] GetCompletions(string word){
            autocomplete = new Autocomplete(engine.GetAllWords());
            return autocomplete.auto(word);
        }

        public void Query(string query){
            this.indexer =  new Indexer(this.engine.getmetainfo.stopwords);
            this.parser = new ParsedQuery(query, this.indexer);
            var ranker = new Ranker(this.parser, this.engine);
            ranker.Rank();
            var ranks = ranker.documentRanks;
            var ids = new List<int>();
            for(files in ranks){
                ids.Add(files.Key);
            }
            var result = engine.GetDocuments(ids);

        }
        

    }
}
