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

            string filePath = Path.Combine(currentDirectory, "../../../../Files/" + originalFileName);
            string[] stopWords = { "a" };

            Dictionary<string, long> reverseIndex = new Indexer(filePath, stopWords).Index();

            Assert.IsTrue(reverseIndex.ContainsKey("nest"));
            Assert.IsTrue(reverseIndex.ContainsKey("first"));
            Assert.IsTrue(reverseIndex.ContainsKey("more"));

            Assert.AreEqual(2, reverseIndex["nest"]);
            Assert.AreEqual(1, reverseIndex["first"]);
            Assert.AreEqual(1, reverseIndex["more"]);
        }

        // [TestMethod]
        // public void ReadFile_ShouldReturnTheNameAndContentOfAValidFileRead()
        // {
        //     string originalFileName = "sample.txt";
        //     string currentDirectory = Directory.GetCurrentDirectory();

        //     string filePath = Path.Combine(currentDirectory, "../../../../Files/" + originalFileName);

        //     var (returnedFileName, text) = Indexer.ReadFile(filePath);

        //     Assert.AreEqual(originalFileName, returnedFileName);
        //     Assert.AreEqual("sample text", text);
        // }

        // [TestMethod]
        // public void ReadFile_ShouldThrowAnExceptionForAnUnsuccessfulFileRead()
        // {
        //     string originalFileName = "non-existent.txt";
        //     string currentDirectory = Directory.GetCurrentDirectory();

        //     string filePath = Path.Combine(currentDirectory, "../../../../Files/" + originalFileName);

        //     Assert.ThrowsException<FileNotFoundException>(() => Indexer.ReadFile(filePath));
        // }
    }
}
