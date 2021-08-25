using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SearchEngine;

namespace Tests{
     [TestClass]

    public class TestCollectionModels
    {
        [DataRow("test.txt")]
        [DataRow("test.pdf")]
        [DataRow("test.xlsx")]
        [DataRow("test.docx")]
        [DataRow("test.html")]
        [DataRow("test.pptx")]
        [DataRow("test.xml")]
        [DataTestMethod]
        public void Test_FileDocument_CalculateMD5Hash_WorksOnSupportedFiletypes(string filename)
        {
            var pathToRepo = "../../../TestFiles/";
            var document = new FileDocument()
            {
                Filename = filename,
                MD5Hash = FileDocument.CalculateMD5Hash(pathToRepo + filename)
            };
            Assert.IsNotNull(document.MD5Hash);
        }

        [TestMethod]
        public void Test_FileDocument_CalculateMD5Hash_HashChangesIfFileContentChanges(){
            var pathToRepo = "../../../TestFiles/";
            var filename = "test.txt";
            string initialhash;
            string changedhash;

            initialhash = FileDocument.CalculateMD5Hash(pathToRepo + filename);
            
            File.WriteAllText(pathToRepo + filename, "This a Random Line" + new Random().Next(100000).ToString());
            changedhash = FileDocument.CalculateMD5Hash(pathToRepo + filename);

            Assert.AreNotEqual(initialhash, changedhash);;
        }

        [TestMethod]
        public void Test_WordDocument_AddDoc(){
            var worddoc = new WordDocument("Dream");
            var docid = 4;

            worddoc.AddDoc(docid, 7);

            Assert.AreEqual(worddoc.Documents.Count, 1);
        }

        [TestMethod]
        public void Test_WordDocument_RemoveDoc(){
            var worddoc = new WordDocument("Dream");
            int doc1 = 3;
            int doc2 = 5;
            worddoc.AddDoc(doc1, 3);
            worddoc.AddDoc(doc2, 7);

            worddoc.RemoveDoc(doc1);
            Assert.AreEqual(worddoc.Documents.Count, 1);
        }
    }

}