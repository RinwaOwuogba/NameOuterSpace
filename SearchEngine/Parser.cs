using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using HtmlAgilityPack;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using Spire.Doc;
using Spire.Presentation;
using Spire.Xls;


namespace SearchEngine
{
    /// <summary>
    /// Class to get the representation for a natural
    /// language query
    /// </summary>
    public class QueryParser
    {
        public static ParsedQuery Parse(
            string naturalLangQuery,
            HashSet<string> stopWords
        )
        {
            ForwardIndex queryIndex = Indexer.IndexText(naturalLangQuery, stopWords);

            return new ParsedQuery(naturalLangQuery, queryIndex);
        }
    }

    /// <summary>
    /// Class to automatically handle parsing of semantic text from files
    /// </summary>
    public class AutoDetectParser
    {
        /// <summary>
        /// Fetches an instance of the appropriate parser for the passed
        /// file type
        /// </summary>
        /// <param name="filePath">Path of file to parse</param>
        /// <returns>Instance of an appropriate parser </returns>
        /// <exception cref="System.ArgumentException">This is thrown
        /// when the specified file extension isn't among the allowed
        /// extensions
        /// </exception>
        public static Parser GetContextParser(string filePath)
        {
            string fileExtension = Path.GetExtension(filePath).ToLower();

            switch (fileExtension)
            {
                case ".xml":
                    return new XmlParser(filePath);
                case ".html":
                    return new HtmlParser(filePath);
                case ".txt":
                    return new TxtParser(filePath);
                case ".pdf":
                    return new PDFParser(filePath);
                case ".doc":
                case ".docx":
                    return new DocParser(filePath);
                case ".ppt":
                case ".pptx":
                    return new PresentationParser(filePath);
                case ".xls":
                case ".xlsx":
                    return new SheetParser(filePath);
                default:
                    throw new ArgumentException("Unrecognized file type");
            }
        }
    }

    /// <summary>
    /// General class for all parsers to extract semantic text
    /// from files  
    /// </summary>
    public abstract class Parser
    {
        // Path of file to extract text from
        protected string filePath;

        public Parser(string filePath)
        {
            this.filePath = filePath;
        }

        /// <summary>
        /// Parses semantic text out of file
        /// </summary>
        /// <returns>Semantic text in file</returns>
        public abstract string Parse();
    }

    /// <summary>
    /// Class to parse text from .txt files
    /// </summary>
    public class TxtParser : Parser
    {
        public TxtParser(string filePath) : base(filePath) { }

        public override string Parse()
        {
            string content = File.ReadAllText(filePath);
            return content;
        }
    }

    /// <summary>
    /// Class to parse semantic content out of .ppt and .ppts files
    /// </summary>
    public class PresentationParser : Parser
    {
        private Presentation pptDocument;
        public PresentationParser(string filePath) : base(filePath)
        {
            pptDocument = openFile(filePath);
        }

        private Presentation openFile(string path_to_file)
        {
            Presentation ppt = new Presentation();
            ppt.LoadFromFile(path_to_file);
            return ppt;
        }
        public override string Parse()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < pptDocument.Slides.Count; i++)
            {
                for (int j = 0; j < pptDocument.Slides[i].Shapes.Count; j++)
                {
                    if (pptDocument.Slides[i].Shapes[j] is IAutoShape)
                    {
                        IAutoShape shape = pptDocument.Slides[i].Shapes[j] as IAutoShape;
                        if (shape.TextFrame != null)
                        {
                            foreach (TextParagraph tp in shape.TextFrame.Paragraphs)
                            {
                                sb.Append(tp.Text + " ");
                            }
                        }
                    }
                }
            }

