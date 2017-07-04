using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NliWebSec
{
    class sourcecodeleak
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please enter the url of your target: ");
            string url = Console.ReadLine();
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

                string html = readStream.ReadToEnd();
                
                response.Close();
                readStream.Close();
            }
            //searching for urls / paths in the html response of the target

            Console.ReadLine();
        }
    }
}
