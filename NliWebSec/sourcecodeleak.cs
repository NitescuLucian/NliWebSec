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
        //the function
        public static void stringator (string baseurl, string testaddition, List<string> restatus)
        {
            HttpWebResponse response2 = null;
            string testfin = testaddition;
            string urlcha = baseurl + testfin;
            Console.WriteLine(urlcha + " ");
            HttpWebRequest request1 = (HttpWebRequest)WebRequest.Create(urlcha);
            request1.Timeout = 1000;
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
                    restatus.Add("URL: " + urlcha + " HTTP Status Code:" + (int)response2.StatusCode);
                }
            }
            Console.WriteLine();
            //Console.ReadLine();
        }

        static void Main(string[] args)
        {
            List<string> mytargets = new List<string>();
            HttpWebResponse response = null;
            HttpWebResponse response2 = null;
            string html = null;
            Console.WriteLine("Please enter the root url of your target: ");
            string url = Console.ReadLine();
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
                request1.Timeout = 1000;
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
                //Console.ReadLine();
            }
            //here I will read several files and put them in a string array for further tests
            string line;
            List<string> urlman = new List<string>();
            // Read the file and display it line by line.  
            System.IO.StreamReader file = new System.IO.StreamReader(@"../../urlman.txt");
            while ((line = file.ReadLine()) != null)
            {
                urlman.Add(line);
            }
            urlman.ToArray();
            file.Close();
            // end of read
            int testno = 0;
            List<string> restatus = new List<string>();
            for (int j = 0; j < urlman.Count(); j++)
            {
                for (int i = 0; i < mytargets.Count(); i++)
                {
                    string kill = urlman[j];
                    stringator(mytargets[i], kill, restatus);
                    testno++;
                }
            }
            restatus.ToArray();
            Console.WriteLine("All tests have been performed. Count: " + testno + ". ");
            for (int j = 0; j < restatus.Count(); j++)
            {
                Console.WriteLine(restatus[j]);
            }
        }
    }
}


