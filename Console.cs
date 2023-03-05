/*
 *
 * This was made in a windows console application, if you wish to copy paste it, make sure you got all the libraries i have.
 * 
 * 
 * DISCLAIMER: I used https://www.france24.com/fr/rss as my rss source, from what i noticed, not all rss websites have the same html structure.
 * That's why i added "SearchTagInfo" function, so you can customize your tags without needing to recode all the structure.
 * 
 * Made it in 1 day because i needed ideas to show my scripting skills for uni, even thought i'm mainly on LUA, C# isn't bad at all.
 * 
 */

using System;
using System.Net;
using System.IO;
using Aspose.Html;
// HTML Library
using System.Collections.Generic;

namespace CS_Training
{
    
    class Program
    { 

        private static string ReadWebsite(string RSS_URL, WebClient web)
        {
            Stream stream = web.OpenRead(RSS_URL);
            // Opens a webclient in the url we've set
            using (StreamReader reader = new StreamReader(stream))
            {
                Console.WriteLine("Info from: " + RSS_URL + " Successfully Recovered");
                return reader.ReadToEnd();
                // Returns the HTML code of the website
            }
        }
        // Turns a URL into HTML Code
        private static string SearchTagInfo(string HTML_Code, string HTML_Tag)
        {
            if (HTML_Code.Contains("<" + HTML_Tag + ">")) // Searches for the chosen tag in the code
             { 
                string TagInfo;

                string[] TagTable = HTML_Code.Split(new string[] { "<" + HTML_Tag + ">", "</" + HTML_Tag + ">" }, StringSplitOptions.RemoveEmptyEntries);
                // Once the tag is found, he splits the code in multiple parts.

                TagInfo = TagTable[0]; 
                // I set the default part as the 1st, because the category often is on the first part of the code.

                if (TagInfo.Contains("</") && TagTable.Length > 1) // If he still finds tags (which is normal, if the chosen tag isn't the first one)
                {
                    TagInfo = TagTable[1]; 
                    // He sets the tag information as the 2nd half of the table (the part where the info is located)
                }else if (TagInfo.Contains("<title>")) // incase theres any bug where the category is the title
                {
                    TagInfo = null; 
                    // We set the taginfo to null, so the client doesn't have to see a bunch of text which were a result of an error. [TO-FIX]
                }

                Console.WriteLine("\n"+ HTML_Tag + ": \n"+ TagInfo); 
                // We send the info the console
                return TagInfo; // We return the Tag
            }else
            {
                Console.WriteLine("no " + HTML_Tag); // Incase the tag doesn't exist
                return null; // We send Null.
            }
        }

        static void Main() // Main Function of the program
        {
            WebClient web = new WebClient(); 
            // We create a web client to read the website

            string Website = "https://www.france24.com/fr/rss"; 
            // Your Website Target
            string HTML_Code = ReadWebsite(Website, web); 
            // Recover the HTML Code of the Target

            string[] Items = HTML_Code.Split(new string[] { "<item>", "</item>" }, StringSplitOptions.RemoveEmptyEntries); 
            // You split the HTML code into multiple parts, which are the "Items
            List<string> ItemList = new List<string>(Items); 
            // The different Contents inside our code.

            Console.WriteLine("Found :" + (ItemList.Count - 1) + " Articles."); 
            // We write how much Articles we've found, - 1 because one of them is the end of the HTML code.

            string outFile = @"D:\Downloads\RSS_Organizer.html"; 
            // Our HTML file Path

            using (var htmldocument = new HTMLDocument()) // We create a new html document
            {
                var Body = htmldocument.Body; 
               
            //-------------------------------------------------------
                for (int i = 0; i < ItemList.Count; i++) // We loop through our amount of 
                {
                    if (i == 0) // if it's the Title, the first element of our code
                    { 
                        Console.WriteLine("This is the Title:" + "\n" + ItemList[i]);
                        continue;
                    }
                    else if (i == (ItemList.Count - 1)) // It's the end of the table 
                    {
                        Console.WriteLine("This is the end:" + "\n" + ItemList[i]);
                        htmldocument.Save(outFile);
                        return;
                    }
                    else // It's an item

                    Console.WriteLine("This is the Article n°" + i);

                    string ArticleHTML_Code = ItemList[i];// we get our Article's HTML Code

                    Dictionary<string, string> Properties = new Dictionary<string, string>(); // We create a dictionary with all the tags we need
                    Properties.Add("Category", SearchTagInfo(ArticleHTML_Code, "category"));
                    Properties.Add("Description", SearchTagInfo(ArticleHTML_Code, "description"));
                    Properties.Add("Title", SearchTagInfo(ArticleHTML_Code, "title"));
                    Properties.Add("Link", SearchTagInfo(ArticleHTML_Code, "link"));


                    foreach (KeyValuePair<string,string> Property in Properties) // We loop through the dictionary in order to code it into the website
                    {
                        if (Property.Key == "Link") { continue;} // We skip the links because they will be in the title (href)

                        if (Property.Key == "Title")
                        {
                            var Anchor = (HTMLAnchorElement)htmldocument.CreateElement("a");
                            Anchor.SetAttribute("href", Properties["Link"]);
                            var Text = htmldocument.CreateTextNode(Property.Value);
                            Anchor.AppendChild(Text);
                            // We code our anchor element with the link and text
                            Body.AppendChild(Anchor);
                            // We add it into the body
                        }
                        else if (Property.Key == "Category")
                        {
                            var Heading2 = (HTMLHeadingElement)htmldocument.CreateElement("h4");
                            var text = htmldocument.CreateTextNode(Property.Value);
                            Heading2.AppendChild(text);
                            Body.AppendChild(Heading2);
                            // We create an heading and add it into the body
                        }
                        else
                        {
                            var p = (HTMLParagraphElement)htmldocument.CreateElement("p");
                            var text = htmldocument.CreateTextNode(Property.Value);
                            p.AppendChild(text);
                            Body.AppendChild(p);
                            // for all the rest (description) we do a simple paragraph.
                        }
                    }

                }
            }
        }
    }
}
// Thank you for checking!
