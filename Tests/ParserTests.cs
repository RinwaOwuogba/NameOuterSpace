using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using SearchEngine;

namespace Tests
{
    [TestClass]
    public class AutoDetectParserTests
    {
        [TestMethod]
        public void GetContextParser_ShouldThrowAnArgumentExceptionForAnInvalidFileType()
        {
            string originalFileName = "simpleXml.xyz";

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "../../../../Files/" + originalFileName
            );

            Assert.ThrowsException<ArgumentException>(
                () => AutoDetectParser.GetContextParser(filePath),
                "Unrecognized file type"
            );
        }

        [TestMethod]
        public void GetContextParser_ShouldReturnAnXmlParserForAnXmlFile()
        {
            string originalFileName = "simple.xml";

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "../../../../Files/" + originalFileName
           );

            Parser parser = AutoDetectParser.GetContextParser(filePath);

            Assert.IsInstanceOfType(parser, typeof(XmlParser));
        }

        [TestMethod]
        public void GetContextParser_ShouldParseAnHtmlFileSuccessfully()
        {
            string originalFileName = "simple.html";

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "../../../../Files/" + originalFileName
           );

            Parser parser = AutoDetectParser.GetContextParser(filePath);

            Assert.IsInstanceOfType(parser, typeof(HtmlParser));
        }

        [TestMethod]
        public void GetContextParser_ShouldReturnATxtParserForATxtFile()
        {
            string originalFileName = "simple.txt";

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "../../../../Files/" + originalFileName
           );

            Parser parser = AutoDetectParser.GetContextParser(filePath);

            Assert.IsInstanceOfType(parser, typeof(TxtParser));

        }

        [TestMethod]
        public void GetContextParser_ShouldReturnPDFParserForPDFFile()
        {
            string originalFileName = "simple.pdf";

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "../../../../Files/" + originalFileName
           );

            Parser parser = AutoDetectParser.GetContextParser(filePath);

            Assert.IsInstanceOfType(parser, typeof(PDFParser));
        }

        [TestMethod]
        public void GetContextParser_ShouldReturnDocParserForDocFile()
        {
            string originalFileName = "simple.doc";

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "../../../../Files/" + originalFileName
           );

            // string result = new Parser(filePath).AutoDetectParse();
            // Assert.AreEqual(
            //     "  Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nunc ac faucibus odio. \n",
            //     result
            // );

            Parser parser = AutoDetectParser.GetContextParser(filePath);

            Assert.IsInstanceOfType(parser, typeof(DocParser));
        }

        [TestMethod]
        public void GetContextParser_ShouldReturnDocxParserForDocxFile()
        {
            string originalFileName = "simple.docx";

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "../../../../Files/" + originalFileName
           );

            // string result = new Parser(filePath).AutoDetectParse();

            // Assert.AreEqual("book cook chef\n", result);

            Parser parser = AutoDetectParser.GetContextParser(filePath);

            Assert.IsInstanceOfType(parser, typeof(DocParser));
        }

        // [TestMethod]
        // public void GetContextParser_ShouldReturnPresentationParserForPptFile()
        // {
        //     string originalFileName = "simple.ppt";

        //     string filePath = Path.Combine(
        //         Directory.GetCurrentDirectory(),
        //         "../../../../Files/" + originalFileName
        //    );

        //     Parser parser = AutoDetectParser.GetContextParser(filePath);

        //     Assert.IsInstanceOfType(parser, typeof(PresentationParser));
        // }

        // [TestMethod]
        // public void GetContextParser_ShouldReturnPresentationParserForPptsFile()

        // {
        //     string originalFileName = "simple.ppts";

        //     string filePath = Path.Combine(
        //         Directory.GetCurrentDirectory(),
        //         "../../../../Files/" + originalFileName
        //    );

        //     Parser parser = AutoDetectParser.GetContextParser(filePath);

        //     Assert.IsInstanceOfType(parser, typeof(PresentationParser));
        // }
    }
}