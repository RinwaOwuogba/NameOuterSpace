using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using SearchEngine;

namespace Tests
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void AutoDetectParser_ShouldThrowAnExceptionForAnInvalidFileType()
        {
            string originalFileName = "simpleXml.xyz";

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "../../../../Files/" + originalFileName
            );

            Assert.ThrowsException<ArgumentException>(
                () => new Parser(filePath).AutoDetectParse(),
                "Unrecognized file type"
            );
        }

        [TestMethod]
        public void AutoDetectParser_ShouldParseAnXmlFileSuccessfully()
        {
            string originalFileName = "simple.xml";

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "../../../../Files/" + originalFileName
           );

            string result = new Parser(filePath).AutoDetectParse();

            Assert.AreEqual("book cook chef", result);
        }

        [TestMethod]
        public void AutoDetectParser_ShouldParseAnHtmlFileSuccessfully()
        {
            string originalFileName = "simple.html";

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "../../../../Files/" + originalFileName
           );

            string result = new Parser(filePath).AutoDetectParse();

            Assert.AreEqual("books a first nesting more nesting", result);
        }

        [TestMethod]
        public void AutoDetectParser_ShouldReturnContentsOfATxtFile()
        {
            string originalFileName = "simple.txt";

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "../../../../Files/" + originalFileName
           );

            string result = new Parser(filePath).AutoDetectParse();

            Assert.AreEqual("book cook chef", result);
        }

        [TestMethod]
        public void AutoDetectParser_ShouldReturnContentsOfAPdfFile()
        {
            string originalFileName = "simple.pdf";

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "../../../../Files/" + originalFileName
           );

            string result = new Parser(filePath).AutoDetectParse();

            Assert.AreEqual("book cook chef", result);
        }

        [TestMethod]
        public void AutoDetectParser_ShouldReturnContentsOfADocFile()
        {
            string originalFileName = "simple.doc";

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "../../../../Files/" + originalFileName
           );

            string result = new Parser(filePath).AutoDetectParse();
            Assert.AreEqual(
                "  Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nunc ac faucibus odio. \n",
                result
            );
        }

        [TestMethod]
        public void AutoDetectParser_ShouldReturnContentsOfADocxFile()
        {
            string originalFileName = "simple.docx";

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "../../../../Files/" + originalFileName
           );

            string result = new Parser(filePath).AutoDetectParse();

            Assert.AreEqual("book cook chef\n", result);
        }

        // [TestMethod]
        // public void AutoDetectParser_ShouldReturnContentsOfAPptFile()
        // {
        //     string originalFileName = "simple.ppt";

        //     string filePath = Path.Combine(
        //         Directory.GetCurrentDirectory(),
        //         "../../../../Files/" + originalFileName
        //    );

        //     string result = new Parser(filePath).AutoDetectParse();

        //     Assert.AreEqual("book cook chef", result);
        // }

        // [TestMethod]
        // public void AutoDetectParser_ShouldReturnContentsOfAPptsFile()
        // {
        //     string originalFileName = "simple.ppts";

        //     string filePath = Path.Combine(
        //         Directory.GetCurrentDirectory(),
        //         "../../../../Files/" + originalFileName
        //    );

        //     string result = new Parser(filePath).AutoDetectParse();

        //     Assert.AreEqual("book cook chef", result);
        // }
    }
}