using System.Collections.Generic;

namespace SearchEngine
{
    /// <summary>
    /// Representation of a parsed natural language query
    /// </summary>
    public class ParsedQuery : IParsedQuery
    {
        public string NaturalLangQuery { get; }
        public Dictionary<string, long> QueryIndex { get; }
        public ParsedQuery(string naturalLanguageQuery, Dictionary<string, long> queryIndex)
        {
            this.NaturalLangQuery = naturalLanguageQuery;
            this.QueryIndex = queryIndex;
        }

        /// <summary>
        /// Calculates the maximum frequency of any term in the
        /// current query representation
        /// </summary>
        /// <returns>Maximum frequency of any term in the current query</returns>
        public long GetMaxQueryFreq()
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