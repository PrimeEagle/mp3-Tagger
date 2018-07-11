using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using Tag.Utility;
using HtmlAgilityPack;

namespace Tag.LyricsHead
{
    public class LyricsHeadConnection
    {
        const string LYRICS_HEAD_URL = @"http://www.lyricshead.com/search_controller.php";
        const string LYRICS_HEAD_BASE_URL = @"http://www.lyricshead.com/";
        const string ENCODING = "iso-8859-1";

        public string GetLyrics(string artist, string song)
        {
            string lyrics = null;

            lyrics = SearchForLyrics(artist, song, ComparisonType.Exact);

            if (lyrics == null)
            {
                lyrics = SearchForLyrics(artist, song, ComparisonType.EndsWith);
            }

            if (lyrics == null)
            {
                lyrics = SearchForLyrics(artist, song, ComparisonType.StartsWith);
            }

            if (lyrics == null)
            {
                lyrics = SearchForLyrics(artist, song, ComparisonType.Exact);
            }

            return lyrics;
        }


        private string SearchForLyrics(string artist, string song, ComparisonType matchType)
        {
            string lyrics = string.Empty;
            string artistUrl = LYRICS_HEAD_URL + "?q=" + HttpUtility.UrlEncode(artist) + "&search_in=artists";

            HtmlDocument htmlDoc = Web.GetResponse(artistUrl, ENCODING);
            
            HtmlNodeCollection songNodes = htmlDoc.DocumentNode.SelectNodes("//td[@class='data']//a");
            if (songNodes != null)
            {
                foreach (HtmlNode songNode in songNodes)
                {
                    string songName = songNode.InnerText;

                    string songUrl = songNode.Attributes["href"].Value;
                    
                    //check to see if we're looking at the "Songs" section, or something else
                    if (songUrl.ToUpper().Contains("/LYRICS/") && Management.NamesMatch(songName, song, matchType))
                    {
                        htmlDoc = Web.GetResponse(songUrl, ENCODING);

                        bool lyricsMissing = false;

                        HtmlNodeCollection missingLyricsNodes = htmlDoc.DocumentNode.SelectNodes("//u");
                        if (missingLyricsNodes != null)
                        {
                            foreach(HtmlNode missingNode in missingLyricsNodes) 
                            {
                                if (Management.NamesMatch(missingNode.InnerText, "Help the community and add lyrics for this song", ComparisonType.Exact))
                                {
                                    lyricsMissing = true;
                                }
                            }
                            
                        }

                        if (!lyricsMissing)
                        {
                            HtmlNode lyricNode = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='enlargeOnHover']");
                            if (lyricNode != null)
                            {
                                lyrics = lyricNode.InnerHtml;
                                lyrics = lyrics.Replace("<br>", Environment.NewLine).Replace("<br/>", Environment.NewLine).Replace("<br />", Environment.NewLine).Replace("ï¿½", "'");
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
