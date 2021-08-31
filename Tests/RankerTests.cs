using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using SearchEngine;
using Moq;

namespace Tests
{
    [TestClass]
    public class RankerTests
    {
        List<WordDocument> mockWordDocumentList = new List<WordDocument>();
        const int DOCUMENTS_IN_COLLECTION = 3;
        Mock<IEngine> engineMock = new Mock<IEngine>();
        HashSet<string> stopWords = new HashSet<string> { "in" };


        [TestInitialize]
        public void Setup()
        {
            WordDocument mockWordDocument1 = new WordDocument("book");
            mockWordDocument1.AddDoc(1, 50);
            mockWordDocument1.AddDoc(2, 3);
            mockWordDocument1.AddDoc(3, 10);

            WordDocument mockWordDocument2 = new WordDocument("china");
            mockWordDocument2.AddDoc(1, 3);
            mockWordDocument2.AddDoc(2, 6);
            mockWordDocument2.AddDoc(3, 1);

            WordDocument mockWordDocument3 = new WordDocument("bought");
            mockWordDocument3.AddDoc(1, 0);
            mockWordDocument3.AddDoc(2, 2);
            mockWordDocument3.AddDoc(3, 4);

            mockWordDocumentList.Add(mockWordDocument1);
            mockWordDocumentList.Add(mockWordDocument2);
            mockWordDocumentList.Add(mockWordDocument3);

            engineMock.Setup(
                engine => engine.GetAllDocumentsCount()
            ).Returns(DOCUMENTS_IN_COLLECTION);
        }


        [TestMethod]
        public void Ranker_AggregateDocumentTermWeights_CalculatesTermWeightsInDocuments()
        {
            string naturalLangQuery = "books bought in china";

            ParsedQuery parsedQuery = new ParsedQuery(naturalLangQuery, new Indexer(stopWords));
            var queryWords = new HashSet<string>(parsedQuery.QueryIndex.Keys);

            engineMock.Setup(
                engine => engine.GetWordDocuments(queryWords)
            ).Returns(mockWordDocumentList);
            // engine => engine.GetWordDocuments(It.IsAny<List<string>>())

            Ranker ranker = new Ranker(parsedQuery, engineMock.Object);
            ranker.AggregateDocumentTermWeights();

            Dictionary<string, double> termIdfs =
                new Dictionary<string, double>
                {
                    {
                        "book", Math.Log2(
                            (DOCUMENTS_IN_COLLECTION - 1) / 63
                        )
                    },
                    {
                        "china", Math.Log2(
                            (DOCUMENTS_IN_COLLECTION - 1) / 10
                        )
                    },
                    {
                        "bought", Math.Log2(
                            (DOCUMENTS_IN_COLLECTION - 1) / 6
                        )
                    }
                };

            Dictionary<int, Dictionary<string, double>> expectedDocumentTermWeights =
                new Dictionary<int, Dictionary<string, double>>
                {
                    {1, new Dictionary<string, double> {
                        {"book", 50 * termIdfs["book"]},
                        {"china", 3 * termIdfs["china"]},
                        {"bought", 0 * termIdfs["bought"]}
                    }},
                    {2, new Dictionary<string, double> {
                        { "book", 3 * termIdfs["book"]},
                        { "china", 6 * termIdfs["china"]},
                        { "bought", 2 * termIdfs["bought"]}
                    }},
                    {3, new Dictionary<string, double> {
                        { "book", 10 * termIdfs["book"]},
                        { "china", 1 * termIdfs["china"]},
                        { "bought", 4 * termIdfs["bought"]}
                    }}
                };

            CollectionAssert.AreEquivalent(expectedDocumentTermWeights[1], ranker.documentTermWeights[1]);
            CollectionAssert.AreEquivalent(expectedDocumentTermWeights[2], ranker.documentTermWeights[2]);
            CollectionAssert.AreEquivalent(expectedDocumentTermWeights[3], ranker.documentTermWeights[3]);
        }

