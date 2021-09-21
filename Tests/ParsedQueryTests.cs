using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using NameOuterSpace;
using Moq;

namespace Tests
{
    [TestClass]
    public class ParsedQueryTests
    {
        [TestMethod]
        public void ParsedQuery_GetMaxQueryTermFreq_ReturnsHighestTermFrequencyInQueryIndex()
        {
            string naturalLangQuery = "books bought in china about china";

            Mock<IIndexer> mockedIndexer = new Mock<IIndexer>();
            mockedIndexer
                .Setup(indexer => indexer.IndexText(naturalLangQuery))
                .Returns(new Dictionary<string, long>{
                    { "book", 1},
                    { "bought", 1},
                    { "china", 2},
                    { "about", 1},
                });

            ParsedQuery parsedQuery = new ParsedQuery(naturalLangQuery, mockedIndexer.Object);

            Assert.AreEqual(2, parsedQuery.GetMaxQueryTermFreq());
        }
    }
}