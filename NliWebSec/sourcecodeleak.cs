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
            Console.WriteLine("Please enter the url of your target: ");
            string url = Console.ReadLine();
            /*Console.WriteLine("Please enter the level: ");
            int lvl = Convert.ToInt32(Console.ReadLine());

            for (int i = 0; i < lvl; i++)
            {

            }*/
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
                mytargets.Add(item.Value);
            }
            mytargets.ToArray();
            for (int i = 0; i < mytargets.Count(); i++)
            {
                Console.WriteLine(mytargets[i]);
            }
            Console.WriteLine(mytargets);

            Console.WriteLine();
            Console.ReadLine();
        }
    }
}
