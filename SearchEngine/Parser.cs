using System;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using AngleSharp;

namespace SearchEngine
{
    public class Parser
    {
        static async Task ReadHtml()
        {
            var source = @"
<!DOCTYPE html>
<html lang=en>
  <meta charset=utf-8>
  <meta name=viewport content=""initial-scale=1, minimum-scale=1, width=device-width"">
  <title>Error 404 (Not Found)!!1</title>
  <style>
    *{margin:0;padding:0}html,code{font:15px/22px arial,sans-serif}html{background:#fff;color:#222;padding:15px}body{margin:7% auto 0;max-width:390px;min-height:180px;padding:30px 0 15px}* > body{background:url(//www.google.com/images/errors/robot.png) 100% 5px no-repeat;padding-right:205px}p{margin:11px 0 22px;overflow:hidden}ins{color:#777;text-decoration:none}a img{border:0}@media screen and (max-width:772px){body{background:none;margin-top:0;max-width:none;padding-right:0}}#logo{background:url(//www.google.com/images/errors/logo_sm_2.png) no-repeat}@media only screen and (min-resolution:192dpi){#logo{background:url(//www.google.com/images/errors/logo_sm_2_hr.png) no-repeat 0% 0%/100% 100%;-moz-border-image:url(//www.google.com/images/errors/logo_sm_2_hr.png) 0}}@media only screen and (-webkit-min-device-pixel-ratio:2){#logo{background:url(//www.google.com/images/errors/logo_sm_2_hr.png) no-repeat;-webkit-background-size:100% 100%}}#logo{display:inline-block;height:55px;width:150px}
  </style>
  <a href=//www.google.com/><span id=logo aria-label=Google></span></a>
  <p><b>404.</b> <ins>That’s an error.</ins>
  <p>The requested URL <code>/error</code> was not found on this server.  <ins>That’s all we know.</ins>";


            string text = System.IO.File.ReadAllText(@"/home/bolarinwa/Programming/C#/SearchEngine/src/article.html");

            // Display the file contents to the console. Variable text is a string.
            // System.Console.WriteLine("Contents of WriteText.txt = {0}", text);

            //Use the default configuration for AngleSharp
            var config = Configuration.Default;

            //Create a new context for evaluating webpages with the given config
            var context = BrowsingContext.New(config);

            //Just get the DOM representation
            var document = await context.OpenAsync(req => req.Content(text));
            // var document = await context.OpenAsync(req => req.Content(source));

            // Serialize it back to the console
            // Console.WriteLine(document.DocumentElement.TextContent);

            var body = document.QuerySelector("body");

            foreach (var element in body.QuerySelectorAll("script"))
            {
                element.Remove();
            }

            Console.WriteLine(body.TextContent);
        }

        static async Task ReadXML()
        {
            // Load the XML file from our project directory containing the purchase orders
            var filename = "books.xml";
            var currentDirectory = Directory.GetCurrentDirectory();
            var booksFilepath = Path.Combine(currentDirectory, "../files/" + filename);

            XElement books = XElement.Load(booksFilepath);
            IEnumerable<XNode> NodesList = books.DescendantNodes();
            StringBuilder sb = new StringBuilder("", NodesList.Count());

            foreach (XNode element in NodesList)
            {
                if (element.NodeType == System.Xml.XmlNodeType.Text)
                {
                    sb.Append(element.ToString() + "\n");
                }
            }

            Console.WriteLine("sb: \n" + sb.ToString());
        }


        // static async Task Main(string[] args)
        // // static void Main(string[] args)
        // {
        //     // Console.WriteLine("Hello World!");
        //     // await ReadHtml();
        //     await ReadXML();

        // }
    }
}
