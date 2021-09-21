using System.Collections.Generic;

namespace NameOuterSpace
{
    /// <summary>
    /// Representation of a parsed natural language query
    /// </summary>
    public class ParsedQuery : IParsedQuery
    {
        /// <summary>
        /// Natural language query
        /// </summary>
        public string NaturalLangQuery { get; }

        /// <summary>
        /// Query representation of natural language query
        /// </summary>
        public Dictionary<string, long> QueryIndex { get; }
        public ParsedQuery(string naturalLanguageQuery, IIndexer indexer)
        {
            this.NaturalLangQuery = naturalLanguageQuery;
            this.QueryIndex = indexer.IndexText(naturalLanguageQuery);
        }

        /// <summary>
        /// Calculates the maximum frequency of any term in the
        /// current query representation
        /// </summary>
        /// <returns>Maximum frequency of any term in the current query</returns>
        public long GetMaxQueryTermFreq()
        {
            long maxFreq = 0;

            foreach (long frequency in this.QueryIndex.Values)
            {
                if (frequency >= maxFreq) maxFreq = frequency;
            }

            return maxFreq;
        }
    }
}