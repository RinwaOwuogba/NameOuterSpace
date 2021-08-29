using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SearchEngine;

namespace Tests
{
    [TestClass]
    public class TestEngine
    {
        Engine engine;

        [TestInitialize]
        public void SetUp()
        {
            engine = new Engine("../../../TestFiles/", pathToStopWords: "../../../stopwords.txt",
            connectionString: "../../../database.db");
        }

        [TestCleanup]
        public void TearDown()
        {
            File.Delete("../../../database.db");
            File.Delete("../../../database-log.db");
        }

        [TestMethod]
        public void Test_MetaInfoCreatedOnInit()
        {
            var meta = engine.GetMetaInfo();
            Assert.IsInstanceOfType(meta, typeof(MetaDetails));
        }

        [TestMethod]
        public void Test_Engine_MetaInfo_UpdateMetaInfoWorks()
        {
            var meta = engine.GetMetaInfo();
            var oldIndexcount = meta.indexedDocumentCount;
            var oldstopwords = meta.stopWords;
            var oldcrawltime = meta.lastRepoTraverseTime;
            var oldrepopath = meta.repositoryPath;

            // editing the meta details
            meta.indexedDocumentCount = 8;
            meta.stopWords = new List<String>();
            meta.lastRepoTraverseTime = DateTime.MaxValue;
            meta.repositoryPath = "new path";

            engine.UpdateMetaInfo(meta);

            var newmeta = engine.GetMetaInfo();

            Assert.AreNotEqual(Tuple.Create(oldcrawltime, oldIndexcount, oldrepopath, oldstopwords),
                                         Tuple.Create(meta.lastRepoTraverseTime, meta.indexedDocumentCount,
                                         meta.repositoryPath, meta.stopWords));
        }

        [TestMethod]
        public void Test_Engine_AddDocument_addsDocumentToDB()
        {
            var filename = "simple.html";
            var soon_to_be_outdatedmeta = engine.GetMetaInfo();

            var docid = engine.AddDocument(filename);

            var docinDb = engine.GetDocument(docid);

            Assert.AreEqual(docid, 1);
            Assert.AreEqual(filename, docinDb.Filename);
            Assert.AreEqual(soon_to_be_outdatedmeta.indexedDocumentCount + 1,
                            engine.GetMetaInfo().indexedDocumentCount);
        }

        [TestMethod]
        public void Test_Engine_DeleteDocumentByID_actuallyDeletesDocFromDB()
        {
            var filename1 = "simple.html";
            var filename2 = "simple.ppt";
            var docid1 = engine.AddDocument(filename1);
            var docid2 = engine.AddDocument(filename2);

            engine.DeleteDocument(docid1);

            Assert.AreEqual(1, engine.GetMetaInfo().indexedDocumentCount);
            Assert.IsNull(engine.GetDocument(docid1));
        }

        [TestMethod]
        public void Test_Engine_DeleteDocumentByFilename_actuallyDeletesDocFromDB()
        {
            var filename1 = "simple.html";
            var filename2 = "simple.ppt";
            var docid1 = engine.AddDocument(filename1);
            var docid2 = engine.AddDocument(filename2);

            engine.DeleteDocument(filename1);

            Assert.AreEqual(1, engine.GetMetaInfo().indexedDocumentCount);
            Assert.IsNull(engine.GetDocument(filename1));
        }

        [TestMethod]
        public void Test_Engine_GetDocument_ByIDWorks()
        {
            var filename1 = "simple.html";
            var filename2 = "simple.ppt";
            var docid1 = engine.AddDocument(filename1);
            var docid2 = engine.AddDocument(filename2);

            var doc = engine.GetDocument(docid2);

            Assert.AreEqual(doc.Id, docid2);
            Assert.AreEqual(doc.Filename, filename2);
        }

        // [TestMethod]

        // public void Test_Engine_GetDocument_ByID_FailsWithBadID(){
        //     var filename1 = "simple.html";
        //     var filename2 = "simple.ppt";
        //     var docid1 = engine.AddDocument(filename1);
        //     var docid2 = engine.AddDocument(filename2);

        //     var doc = engine.GetDocument(1000);

        //     Assert.AreEqual(doc.Id, docid2);
        //     Assert.AreEqual(doc.Filename, filename2);
        // }

        [TestMethod]
        public void Test_Engine_GetDocument_ByFileNameWorks()
        {
            var filename1 = "simple.html";
            var filename2 = "simple.ppt";
            var docid1 = engine.AddDocument(filename1);
            var docid2 = engine.AddDocument(filename2);

            var doc = engine.GetDocument(filename1);

            Assert.AreEqual(doc.Id, docid1);
            Assert.AreEqual(doc.Filename, filename1);
        }

        [TestMethod]
        public void Test_Engine_GetAllDocuments_WorksASExpected()
        {
            var filename1 = "simple.html";
            var filename2 = "simple.ppt";
            var filename3 = "simple.xml";
            engine.AddDocument(filename1);
            engine.AddDocument(filename2);
            engine.AddDocument(filename3);

            var docs = engine.GetAllDocuments();

            Assert.AreEqual(3, docs.Count);
            Assert.IsInstanceOfType(docs[0], typeof(FileDocument));
        }

        [TestMethod]
        public void Test_Engine_GetDocumentsByIDS_WorksAsExpected()
        {
            var filename1 = "simple.html";
            var filename2 = "simple.ppt";
            var filename3 = "simple.xml";
            engine.AddDocument(filename1);
            engine.AddDocument(filename2);
            engine.AddDocument(filename3);

            var docs = engine.GetDocuments(new List<int>() { 1, 2 });

            Assert.AreEqual(2, docs.Count);
        }

