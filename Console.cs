using System;
using System.Net;
using System.IO;
// Add reference to Aspose.HTML for .NET API
// Use following namespace to create HTML file
using Aspose.Html;
using System.Collections.Generic;

namespace CS_Training
{
     
    class Program
    {
        private static string ReadWebsite(string RSS_URL, WebClient web)
        {
            Stream stream = web.OpenRead(RSS_URL);

            using (StreamReader reader = new StreamReader(stream))
            {
                Console.WriteLine("Info from: " + RSS_URL + " Successfully Recovered");
                return reader.ReadToEnd();
            }
        }
        private static void SearchTagInfo(string HTML_Code, string HTML_Tag)
        {
            if (HTML_Code.Contains("<" + HTML_Tag + ">"))
             { 
                string TagInfo;

                string[] TagTable = HTML_Code.Split(new string[] { "<" + HTML_Tag + ">", "</" + HTML_Tag + ">" }, StringSplitOptions.RemoveEmptyEntries);

                TagInfo = TagTable[0];

                if (TagInfo.Contains("</") && TagTable.Length > 1)
                {
                    TagInfo = TagTable[1];
                }

                Console.WriteLine("\n"+ HTML_Tag + ": \n"+ TagInfo);
            }else
            {
                Console.WriteLine("no " + HTML_Tag);
            }
        }

        private static void FilterItem(string HTML_Code)
        {
            SearchTagInfo(HTML_Code, "category");
            SearchTagInfo(HTML_Code, "title");
            SearchTagInfo(HTML_Code, "link");
            SearchTagInfo(HTML_Code, "description");
        }

        static void Main()
        {
            WebClient web = new WebClient();

            string HTML_Code = ReadWebsite("https://www.france24.com/fr/rss", web);

            string[] Items = HTML_Code.Split(new string[] { "<item>", "</item>" }, StringSplitOptions.RemoveEmptyEntries);

            List<string> ItemList = new List<string>(Items);

            int ItemIndex = 0;

            foreach (var i in ItemList)
            {
                ItemIndex++;
            }

            Console.WriteLine("Found :" + (ItemIndex-1) + " Items.");

            for (int i = 0; i < ItemIndex; i++)
            {
                if (i == 0) // if it's the Title
                {

                    Console.WriteLine("This is the Title:" + "\n" + ItemList[i]);
                    continue;
                }
                else if (i == (ItemIndex - 1)) // It's the end of the table 
                {
                    Console.WriteLine("This is the end:" + "\n" + ItemList[i]);
                    return;
                }
                else // It's an item
                    
                    Console.WriteLine("This is the item n°" + i);
                    FilterItem(ItemList[i]);
            }

        }

        // string outFile = @"D:\Downloads\DJ_Will_Get_No_License.html";

        // Initialize an empty HTML document
        // using (var htmldocument = new HTMLDocument(HTML_Code,"."))
        //{ 
        // Save the HTML file to a disk
        // htmldocument.Save(outFile);
        //}
    }
}
