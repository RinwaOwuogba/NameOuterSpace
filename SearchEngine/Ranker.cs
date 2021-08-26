using System;
using System.Collections.Generic;
using System.IO;
using OpenNLP.Tools.Tokenize;
using Porter2Stemmer;

namespace SearchEngine
{
    /// <summary>
    /// Class to retrieve all relevant documents to a
    /// particular query ranked in order of relevance
    /// </summary>
    public class Ranker
    {
        public ParserQuery ParserQuery { get; }

        public Ranker(ParserQuery query)
        {
            this.ParserQuery = query;
        }

        /// <summary>
        /// Calculates the weight of a query term in a particular
        /// document 
        /// </summary>
        /// <param name="term">ParserQuery term to calculate weight</param>
        /// <param name="IDF">Inverse document frequency of query term</param>
        /// <returns></returns>
        public static double CalculateTermWeight(double normalizedFrequency, double IDF)
        {
            return IDF;
        }
    }
}