        [TestMethod]
        public void Test_Engine_GetDocumentsByFilenames_WorksAsExpected()
        {
            var filename1 = "simple.html";
            var filename2 = "simple.ppt";
            var filename3 = "simple.xml";
            engine.AddDocument(filename1);
            engine.AddDocument(filename2);
            engine.AddDocument(filename3);

            var docs = engine.GetDocuments(new List<string>() { filename3, filename2 });

            Assert.AreEqual(2, docs.Count);
        }

        [TestMethod]
        public void Test_Engine_AddWordDocument_worksWellOnNewWord()
        {
            var filename1 = "simple.html";
            var filename2 = "simple.ppt";
            var docid1 = engine.AddDocument(filename1);
            var docid2 = engine.AddDocument(filename2);

            var word1 = "dog";
            var word2 = "cat";

            var doc1_forwardIndex = new Dictionary<string, long>();
            doc1_forwardIndex.Add(word1, 3);
            doc1_forwardIndex.Add(word2, 2);

            var doc2_forwardIndex = new Dictionary<string, long>();
            doc2_forwardIndex.Add(word2, 2);

            engine.AddIntoReverseIndex(docid1, doc1_forwardIndex);
            engine.AddIntoReverseIndex(docid2, doc2_forwardIndex);

            var worddoc = engine.GetWordDocument(word2);

            Assert.AreEqual(worddoc.Documents.Count, 2);
        }

        [TestMethod]
        public void Test_Engine_GetAllWords_fetchesALLWordsInIndex()
        {
            var filename1 = "simple.html";
            var filename2 = "simple.ppt";
            var docid1 = engine.AddDocument(filename1);
            var docid2 = engine.AddDocument(filename2);

            var word1 = "dog";
            var word2 = "cat";
            var word3 = "chair";

            var doc1_forwardIndex = new Dictionary<string, long>();
            doc1_forwardIndex.Add(word1, 3);
            doc1_forwardIndex.Add(word2, 2);
            doc1_forwardIndex.Add(word3, 1);

            var doc2_forwardIndex = new Dictionary<string, long>();
            doc2_forwardIndex.Add(word2, 2);
            doc2_forwardIndex.Add(word3, 115);

            engine.AddIntoReverseIndex(docid1, doc1_forwardIndex);
            engine.AddIntoReverseIndex(docid2, doc2_forwardIndex);

            var words = engine.GetAllWords();

            Assert.AreEqual(words.Count, 3);
            CollectionAssert.Contains(words, word1);
        }

        [TestMethod]
        public void Test_Engine_DeleteWordDocument_works()
        {
            var filename1 = "simple.html";
            var filename2 = "simple.ppt";
            var docid1 = engine.AddDocument(filename1);
            var docid2 = engine.AddDocument(filename2);

            var word1 = "dog";
            var word2 = "cat";
            var word3 = "chair";

            var doc1_forwardIndex = new Dictionary<string, long>();
            doc1_forwardIndex.Add(word1, 3);
            doc1_forwardIndex.Add(word2, 2);
            doc1_forwardIndex.Add(word3, 1);

            var doc2_forwardIndex = new Dictionary<string, long>();
            doc2_forwardIndex.Add(word2, 2);
            doc2_forwardIndex.Add(word3, 115);

            engine.AddIntoReverseIndex(docid1, doc1_forwardIndex);
            engine.AddIntoReverseIndex(docid2, doc2_forwardIndex);

            engine.DeleteWord(word1);
            var deletedword = engine.GetWordDocument(word1);

            var words = engine.GetAllWords();

            Assert.IsNull(deletedword);
            Assert.AreEqual(words.Count, 2);
            CollectionAssert.DoesNotContain(words, word1);

        }

        [TestMethod]
        public void Test_Engine_DeleteDocumentReferencesFromInvertedIndex_works()
        {
            var filename1 = "deletetest1.txt";
            var filename2 = "deletetestdeux.txt";
            var meta = engine.GetMetaInfo();
            var docid1 = engine.AddDocument(filename1);
            var docid2 = engine.AddDocument(filename2);

            var i = new Indexer(meta.stopWords.ToHashSet<string>());
            var dex1 = i.IndexFile(meta.repositoryPath + filename1);

            var j = new Indexer(meta.stopWords.ToHashSet<string>());
            var dex2 = j.IndexFile(meta.repositoryPath + filename2);

            var wordsharedbyboth = "babe";

            engine.AddIntoReverseIndex(docid1, dex1);
            engine.AddIntoReverseIndex(docid2, dex2);

            var words = engine.GetAllWords();

            engine.DeleteDocumentReferencesFromInvertedIndex(docid1);

            var worddocs = engine.GetWordDocument(wordsharedbyboth);
            Assert.IsFalse(worddocs.Documents.ContainsKey(docid1));
        }

        [TestMethod]
        public void Test_Count_InvertedIndex()
        {
            File.WriteAllText("temp.txt", "greg monday creek");
            var i = new Indexer(new HashSet<string>());
            var dex = i.IndexFile("temp.txt");

            engine.AddIntoReverseIndex(1, dex);

            Assert.AreEqual(4, engine.CountInvertedIndex());
            File.Delete("temp.txt");
        }
    }

}
