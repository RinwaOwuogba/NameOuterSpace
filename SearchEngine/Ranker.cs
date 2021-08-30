using System;
using System.Collections.Generic;

namespace SearchEngine
{
    /// <summary>
    /// Class to retrieve all relevant documents to a
    /// particular query ranked in order of relevance
    /// </summary>
    public class Ranker
    {
        /// <summary>
        /// Engine to use in accessing reverse index
        /// </summary>
        private IEngine engine;

        /// <summary>
        /// Query to rank relevant documents for
        /// </summary>
        public ParsedQuery ParsedQuery { get; }

        /// <summary>
        /// Dictionary mapping document ID to term weights in
        /// document
        /// </summary>
        public Dictionary<int, Dictionary<string, double>> documentTermWeights
        {
            get;
            protected set;
        }

        /// <summary>
        /// Documents rank according to query relevance
        /// </summary>
        public List<KeyValuePair<int, double>> documentRanks
        {
            get;
            private set;
        }

        /// <summary>
        /// Weight of query terms in relation to query
        /// </summary>
        public Dictionary<string, double> queryTermWeights
        {
            get;
            protected set;
        }

        /// <summary>
        /// List of word documents <see cref="WordDocument" /> from the inverted index
        /// that relate to the current query
        /// </summary>
        private List<WordDocument> wordDocumentsList;

        /// <summary>
        /// Constructor for a ranker instance
        /// </summary>
        /// <param name="query">Query to fetch results for</param>
        /// <param name="engine">An instance of the engine class that
        /// implements the <see cref="IEngine" /> interface
        /// </param>
        public Ranker(ParsedQuery query, IEngine engine)
        {
            this.engine = engine;
            this.ParsedQuery = query;

            this.documentTermWeights = new Dictionary<int, Dictionary<string, double>>();
            this.wordDocumentsList = this.wordDocumentsList = this.engine.GetWordDocuments(
                new List<string>(this.ParsedQuery.QueryIndex.Keys)
            );
        }

        /// <summary>
        /// Calulates relevant documents to the current query sorted in
        /// descending / ascending order (not sure) yet by relevance
        /// </summary>
        public void Rank()
        {
            this.AggregateQueryTermWeights();
            this.AggregateDocumentTermWeights();

            this.documentRanks = Ranker.CalculateDocumentsQueryRelevance(
                this.documentTermWeights,
                this.ParsedQuery,
                this.queryTermWeights
            );

            this.documentRanks.Sort(
                (docRank1, docRank2) => docRank2.Key.CompareTo(docRank1.Key)
            );
        }

        /// <summary>
        /// Collects the weight of every term in the current query relative
        /// to the query itself
        /// </summary>
        public void AggregateQueryTermWeights()
        {
            this.queryTermWeights = new Dictionary<string, double>();

            long maxTermFreq = this.ParsedQuery.GetMaxQueryTermFreq();
            long documentsInCollection = this.engine.GetAllDocumentsCount();

            foreach (WordDocument wordDocument in this.wordDocumentsList)
            {
                long totalTermOccurrenceInCollection = wordDocument.totalOccurrence;
                double termIDF =
                    Math.Log2(
                        (documentsInCollection - totalTermOccurrenceInCollection) /
                        totalTermOccurrenceInCollection
                    );

                double termWeightInQuery =
                    (0.5 + ((0.5 * this.ParsedQuery.QueryIndex[wordDocument.Word]) /
                    maxTermFreq)) * termIDF;

                this.queryTermWeights.Add(wordDocument.Word, termWeightInQuery);
            }
        }

        /// <summary>
        /// Collects the weight of every term in the current query relative to 
        /// every relevant document
        /// </summary>
        public void AggregateDocumentTermWeights()
        {
            this.documentTermWeights = new Dictionary<int, Dictionary<string, double>>();
            long documentsInCollection = this.engine.GetAllDocumentsCount();

            foreach (WordDocument wordDocument in this.wordDocumentsList)
            {
                long totalTermOccurrenceInCollection = wordDocument.totalOccurrence;
                double termIDF =
                    Math.Log2(
                        (documentsInCollection - totalTermOccurrenceInCollection) /
                        totalTermOccurrenceInCollection
                    );

                foreach (KeyValuePair<int, long> document in wordDocument.Documents)
                {
                    // calculate term weight in document
                    double termWeightInDocument = 0;
                    termWeightInDocument = document.Value * termIDF;

                    // add term weight to collection of document term weights
                    if (this.documentTermWeights.ContainsKey(document.Key))
                    {
                        this.documentTermWeights[document.Key][wordDocument.Word] = termWeightInDocument;
                    }
                    else
                    {
                        this.documentTermWeights.Add(document.Key, new Dictionary<string, double>());

                        this.documentTermWeights[document.Key][wordDocument.Word] = termWeightInDocument;
                    }
                }
            }
        }

        /// <summary>
        /// Calculates relevance of each related document to the current query
        /// using cosine similarity formula
        /// 
        /// http://orion.lcg.ufrj.br/Dr.Dobbs/books/book5/chap14.htm
        /// </summary>
        public static List<KeyValuePair<int, double>> CalculateDocumentsQueryRelevance(
            Dictionary<int, Dictionary<string, double>> documentTermWeights,
            IParsedQuery parsedQuery,
            Dictionary<string, double> queryTermWeights
        )
        {
            List<KeyValuePair<int, double>> documentRanks = new List<KeyValuePair<int, double>>();

            foreach (KeyValuePair<int, Dictionary<string, double>> document in documentTermWeights)
            {
                double similarity = 0;
                double queryDocumentDotProduct = 0;

                double queryQueryDotProduct = 0;
                double documentDocumentDotProduct = 0;

                foreach (KeyValuePair<string, long> queryTermEntry in parsedQuery.QueryIndex)
                {
                    double weightInQuery = queryTermWeights[queryTermEntry.Key];
                    double weightInDocument = document.Value[queryTermEntry.Key];

                    queryDocumentDotProduct += weightInQuery * weightInDocument;

                    queryQueryDotProduct += Math.Pow(weightInQuery, 2);
                    documentDocumentDotProduct += Math.Pow(weightInDocument, 2);
                }

                // calculate document query similarity by cosine similarity measure
                // with in-document weight
                if (queryDocumentDotProduct != 0)
                {
                    similarity =
                        queryDocumentDotProduct /
                        Math.Sqrt((queryQueryDotProduct * documentDocumentDotProduct));
                }
                else
                {
                    similarity = 0;
                }


                documentRanks.Add(new KeyValuePair<int, double>(document.Key, similarity));
            }

            return documentRanks;
        }

    }
}
