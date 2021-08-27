using System;
using System.Collections.Generic;
using System.IO;

namespace SearchEngine
{
    /// <summary>
    /// Class to retrieve all relevant documents to a
    /// particular query ranked in order of relevance
    /// </summary>
    public class Ranker
    {
        public Engine engine;
        public ParserQuery ParserQuery { get; }

        public Ranker(ParserQuery query, Engine engine)
        {
            this.engine = engine;
            this.ParserQuery = query;
        }


        /// <summary>
        /// Gets the cumulative weight of every document across word
        /// documents list
        /// </summary>
        /// <param name="wordDocumentsList">Collection of word-documents entries</param>
        /// <returns>Dictionary mapping document ID to cumulative term weight</returns>
        public Dictionary<int, double> AggregateTermWeights(List<WordDocument> wordDocumentsList)
        {
            Dictionary<int, double> documentWeights = new Dictionary<int, double>();

            // foreach (WordDocument wordDocument in wordDocumentsList)
            // {
            //     wordDocument.Documents.
            // }

            return documentWeights;
        }

        /// <summary>
        /// Calculates the weight of a query term in a particular
        /// document using Harman (1986) document similarity formula
        /// </summary>
        /// <param name="term">ParserQuery term to calculate weight</param>
        /// <param name="IDF">Inverse document frequency of query term</param>
        /// <returns></returns>
        public static double CalculateTermWeight(
            double normalizedDocumentTermFrequency,
            double IDF,
            long maxQueryTermFreq,
            long recordLength
        )
        {
            double numerator = Math.Log2(normalizedDocumentTermFrequency + 1) * IDF;
            double denominator = Math.Log2(recordLength);

            return numerator / denominator;
        }
    }
}
