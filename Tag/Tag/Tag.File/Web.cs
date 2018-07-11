using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using HtmlAgilityPack;

namespace Tag.Utility
{
    public class Web
    {
        public static HtmlDocument GetResponse(string url, string encoding)
        {
            try
            {
                StringBuilder htmlResponse = new StringBuilder();

                Uri uriLocation = new Uri(url);

                HttpWebRequest amgRequest = (HttpWebRequest)WebRequest.Create(uriLocation);
                Stream webresponse = ((HttpWebResponse)amgRequest.GetResponse()).GetResponseStream();

                StreamReader reader = new StreamReader(webresponse, Encoding.GetEncoding(encoding));

                while (!reader.EndOfStream)
                {
                    htmlResponse.Append(reader.ReadLine());
                }

                byte[] html = Encoding.GetEncoding(encoding).GetBytes(htmlResponse.ToString());

                MemoryStream ms = new MemoryStream(html);

                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.Load(ms);

                return htmlDoc;
            }
            catch
            {
                return new HtmlDocument();
            }
        }

        public static Image GetImage(string url)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

            return Image.FromStream(webResponse.GetResponseStream());
        }
    }
}
