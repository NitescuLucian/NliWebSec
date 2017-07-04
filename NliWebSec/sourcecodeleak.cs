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
            HttpWebResponse response1 = null;
            string html = null;
            string html1 = null;
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
/*
            if (lvl > 1)
            {
                for (int i = 0; i < lvl; i++)
                {
                    for (int j = 0; j < mytargets.Count; j++)
                    {
                        string url1 = mytargets[j];
                        if (url1 != null)
                        {
                            // reading the html response from the target
                            HttpWebRequest request1 = (HttpWebRequest)WebRequest.Create(url1);
                            if (request1 != null)
                            {
                                try
                                {
                                    response1 = (HttpWebResponse)request1.GetResponse();
                                }
                                catch (System.Net.WebException ex)
                                {
                                    Debug.WriteLine("Exception Message: " + ex.Message);
                                }
                                if (response1 != null)
                                {
                                    if (response1.StatusCode == HttpStatusCode.OK)
                                    {
                                        Stream receiveStream1 = response1.GetResponseStream();
                                        StreamReader readStream1 = null;

                                        if (response1.CharacterSet == null)
                                        {
                                            readStream1 = new StreamReader(receiveStream1);
                                        }
                                        else
                                        {
                                            readStream1 = new StreamReader(receiveStream1, Encoding.GetEncoding(response1.CharacterSet));
                                        }

                                        html1 = readStream1.ReadToEnd();

                                        response1.Close();
                                        readStream1.Close();
                                    }
                                }
                            }
                        }
                        //searching for urls / paths in the html response of the target
                        if (html1 != null)
                        {
                            foreach (Match item1 in Regex.Matches(html1, @"(http|ftp|https):\/\/([\w\-_]+(?:(?:\.[\w\-_]+)+))([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?"))
                            {
                                string draft1 = item1.Value;
                                //the role of this if is to filter false positives and 3rd parties resources from crawl
                                if (draft1.StartsWith(url))
                                {
                                    mytargets.Add(item1.Value);
                                }
                            }
                        }

                    }

                }
            }
*/
            mytargets.ToArray();

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

