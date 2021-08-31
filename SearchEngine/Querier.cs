using System;
using System.IO;
using System.Transactions;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

using LiteDB;

namespace SearchEngine
{
    public class Querier
    {
        /// <summary>
        ///     A reference to the Engine instance
        /// </summary>
        private Engine engine;

        /// <summary>
        ///     A reference to the the autocompplete instance
        /// </summary>
        private Autocomplete autocomplete;

        /// <summary>
        ///     The parsed query that will be formed from the query given
        /// </summary>
        private ParsedQuery parsedquery;

        /// <summary>
        ///     A referencer to the the indexer
        /// </summary>
        private Indexer indexer;
        private string pathtorepo;


        /// <summary>
        ///     Initialise the Indexer and grab an engine ref
        /// </summary>
        /// <param name="eng">an instance of engine</param>
        public Querier(Engine eng)
        {   
            engine = eng;
            var meta = engine.GetMetaInfo();
            pathtorepo = meta.repositoryPath;
            this.indexer = new Indexer(meta.stopWords.ToHashSet<string>());
        }

        /// <summary>
        ///     returns words closest to the prefix argument that are stored
        ///     in the index
        /// </summary>
        /// <param name="word">a prefix of a word</param>
        /// <returns>A list of possible word completions</returns>
        public string[] GetCompletions(string word)
        {
            autocomplete = new Autocomplete(engine.GetAllWords());
            return autocomplete.auto(word);
        }

        /// <summary>
        ///     returns an 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<Tuple<string, double>> Query(string query)
        {
            parsedquery = new ParsedQuery(query, this.indexer);
            var d = parsedquery.QueryIndex;

            var ranker = new Ranker(parsedquery, this.engine);
            ranker.Rank();
            var ranks = ranker.documentRanks;

            var filedocs = engine.GetDocuments(ranks.Keys.ToHashSet());
            
            var filesAndRanks = new List<Tuple<string, double>> ();
            foreach(var docs in filedocs){
                filesAndRanks.Add(new Tuple<string, double>(pathtorepo + docs.Filename, ranks[docs.Id]));
            }
            filesAndRanks.Sort(
                (docRank1, docRank2) => docRank2.Item2.CompareTo(docRank1.Item2)
            );
            Console.WriteLine(filesAndRanks[0].Item2);
            return filesAndRanks;

        }


    }
}
