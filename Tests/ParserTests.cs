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
        public void GetContextParser_ThrowsAnArgumentExceptionForAnInvalidFileType()
        {
            string fileName = "filedoesnotexist.xyz";

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "../../../TestFiles/" + fileName
            );

            Assert.ThrowsException<ArgumentException>(
                () => AutoDetectParser.GetContextParser(filePath),
                "Unrecognized file type"
            );
        }

        [TestMethod]
        public void GetContextParser_ReturnsAnXmlParserForAnXmlFile()
        {
            string fileName = "simple.xml";

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "../../../TestFiles/" + fileName
           );

            Parser parser = AutoDetectParser.GetContextParser(filePath);

            Assert.IsInstanceOfType(parser, typeof(XmlParser));
        }

        [TestMethod]
        public void GetContextParser_ParseAnHtmlFileSuccessfully()
        {
            string fileName = "simple.html";

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "../../../TestFiles/" + fileName
           );

            Parser parser = AutoDetectParser.GetContextParser(filePath);

            Assert.IsInstanceOfType(parser, typeof(HtmlParser));
        }

        [TestMethod]
        public void GetContextParser_ReturnsATxtParserForATxtFile()
        {
            string fileName = "simple.txt";

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "../../../TestFiles/" + fileName
            );

            Parser parser = AutoDetectParser.GetContextParser(filePath);

            Assert.IsInstanceOfType(parser, typeof(TxtParser));
        }

        [TestMethod]
        public void GetContextParser_ReturnsPDFParserForPDFFile()
        {
            string fileName = "simple.pdf";

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "../../../TestFiles/" + fileName
           );

            Parser parser = AutoDetectParser.GetContextParser(filePath);

            Assert.IsInstanceOfType(parser, typeof(PDFParser));
        }

        [TestMethod]
        public void GetContextParser_ReturnsDocParserForDocFile()
        {
            string fileName = "simple.doc";

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "../../../TestFiles/" + fileName
           );

            Parser parser = AutoDetectParser.GetContextParser(filePath);

            Assert.IsInstanceOfType(parser, typeof(DocParser));
        }

        [TestMethod]
        public void GetContextParser_ReturnsDocxParserForDocxFile()
        {
            string fileName = "simple.docx";

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "../../../TestFiles/" + fileName
           );


            Parser parser = AutoDetectParser.GetContextParser(filePath);

            Assert.IsInstanceOfType(parser, typeof(DocParser));
        }

        [TestMethod]
        public void GetContextParser_ReturnsPresentationParserForPptFile()
        {
            string fileName = "simple.ppt";

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "../../../TestFiles/" + fileName
           );

            Parser parser = AutoDetectParser.GetContextParser(filePath);

            Assert.IsInstanceOfType(parser, typeof(PresentationParser));
        }

        [TestMethod]
        public void GetContextParser_ReturnsPresentationParserForPptxFile()
        {
            string fileName = "simple.pptx";

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "../../../TestFiles/" + fileName
           );

            Parser parser = AutoDetectParser.GetContextParser(filePath);

            Assert.IsInstanceOfType(parser, typeof(PresentationParser));
        }
    }


    [TestClass]
    public class TxtParserTests
    {
        [TestMethod]
        public void Parse_ReturnsSemanticFileContentForTxt()
        {
            string fileName = "simple.txt";

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "../../../TestFiles/" + fileName
            );

            Parser parser = new TxtParser(filePath);

            Assert.AreEqual("book cook chef", parser.Parse());
        }
    }

    [TestClass]
    public class PDFParserTests
    {
        [TestMethod]
        public void Parse_ReturnsSemanticFileContentForPDF()
        {
            string fileName = "simple.pdf";

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "../../../TestFiles/" + fileName
            );

            Parser parser = new PDFParser(filePath);

            Assert.AreEqual("book cook chef", parser.Parse());
        }
    }

    [TestClass]
    public class XmlParserTests
    {
        [TestMethod]
        public void Parse_ReturnsSemanticFileContentForXML()
        {
            string fileName = "simple.xml";

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "../../../TestFiles/" + fileName
            );

            Parser parser = new XmlParser(filePath);

            Assert.AreEqual("book cook chef", parser.Parse());
        }
    }

    [TestClass]
    public class HtmlParserTests
    {
        [TestMethod]
        public void Parse_ReturnsSemanticFileContentForHtml()
        {
            string fileName = "simple.html";

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "../../../TestFiles/" + fileName
            );

            Parser parser = new HtmlParser(filePath);

            Assert.AreEqual("books a first nesting more nesting", parser.Parse());
        }
    }

    [TestClass]
    public class DocParserTests
    {
        [TestMethod]
        public void Parse_ReturnsSemanticFileContentForDoc()
        {
            string fileName = "simple.doc";

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "../../../TestFiles/" + fileName
            );

            Parser parser = new DocParser(filePath);

            Assert.AreEqual(
                "  Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nunc ac faucibus odio.\n",
                parser.Parse()
            );
        }
    }

    [TestClass]
    public class PresentationParserTests
    {

        [TestMethod]
        public void Parse_ReturnsSemanticFileContentForPptx()
        {
            string fileName = "simple.pptx";

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "../../../TestFiles/" + fileName
            );

            Parser parser = new PresentationParser(filePath);

            Assert.AreEqual(
                "Books Shelf doctor items ",
                parser.Parse()
            );
        }

        [TestMethod]
        public void Parse_ReturnsSemanticFileContentForPpt()
        {
            string fileName = "simple.ppt";

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "../../../TestFiles/" + fileName
            );

            Parser parser = new PresentationParser(filePath);

            Assert.AreEqual(
                "Lorem ipsum Lorem ipsum dolor sit amet, consectetur  ",
                parser.Parse()
            );
        }
    }

    [TestClass]
    public class SheetParserTests
    {

        [TestMethod]
        public void Parse_ReturnsSemanticFileContentForXls()
        {
            string fileName = "simple.xls";

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "../../../TestFiles/" + fileName
            );

            Parser parser = new SheetParser(filePath);

            Assert.AreEqual(
                "0 First Name Last Name Gender Country Age Date Id 1 Dulce Abril " +
                "Female United States 32 15/10/2017 1562",
                parser.Parse()
            );
        }

        [TestMethod]
        public void Parse_ReturnsSemanticFileContentForXlsx()
        {
            string fileName = "simple.xlsx";

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "../../../TestFiles/" + fileName
            );

            Parser parser = new SheetParser(filePath);

            Assert.AreEqual(
                "0 First Name Last Name Gender Country Age Date Id 1 Dulce Abril " +
                "Female United States 32 15/10/2017 1562",
                parser.Parse()
            );
        }
    }
}