        [TestMethod]
        public void Ranker_AggregateTermWeightsInQuery_CalculatesTermWeightsInQuery()
        {
            string query = "books bought in china";
            ParsedQuery parsedQuery = new ParsedQuery(query, new Indexer(new HashSet<string>()));
            List<string> queryWords = new List<string>(parsedQuery.QueryIndex.Keys);

            engineMock.Setup(
                engine => engine.GetWordDocuments(new HashSet<string>(queryWords))
            ).Returns(mockWordDocumentList);

            Ranker ranker = new Ranker(parsedQuery, engineMock.Object);
            ranker.AggregateQueryTermWeights();
            long maxTermFreq = parsedQuery.GetMaxQueryTermFreq();

            Dictionary<string, double> termIdfs =
                new Dictionary<string, double>
                {
                    {
                        "book", Math.Log2(
                            (DOCUMENTS_IN_COLLECTION - 1) / 63
                        )
                    },
                    {
                        "china", Math.Log2(
                            (DOCUMENTS_IN_COLLECTION - 1) / 10
                        )
                    },
                    {
                        "bought", Math.Log2(
                            (DOCUMENTS_IN_COLLECTION - 1) / 6
                        )
                    }
                };

            Dictionary<string, double> expectedQueryTermWeights =
                new Dictionary<string, double>
                {
                    {"book",
                        (
                            (0.5 + ((0.5 * parsedQuery.QueryIndex["book"]) /
                            maxTermFreq)) * termIdfs["book"]
                        )
                    },
                    {"china",                         (
                            (0.5 + ((0.5 * parsedQuery.QueryIndex["china"]) /
                            maxTermFreq)) * termIdfs["china"]
                        )
                    },
                    {"bought",                         (
                            (0.5 + ((0.5 * parsedQuery.QueryIndex["bought"]) /
                            maxTermFreq)) * termIdfs["bought"]
                        )}
                };


            CollectionAssert.AreEquivalent(expectedQueryTermWeights, ranker.queryTermWeights);
        }

        [TestMethod]
        public void Ranker_CalculateDocumentsQueryRelevance_CalculatesAccurateDocumentsRelevance()
        {
            Dictionary<int, Dictionary<string, double>> documentTermWeights =
                new Dictionary<int, Dictionary<string, double>>
                {
                    {1, new Dictionary<string, double> {
                        {"book", 5.33330},
                        {"china", 3.12323},
                        {"bought", 0}
                    }},
                    {2, new Dictionary<string, double> {
                        { "book", 3.12243},
                        { "china", 6.23434},
                        { "bought", 2.233443}
                    }},
                    {3, new Dictionary<string, double> {
                        { "book", 10.23232323},
                        { "china", 1.23232234356},
                        { "bought", 4.9876332}
                    }}
                };

            Mock<IParsedQuery> parsedQueryMock = new Mock<IParsedQuery>();
            parsedQueryMock.Setup(query => query.QueryIndex).Returns(
                new Dictionary<string, long> {
                   {"book", 1},
                   {"china", 1},
                   {"bought", 1}
                }
            );

            Dictionary<string, double> queryTermWeights = new Dictionary<string, double>
                {
                    {"book", 1.234},
                    {"china", 1.545},
                    {"bought", 1.324}
                };

            Dictionary<int, double> computedDocumentRanks = Ranker.CalculateDocumentsQueryRelevance(
                documentTermWeights,
                parsedQueryMock.Object,
                queryTermWeights
            );

            Dictionary<int, double> expectedDocumentRanks = new Dictionary<int, double>();
            expectedDocumentRanks.Add(1, 0.7755706389);
                expectedDocumentRanks.Add(2, 0.9437225296);
                expectedDocumentRanks.Add(3, 0.7756734768);


            // acceptable error margin 
            const double DELTA = 0.0000000001;

            Assert.AreEqual(expectedDocumentRanks[1], computedDocumentRanks[1], DELTA);
            Assert.AreEqual(expectedDocumentRanks[2], computedDocumentRanks[2], DELTA);
            Assert.AreEqual(expectedDocumentRanks[3], computedDocumentRanks[3], DELTA);
        }
    }
}