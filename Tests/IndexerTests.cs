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
        public void Index_ShouldReturnAReverseIndexOfAFileContent()
        {
            string originalFileName = "simple.html";

            string currentDirectory = Directory.GetCurrentDirectory();

            string filePath = Path.Combine(currentDirectory, "../../../TestFiles/" + originalFileName);
            string[] stopWords = { "a" };

            Dictionary<string, long> reverseIndex = new Indexer(filePath, stopWords).Index();

            Assert.IsTrue(reverseIndex.ContainsKey("nest"));
            Assert.IsTrue(reverseIndex.ContainsKey("first"));
            Assert.IsTrue(reverseIndex.ContainsKey("more"));

            Assert.AreEqual(2, reverseIndex["nest"]);
            Assert.AreEqual(1, reverseIndex["first"]);
            Assert.AreEqual(1, reverseIndex["more"]);
        }
    }
}
