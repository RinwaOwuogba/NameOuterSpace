using System;
using System.Text;
using System.Linq;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace SearchEngine
{   
    public class PdfParser
    {
        private PdfDocument document;
        private string text;
        public PdfParser(string path_to_file){
            document = openFile(path_to_file);
        }

        public string Text { get { return transformToString(); } private set => text = value; }

        // the general method should open as bytes
        public PdfDocument openFile(string path_to_file){
            PdfDocument document = PdfDocument.Open(path_to_file);
            return document;
        }

        private string transformToString(){
            StringBuilder alltext = new StringBuilder();
            var pages = document.GetPages();
            foreach (Page page in pages){
                 foreach (Word word in page.GetWords())
                {
                    alltext.Append(word.Text + " ");
                }
            }
            return alltext.ToString();
        }
        
    }

}
