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
    /// Class to extract semantic text from files
    /// </summary>
    public class Parser
    {
        // Path of file to extract text from
        private string filePath;

        // Extension of file to extract text from
        private string fileExtension;

        public Parser(string filePath)
        {
            this.filePath = filePath;
            this.fileExtension = Path.GetExtension(this.filePath).ToLower();
        }

        /// <summary>
        /// Automatically uses the appropriate parser to
        /// parse a file
        /// </summary>
        /// <returns>Semantic text extracted from the file</returns>
        /// <exception cref="System.ArgumentException">This is thrown
        /// when the specified file extension isn't among the allowed
        /// extensions
        /// </exception>
        public string AutoDetectParse()
        {
            switch (this.fileExtension)
            {
                case ".xml":
                    return this.ParseXml();
                case ".html":
                    return this.ParseHtml();
                case ".txt":
                    string content = File.ReadAllText(filePath);
                    return content;
                case ".pdf":
                    return this.ParsePdf();
                case ".doc":
                case ".docx":
                    return this.ParseDoc();
                case ".ppt":
                case ".ppts":
                    return this.ParsePresentation();
                case ".xlx":
                case ".xls":
                    SheetParser sheetParser = new SheetParser(this.filePath);
                    return sheetParser.Text;
                default:
                    throw new ArgumentException("Unrecognized file type");
            }
        }

        /// <summary>
        /// Parse semantic content out of an xml file
        /// </summary>
        /// <returns>Semantic text extracted from xml file</returns>
        private string ParseXml()
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

        /// <summary>
        /// Parse semantic content out of an html file
        /// </summary>
        /// <returns>Semantic text extracted file</returns>
        private string ParseHtml()
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.Load(filePath);

            var node = htmlDoc.DocumentNode.SelectSingleNode("//body");

            StringBuilder sb = new StringBuilder("", node.Descendants().Count());

            foreach (var nNode in node.Descendants())
            {
                if (nNode.NodeType == HtmlNodeType.Element)
                {
                    sb.Append(nNode.GetDirectInnerText().Trim() + " ");
                }
            }

            return sb.ToString().TrimEnd();
        }

        /// <summary>
        /// Parse semantic content out of a pdf file
        /// </summary>
        /// <returns>Semantic text extracted from file</returns>
        private string ParsePdf()
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

        /// <summary>
        /// Parse semantic content out of a .doc or .docx file
        /// </summary>
        /// <returns>Semantic text extracted from file</returns>
        public string ParseDoc()
        {
            Document document = new Document();
            document.LoadFromFile(this.filePath);

            // remove embedded spire evaluation warning from
            // first line of text
            IEnumerable<string> lines = Regex.Split(document.GetText(), "\r\n|\r|\n").Skip(1);

            return string.Join(Environment.NewLine, lines.ToArray());
        }

        /// <summary>
        /// Parse semantic content out of .ppt and .ppts files
        /// </summary>
        /// <returns>Semantic text extracted from file</returns>
        public string ParsePresentation()
        {
            Presentation pptDocument = new Presentation();
            pptDocument.LoadFromFile(this.filePath);

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
    /// Class to parse semantic text out of .xls and .xlsx files
    /// </summary>
    public class SheetParser
    {
        private Workbook workbook;
        private string text;

        public SheetParser(string path_to_file)
        {
            workbook = openFile(path_to_file);
        }
        private Workbook openFile(string path_to_file)
        {
            var wb = new Workbook();
            wb.LoadFromFile(path_to_file);
            return wb;
        }

        private string transformToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < workbook.Worksheets.Count; i++)
            {
                Worksheet sheet = workbook.Worksheets[i];
                if (sheet.IsEmpty)
                {
                    continue;
                }
                sb.AppendLine(string.Format("The cell text in worksheet{0} are as follows:", i.ToString()));
                GetSheetCellText(sb, sheet);

                sb.AppendLine(string.Format("The comment text in worksheet{0} are as follows:", i.ToString()));
                GetSheetCommentText(sb, sheet);

                sb.AppendLine(string.Format("The shape text in worksheet{0} are as follows:", i.ToString()));
                GetSheetShapeText(sb, sheet);

                sb.AppendLine(string.Format("The textbox text in worksheet{0} are as follows:", i.ToString()));
                for (int j = 0; j < sheet.TextBoxes.Count; j++)
                {
                    sb.AppendLine(sheet.TextBoxes[j].Text);
                }

            }
            return sb.ToString();
        }
        public string Text { get => transformToString(); }

        private void GetSheetCellText(StringBuilder sb, Worksheet sheet)
        {
            foreach (var cell in sheet.Cells)
            {
                if (!string.IsNullOrEmpty(cell.Text) || !string.IsNullOrEmpty(cell.NumberText))
                {
                    sb.Append(cell.Text + " ");
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