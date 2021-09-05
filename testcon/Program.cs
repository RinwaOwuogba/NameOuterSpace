using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

using SearchEngine;

namespace testcon
{
    class Program
    {
        static void Main(string[] args)
        {
            //     var doc = new PdfParser("../repository/regis.pdf");
            //     string text = doc.Text;
            //     Console.WriteLine(text);

            //     var doc1 = new PdfParser("../repository/rere.pdf");
            //     Console.WriteLine(doc1.Text);

            //     var doc2 = new DocParser("../repository/sample.docx");
            //     Console.WriteLine(doc2.Text);

            //     var doc3 = new PresentationParser("../repository/sharp.pptx");
            //     Console.WriteLine(doc3.Text);

            //     var doc4 = new SheetParser("../repository/testsheet2.xlsx");
            //   Console.WriteLine(doc4.Text);
            var engine = new Engine("../repository/");

            var watch = new Watcher(engine);
            var querier = new Querier(engine);
            Watcher.IndexInBackGround(watch);
            Stopwatch stopwatch = new Stopwatch();

            // foreach( var x in watch.getValidFilesFromRepo(meta.repositoryPath)){
            //     Console.WriteLine(x);
            // }


            while (true)
            {

                // Console.WriteLine(engine.GetWordDocument("june"));
                // Console.WriteLine(engine.GetWordDocument("rebecca"));
                // Console.WriteLine(engine.GetWordDocument("clown"));
                // Console.WriteLine(engine.GetWordDocument("microsoft"));
                // Console.WriteLine(engine.GetWordDocument("random"));
                // Console.WriteLine(engine.GetDocument(1));

                //     var sofar = "op";
                //    // foreach (var c in "may")
                //     //{
                //         foreach(var x in querier.GetCompletions(sofar)){
                //                 Console.WriteLine(x);
                //             }
                //    // }

                Console.WriteLine(engine.GetWordDocument("this is a simple sentence"));
                stopwatch.Start();
                Console.WriteLine("got here, while");
                List<DocumentResultModel> d = querier.Query("mina");
                stopwatch.Stop();

                Console.WriteLine("Elapsed Time is {0} ms", stopwatch.ElapsedMilliseconds);
                foreach (DocumentResultModel x in d)
                {
                    Console.WriteLine(x.FilePath + "  " + x.DocumentRank + "");
                }
                // string s = "";
                // // foreach (var x in engine.GetAllWords())
                //     {
                //         //Console.WriteLine(x.Filename + "  " + x.Id);
                //         s += x + "\n";
                //     }
                // File.WriteAllText("showthem.txt", s);
                break;
                Console.WriteLine("-----------------------------------------");
                Thread.Sleep(5000);

            }
        }

        static void p(object something)
        {
            Console.WriteLine(something);
        }

        static void pl<t>(List<t> l)
        {
            foreach (var o in l)
            {
                p(o);
            }
        }


    }
}
