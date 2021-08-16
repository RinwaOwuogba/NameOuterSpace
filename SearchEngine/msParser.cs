using System.IO;
using System.Diagnostics;
using System.Text;

using Spire.Doc;
using Spire.Presentation;


namespace SearchEngine
{ 
    public class DocParser
    {
        private Document document;
        private string text;
        public DocParser(string path_to_file){
            document = openFile(path_to_file);
        }

        private Document openFile(string path_to_file){
            Document doc = new Document();
            doc.LoadFromFile(path_to_file);
            return doc;
        }
        private string transformToString(){
            return document.GetText();
        }

        public string Text { get => transformToString();}
    }

    public class PresentationParser{
        private Presentation pptDocument;
        private string text;
        public PresentationParser(string path_to_file){
            pptDocument = openFile(path_to_file);
        }

        private Presentation openFile(string path_to_file){
            Presentation ppt = new Presentation();
            ppt.LoadFromFile(path_to_file);
            return ppt;
        }
        private string transformToString()
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
        public string Text { get => transformToString();}
    }
}