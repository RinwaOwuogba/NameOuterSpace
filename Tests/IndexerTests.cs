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

            string filePath = Path.Combine(currentDirectory, "../../../TestFiles/" + originalFileName);
            HashSet<string> stopWords = new HashSet<string>();

            stopWords.Add("a");

            Dictionary<string, long> forwardIndex = new Indexer(stopWords).IndexFile(filePath);

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
            HashSet<string> stopWords = new HashSet<string>();

            stopWords.Add("in");

            Dictionary<string, long> forwardIndex = new Indexer(stopWords).IndexText(text);

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

        [TestMethod]
        public void Indexer_IndexText_RemovesPunctuationInWords()
        {
            string text = "bank. blah blah";

            HashSet<string> stopWords = new HashSet<string>();

            stopWords.Add("a");

            Dictionary<string, long> forwardIndex = new Indexer(stopWords).IndexText(text);

            CollectionAssert.Contains(forwardIndex.Keys, "bank");
            CollectionAssert.Contains(forwardIndex.Keys, "blah");
            CollectionAssert.DoesNotContain(forwardIndex.Keys, "bank.");
        }
    }
}