            return sb.ToString();
        }
    }

    /// <summary>
    /// Class to parse semantic content out of an xml file
    /// </summary>
    public class XmlParser : Parser
    {
        public XmlParser(string filePath) : base(filePath) { }

        public override string Parse()
        {
            XElement xmlElement = XElement.Load(this.filePath);
            IEnumerable<XNode> NodesList = xmlElement.DescendantNodes();
            StringBuilder sb = new StringBuilder("", NodesList.Count());

            foreach (XNode node in NodesList)
            {
                if (node.NodeType == System.Xml.XmlNodeType.Text)
                {
                    sb.Append(node.ToString() + " ");
                }
            }

            return sb.ToString().TrimEnd();
        }
    }

    /// <summary>
    /// Class to parse semantic content out of an html file
    /// </summary>
    public class HtmlParser : Parser
    {
        public HtmlParser(string filePath) : base(filePath) { }

        public override string Parse()
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.Load(filePath);

            var node = htmlDoc.DocumentNode.SelectSingleNode("//body");

            StringBuilder sb = new StringBuilder("", node.Descendants().Count());

            //             foreach (var element in body.QuerySelectorAll("script"))
            //             {
            //                 element.Remove();
            //             }

            foreach (var nNode in node.Descendants())
            {
                if (nNode.NodeType == HtmlNodeType.Element)
                {
                    sb.Append(nNode.GetDirectInnerText().Trim() + " ");
                }
            }

            return sb.ToString().TrimEnd();
        }
    }

    /// <summary>
    /// Class to parse semantic content out of a pdf file
    /// </summary>
    public class PDFParser : Parser
    {
        public PDFParser(string filePath) : base(filePath) { }

        public override string Parse()
        {
            StringBuilder sb = new StringBuilder("", 10);

            using (PdfDocument document = PdfDocument.Open(this.filePath))
                foreach (Page page in document.GetPages())
                {
                    string pageText = page.Text;

                    foreach (Word word in page.GetWords())
                    {
                        sb.Append(word.Text + " ");
                    }
                }

            return sb.ToString().TrimEnd();
        }
    }

    /// <summary>
    /// Class to parse semantic content out of a .doc or .docx file
    /// </summary>
    public class DocParser : Parser
    {
        public DocParser(string filePath) : base(filePath) { }

        public override string Parse()
        {
            Document document = new Document();
            document.LoadFromFile(this.filePath);

            // remove embedded spire evaluation warning from
            // first line of text
            IEnumerable<string> lines = Regex.Split(document.GetText(), "\r\n|\r|\n").Skip(1);

            return string.Join(Environment.NewLine, lines.ToArray());
        }
    }


    /// <summary>
    /// Class to parse semantic text out of .xls and .xlsx files
    /// </summary>
    public class SheetParser : Parser
    {
        private Workbook workbook;

        public SheetParser(string path_to_file) : base(path_to_file)
        {
            workbook = openFile(path_to_file);
        }
        private Workbook openFile(string path_to_file)
        {
            var wb = new Workbook();
            wb.LoadFromFile(path_to_file);
            return wb;
        }

        public override string Parse()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < workbook.Worksheets.Count; i++)
            {
                Worksheet sheet = workbook.Worksheets[i];
                if (sheet.IsEmpty)
                {
                    continue;
                }
                GetSheetCellText(sb, sheet);

                GetSheetCommentText(sb, sheet);

                GetSheetShapeText(sb, sheet);

                for (int j = 0; j < sheet.TextBoxes.Count; j++)
                {
                    sb.AppendLine(sheet.TextBoxes[j].Text);
                }

            }
            return sb.ToString().Trim();
        }

        private void GetSheetCellText(StringBuilder sb, Worksheet sheet)
        {
            foreach (var cell in sheet.Cells)
            {
                if (!string.IsNullOrEmpty(cell.Text))
                {
                    sb.Append(cell.Text + " ");

                }
                else if (!string.IsNullOrEmpty(cell.NumberText))
                {
                    sb.Append(cell.NumberText + " ");
                }

            }
            sb.AppendLine();
        }

        private void GetSheetShapeText(StringBuilder sb, Worksheet sheet)
        {
            foreach (var shp in sheet.PrstGeomShapes)
            {
                sb.AppendLine(shp.Text);
            }
        }

        private static void GetSheetCommentText(StringBuilder sb, Worksheet sheet)
        {
            foreach (var commt in sheet.Comments)
            {
                sb.AppendLine(commt.Text);
            }
        }

    }

}


