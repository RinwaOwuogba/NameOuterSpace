using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SearchEngine;

namespace Tests
{
    [TestClass]
    public class TestEngine
    {
        Engine engine;

        [TestInitialize]    
        public void SetUp(){
            engine = new Engine("../../../TestFiles/", pathToStopWords: "../../../stopwords.txt", 
            connectionString:"../../../database.db");
        }

        [TestCleanup]
        public void TearDown(){
            engine.Kill();
        }

        [ClassCleanup]
        public static void TearDownClass(){
            File.Delete("../../../database.db");
            File.Delete("../../../database-log.db");
        }

        [TestMethod]
        public void Test_MetaInfoCreatedOnInit(){
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

        // public void Test_Engine

    }
    
}
