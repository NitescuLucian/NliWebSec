using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NliWebSec
{
    class sourcecodeleak
    {
        static void Main(string[] args)
        {
            List<string> mytargets = new List<string>();

            string html = null;
            Console.WriteLine("Please enter the root url of your target: ");
            string url = Console.ReadLine();
            Console.WriteLine("Please enter the level: ");
            int lvl = 0;
            string line = Console.ReadLine();
            try
            {
                lvl = Int32.Parse(line);
            }
            catch (FormatException)
            {
                Console.WriteLine("\"{0}\" is not an integer! Performing one scan on one level!", line);
            }
            // reading the html response from the target
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }

                html = readStream.ReadToEnd();

                response.Close();
                readStream.Close();
            }
            //searching for urls / paths in the html response of the target
            foreach (Match item in Regex.Matches(html, @"(http|ftp|https):\/\/([\w\-_]+(?:(?:\.[\w\-_]+)+))([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?"))
            {
                string draft = item.Value;
                //the role of this if is to filter false positives and 3rd parties resources from crawl
                if (draft.StartsWith(url)) { 
                    mytargets.Add(item.Value);
                }
            }
            //this is how I set the depth of my scan
            mytargets.ToArray();
            if (lvl > 1)
            {
                for (int i = 0; i < lvl; i++)
                {
                    foreach (string item in mytargets)
                    {
                        url = item;
                        HttpWebRequest request1 = (HttpWebRequest)WebRequest.Create(url);
                        HttpWebResponse response1 = (HttpWebResponse)request1.GetResponse();
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            Stream receiveStream = response1.GetResponseStream();
                            StreamReader readStream = null;

                            if (response.CharacterSet == null)
                            {
                                readStream = new StreamReader(receiveStream);
                            }
                            else
                            {
                                readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                            }

                            html = readStream.ReadToEnd();

                            response.Close();
                            readStream.Close();
                        }
                        //searching for urls / paths in the html response of the target
                        foreach (Match item1 in Regex.Matches(html, @"(http|ftp|https):\/\/([\w\-_]+(?:(?:\.[\w\-_]+)+))([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?"))
                        {
                            string draft = item1.Value;
                            //the role of this if is to filter false positives and 3rd parties resources from crawl
                            if (draft.StartsWith(url))
                            {
                                mytargets.Add(item1.Value);
                            }
                        }
                    }
                }
            }
           
            for (int i = 0; i < mytargets.Count(); i++)
            {
                Console.WriteLine(mytargets[i]);
            }
            Console.WriteLine("At this point we found out: " + mytargets.Count() + " urls");
            Console.WriteLine();
            Console.ReadLine();
        }
    }
}
