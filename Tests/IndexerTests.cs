using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using SearchEngine;

namespace Tests
{
    [TestClass]
    public class IndexerTests
    {

        [TestMethod]
        public void Indexer_IndexFile_ReturnsForwardIndexOfAFileContent()
        {
            string originalFileName = "simple.html";

            string currentDirectory = Directory.GetCurrentDirectory();

            string filePath = Path.Combine(currentDirectory, "../../../../Files/" + originalFileName);
            string[] stopWords = { "a" };

            Dictionary<string, long> forwardIndex = new Indexer(filePath, stopWords).IndexFile();

            Assert.IsTrue(forwardIndex.ContainsKey("nest"));
            Assert.IsTrue(forwardIndex.ContainsKey("first"));
            Assert.IsTrue(forwardIndex.ContainsKey("more"));

            Assert.AreEqual(2, forwardIndex["nest"]);
            Assert.AreEqual(1, forwardIndex["first"]);
            Assert.AreEqual(1, forwardIndex["more"]);
        }

        [TestMethod]
        public void Indexer_IndexText_ReturnsForwardIndexOfAText()
        {
            string text = "booking flights online in china";
            string[] stopWords = { "in" };

            Dictionary<string, long> forwardIndex = Indexer.IndexText(text, stopWords);

            Assert.IsTrue(forwardIndex.ContainsKey("book"));
            Assert.IsTrue(forwardIndex.ContainsKey("flight"));
            Assert.IsTrue(forwardIndex.ContainsKey("onlin"));
            Assert.IsTrue(forwardIndex.ContainsKey("china"));

            Assert.IsFalse(forwardIndex.ContainsKey("in"));

            Assert.AreEqual(1, forwardIndex["book"]);
            Assert.AreEqual(1, forwardIndex["flight"]);
            Assert.AreEqual(1, forwardIndex["onlin"]);
            Assert.AreEqual(1, forwardIndex["china"]);
        }
    }
}
