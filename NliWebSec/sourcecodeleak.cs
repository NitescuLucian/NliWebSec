using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            HttpWebResponse response = null;
            HttpWebResponse response2 = null;
            string html = null;
            Console.WriteLine("Please enter the root url of your target: ");
            string url = Console.ReadLine();
           // Console.WriteLine("Please enter the level: ");
           // int lvl = 0;
            //string line = Console.ReadLine();
           /* try
            {
                lvl = Int32.Parse(line);
            }
            catch (FormatException)
            {
                Console.WriteLine("\"{0}\" is not an integer! Performing one scan on one level!", line);
            }*/
            // reading the html response from the target
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (System.Net.WebException ex)
            {
                Debug.WriteLine("Exception Message: " + ex.Message);
            }
            if (response != null)
            {
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
            }
            //searching for urls / paths in the html response of the target
            foreach (Match item in Regex.Matches(html, @"(http|ftp|https):\/\/([\w\-_]+(?:(?:\.[\w\-_]+)+))([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?"))
            {
                string draft = item.Value;
                //the role of this if is to filter false positives and 3rd parties resources from crawl
                if (draft.StartsWith(url))
                {
                    mytargets.Add(item.Value);
                }
            }
            mytargets.ToArray();
            Console.WriteLine("At this point we found out: " + mytargets.Count() + " urls");
            for (int i = 0; i < mytargets.Count(); i++)
            {
                Console.WriteLine(mytargets[i] + " ");
                HttpWebRequest request1 = (HttpWebRequest)WebRequest.Create(mytargets[i]);
                request1.Timeout = 400;
                if (request1 != null)
                {
                    try
                    {
                        response2 = (HttpWebResponse)request1.GetResponse();
                    }
                    catch (System.Net.WebException ex)
                    {
                        Debug.WriteLine("Exception Message: " + ex.Message);
                    }
                    if (response2 != null)
                    {
                        Console.Write((int)response2.StatusCode);
                    }
                }
                
                Console.WriteLine();
                Console.ReadLine();
            }
        }
    }
}

