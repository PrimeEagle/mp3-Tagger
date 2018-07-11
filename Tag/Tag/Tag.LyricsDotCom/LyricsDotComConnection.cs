using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using Tag.Utility;
using HtmlAgilityPack;

namespace Tag.LyricsDotCom
{
    public class LyricsDotComConnection
    {
        const string LYRICS_URL = @"http://www.lyrics.com/result.php?Keyword=[ARTIST]&TypeOfSearch=1";
        const string LYRICS_BASE_URL = @"http://www.lyrics.com/";
        const string ENCODING = "UTF-8";

        public string GetLyrics(string artist, string album, string song)
        {
            string lyrics = null;

            lyrics = SearchForLyrics(artist, album, song, ComparisonType.Exact);

            if (lyrics == null)
            {
                lyrics = SearchForLyrics(artist, album, song, ComparisonType.EndsWith);
            }

            if (lyrics == null)
            {
                lyrics = SearchForLyrics(artist, album, song, ComparisonType.StartsWith);
            }

            if (lyrics == null)
            {
                lyrics = SearchForLyrics(artist, album, song, ComparisonType.Exact);
            }

            return lyrics;
        }
        
        
        
        private string SearchForLyrics(string artist, string album, string song, ComparisonType matchType)
        {
            string lyrics = string.Empty;
            string artistUrl = LYRICS_URL.Replace("[ARTIST]", HttpUtility.UrlEncode(artist));

            HtmlDocument htmlDoc = Web.GetResponse(artistUrl, ENCODING);
            HtmlNodeCollection artistNodes = htmlDoc.DocumentNode.SelectNodes("//tr[@bgcolor='FFFFFF']//a");
            foreach (HtmlNode artistNode in artistNodes)
            {
                if (Management.NamesMatch(artistNode.InnerText, artist, matchType))
                {
                    artistUrl = LYRICS_BASE_URL + HttpUtility.HtmlDecode(artistNode.Attributes["href"].Value);

                    htmlDoc = Web.GetResponse(artistUrl, ENCODING);
                    HtmlNodeCollection albumNodes = htmlDoc.DocumentNode.SelectNodes("//span[@class='title']");
                    if (albumNodes != null)
                    {
                        foreach (HtmlNode albumNode in albumNodes)
                        {
                            if (Management.NamesMatch(albumNode.InnerText, album, matchType))
                            {
                                HtmlNodeCollection songNodes = albumNode.ParentNode.SelectNodes(".//a");
                                foreach (HtmlNode songNode in songNodes)
                                {
                                    if (Management.NamesMatch(songNode.InnerText, song, matchType))
                                    {
                                        string songUrl = LYRICS_BASE_URL + HttpUtility.HtmlDecode(songNode.Attributes["href"].Value);
                                        htmlDoc = Web.GetResponse(songUrl, ENCODING);

                                        MemoryStream ms = new MemoryStream();
                                        htmlDoc.Save(ms);

                                        StreamReader sr = new StreamReader(ms);
                                        ms.Seek(0, SeekOrigin.Begin);
                                        string html = sr.ReadToEnd();

                                        int startPos = html.IndexOf("<lyrics>");
                                        if (startPos >= 0)
                                        {
                                            startPos = startPos + 8;
                                            int endPos = html.IndexOf("\");", startPos);
                                            lyrics = html.Substring(startPos, endPos - startPos);
                                            char[] newLineChars = new char[] { (char)0x0A, (char)0x0D };
                                            lyrics = lyrics.Replace("&#xD;<br />", newLineChars[0].ToString() + newLineChars[1].ToString());

                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (lyrics == string.Empty)
            {
                return null;
            }
            else
            {
                return lyrics;
            }
        }
    }
}
