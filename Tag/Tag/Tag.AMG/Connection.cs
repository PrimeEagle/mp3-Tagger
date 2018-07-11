using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using HtmlAgilityPack;
using Tag.Utility;

namespace Tag.AMG
{
    public enum AmgSearchType 
    { 
        ArtistGroup = 1, 
        Album = 2, 
        Song = 3, 
        ClassicalWork = 55 
    }

    public enum AmgTabType
    {
        Overview = 0,
        Biography = 1,
        Discography = 2,
        Songs = 3,
        Credits = 4,
        ChartsAndAwards = 5
    }

    public enum AmgDiscographyType
    {
        MainAlbums = 20,
        Compilations = 21,
        Singles = 22,
        DVD = 23,
        Other = 24
    }

    public enum AmgSongType
    {
        Highlight = 30,
        AllSongs = 31,
        ComposedBy = 32
    }

    public enum AmgChartType
    {
        BillboardAlbum = 50,
        BillboardSingle = 51,
        Grammy = 52
    }

    public class Connection
    {
        const string AMG_URL = @"http://www.allmusic.com/cg/amg.dll?";
        const string AMG_BASE_URL = @"http://www.allmusic.com";
        private AmgSearchType _searchType;
        List<Artist> artistCache = new List<Artist>();
        List<Album> albumCache = new List<Album>();

        public AmgSearchType SearchType
        {
            get { return _searchType; }
            set { _searchType = value; }
        }

        private HtmlDocument GetResponse(string url)
        {
            StringBuilder htmlResponse = new StringBuilder();

            Uri uriLocation = new Uri(url);

            HttpWebRequest amgRequest = (HttpWebRequest)WebRequest.Create(uriLocation);
            Stream webresponse = ((HttpWebResponse)amgRequest.GetResponse()).GetResponseStream();

            StreamReader reader = new StreamReader(webresponse, Encoding.UTF8);

            while (!reader.EndOfStream)
            {
                htmlResponse.Append(reader.ReadLine());
            }
 
            byte[] html = Encoding.UTF8.GetBytes(htmlResponse.ToString());

            MemoryStream ms = new MemoryStream(html);

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.Load(ms);
            
            return htmlDoc;
        }

        private string GetQueryString(AmgSearchType searchType, string searchText)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("OPT1=" + Convert.ToInt32(searchType).ToString());
            sb.Append("&");
            sb.Append("P=amg");
            sb.Append("&");
            sb.Append("sql=" + HttpUtility.HtmlEncode(searchText.TrimEnd('&')));
            sb.Append("&");
            sb.Append("submit=Go");
            
            return  sb.ToString();
        }

        public Song SearchForSong(string song, string url)
        {
            Song foundSong = new Song();

            // build list of all songs (could be multi-page)

            bool lastPage = false;
            int pageNum = 1;
            bool foundMatch = false;

            while (!lastPage && !foundMatch)
            {
                HtmlDocument htmlDoc = GetResponse(AddPageNumToUrl(url, pageNum));

                HtmlNodeCollection songNodes = htmlDoc.DocumentNode.SelectNodes("//td[@class='sorted-cell']//a");
                foreach (HtmlNode songNode in songNodes)
                {
                    Song tempSong = new Song();
                    tempSong.Title = songNode.InnerText;
                    if (Management.NamesMatch(tempSong.Title, song))
                    {
                        foundSong.Title = songNode.InnerText;
                        string partialUrl = songNode.Attributes["href"].Value;

                        int startPos = partialUrl.IndexOf(":") + 1;
                        foundSong.AMGId = partialUrl.Substring(startPos);
                        foundSong.AMGUrl = HttpUtility.HtmlDecode(AMG_BASE_URL + partialUrl);
                        foundMatch = true;
                        break;
                    }
                }

                if (!foundMatch)
                {
                    HtmlNodeCollection pageLinks = htmlDoc.DocumentNode.SelectNodes("//div[@class='PagingRow']//a");
                    if (pageLinks != null)
                    {
                        foreach (HtmlNode pageLink in pageLinks)
                        {
                            if (pageLink.InnerText.ToUpper().Contains("NEXT PAGE"))
                            {
                                pageNum++;
                                lastPage = false;
                                break;
                            }
                            else
                            {
                                lastPage = true;
                            }
                        }
                    }
                    else
                    {
                        lastPage = true;
                    }
                }
            }

            if (!foundMatch)
            {
                return null;
            }
            else
            {
                return foundSong;
            }
        }

        public Album SearchForAlbum(string url, bool useDefaultAlbum, string defaultAlbumName)
        {
            // check cache for default albums
            if (useDefaultAlbum)
            {
                Album cacheAlbum = GetAlbumFromCache(defaultAlbumName);
                if (cacheAlbum != null)
                {
                    return cacheAlbum;
                }
            }

            Album foundAlbum = new Album();

            bool foundMatch = false;

            HtmlDocument htmlDoc = GetResponse(url);

            //do we have an "Appears On" section?

            List<Album> tempAlbumList = new List<Album>();

            HtmlNodeCollection titleNodes = htmlDoc.DocumentNode.SelectNodes("//td[@class='large-list-title']");
            foreach (HtmlNode titleNode in titleNodes)
            {
                if (titleNode.InnerText == "Appears On")
                {
                    HtmlNodeCollection albumBlocks = htmlDoc.DocumentNode.SelectNodes("//table[@id='ExpansionTable1']//tr[@id='trlink' and @class='visible']");
                    foreach (HtmlNode albumBlock in albumBlocks)
                    {
                        Album tempAlbum = new Album();

                        HtmlNodeCollection albumData = albumBlock.SelectNodes("td");


                        // have to do this, because some dates are not filled in
                        try
                        {
                            tempAlbum.ReleaseYear = Convert.ToInt32(albumData[1].InnerText);
                        }
                        catch
                        {

                        }

                        HtmlNode albumLinkNode = albumData[2].SelectSingleNode("a");
                        tempAlbum.Name = albumLinkNode.InnerText;

                        string partialUrl = albumLinkNode.Attributes["href"].Value;
                        tempAlbum.AMGUrl = AMG_BASE_URL + partialUrl;
                        int startPos = partialUrl.IndexOf(":") + 1;
                        tempAlbum.AMGId = partialUrl.Substring(startPos);

                        tempAlbumList.Add(tempAlbum);
                        if (useDefaultAlbum && Management.NamesMatch(tempAlbum.Name, defaultAlbumName))
                        {
                            foundMatch = true;
                            foundAlbum.AMGId = tempAlbum.AMGId;
                            foundAlbum.AMGUrl = HttpUtility.HtmlDecode(tempAlbum.AMGUrl);
                            foundAlbum.Name = tempAlbum.Name;
                            foundAlbum.ReleaseYear = tempAlbum.ReleaseYear;
                            break;
                        }
                    }
                }
            }

            if (!foundMatch)
            {
                // sort albums by date
                Album.AlbumComparer c = Album.GetComparer();
                tempAlbumList.Sort(c);

                //find the first album that has a release date greater than zero

                List<int> removeList = new List<int>();

                for (int i = 0; i < tempAlbumList.Count; i++)
                {
                    if (tempAlbumList[i].ReleaseYear == 0 ||
                        tempAlbumList[i].Name.Contains("[") ||
                        tempAlbumList[i].Name.ToUpper().Contains("LIVE IN"))

                        removeList.Add(i);
                }

                for (int i = removeList.Count - 1; i >= 0; i--)
                {
                    tempAlbumList.RemoveAt(removeList[i]);
                }

                if (tempAlbumList.Count > 0)
                {
                    foundMatch = true;
                    foundAlbum.AMGId = tempAlbumList[0].AMGId;
                    foundAlbum.AMGUrl = HttpUtility.HtmlDecode(tempAlbumList[0].AMGUrl);
                    foundAlbum.Name = tempAlbumList[0].Name;
                    foundAlbum.ReleaseYear = tempAlbumList[0].ReleaseYear;

                    Album cacheAlbum = GetAlbumFromCache(foundAlbum.Name);
                    if (cacheAlbum != null)
                    {
                        return cacheAlbum;
                    }
                }
            }

            if (foundMatch)
            {
                htmlDoc = GetResponse(foundAlbum.AMGUrl);

                HtmlNodeCollection flavorBlocks = htmlDoc.DocumentNode.SelectNodes("//div[@id='left-sidebar-list']");

                if (flavorBlocks != null)
                {

                    HtmlNodeCollection genreNodes = flavorBlocks[0].SelectNodes("ul//a");
                    if (genreNodes != null)
                    {
                        foreach (HtmlNode genre in genreNodes)
                        {
                            foundAlbum.Genres.Add(genre.InnerText);
                        }
                    }

                    if (flavorBlocks.Count > 1)
                    {
                        HtmlNodeCollection styleNodes = flavorBlocks[1].SelectNodes("ul//a");
                        if (styleNodes != null)
                        {
                            foreach (HtmlNode style in styleNodes)
                            {
                                foundAlbum.Styles.Add(style.InnerText);
                            }
                        }
                    }

                    if (flavorBlocks.Count > 2)
                    {
                        HtmlNodeCollection moodNodes = flavorBlocks[2].SelectNodes("ul//a");
                        if (moodNodes != null)
                        {
                            foreach (HtmlNode mood in moodNodes)
                            {
                                foundAlbum.Moods.Add(mood.InnerText);
                            }
                        }
                    }

                    if (flavorBlocks.Count > 3)
                    {
                        HtmlNodeCollection themeNodes = flavorBlocks[3].SelectNodes("ul//a");
                        if (themeNodes != null)
                        {
                            foreach (HtmlNode theme in themeNodes)
                            {
                                foundAlbum.Themes.Add(theme.InnerText);
                            }
                        }
                    }
                }


                //get tracks
                titleNodes = htmlDoc.DocumentNode.SelectNodes("//td[@class='large-list-title']");
                foreach (HtmlNode titleNode in titleNodes)
                {
                    if (titleNode.InnerText == "Tracks")
                    {
                        HtmlNodeCollection trackNodes = htmlDoc.DocumentNode.SelectNodes("//tr[@class='visible' and @id='trlink']");
                        foreach(HtmlNode trackNode in trackNodes)
                        {
                            HtmlNodeCollection trackInfoNodes = trackNode.SelectNodes(".//td");
                            if (trackInfoNodes != null)
                            {
                                foundAlbum.TrackNames.Add(trackInfoNodes[4].SelectSingleNode("a").InnerText);
                            }
                        }
                    }
                }

                //get image
                HtmlNode imageNode = htmlDoc.DocumentNode.SelectSingleNode("//td[@class='left-sidebar']//img");
                if (imageNode != null)
                {
                    string imageUrl = imageNode.Attributes["src"].Value;

                    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(imageUrl);
                    HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

                    foundAlbum.AlbumCoverImage = Image.FromStream(webResponse.GetResponseStream());
                }
            }

            if (!foundMatch)
            {
                return null;
            }
            else
            {
                albumCache.Add(foundAlbum);
                return foundAlbum;
            }
        }

        public Artist SearchForArtist(string artist)
        {
            return SearchForArtist(artist, null);
        }

        public Artist SearchForArtist(string artist, string url) 
        {
            // check the cache
            Artist cacheArtist = GetArtistFromCache(artist);
            if (cacheArtist != null)
            {
                return cacheArtist;
            }

            Artist foundArtist = new Artist();
            bool foundMatch = false;

            if (url == null)
            {
                url = AMG_URL + GetQueryString(AmgSearchType.ArtistGroup, artist);
            }

            HtmlDocument htmlDoc = GetResponse(url);

            // check to see if we got to an artist page directly
            HtmlNode artistDivNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='artistpage']");
            if(artistDivNode != null)
            {
                HtmlNode titleNode = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='title']");
                foundArtist.Name = titleNode.InnerText;

                HtmlNodeCollection UrlNodes = htmlDoc.DocumentNode.SelectNodes("//table[@class='tab']//td//a");
                string urlPath = UrlNodes[0].Attributes["href"].Value;
                int startPos = urlPath.IndexOf(":") + 1;
                int endPos = urlPath.IndexOf("~", startPos);
                foundArtist.AMGId = urlPath.Substring(startPos, endPos - startPos);
                foundArtist.AMGOverviewUrl = AMG_BASE_URL + urlPath.Substring(0, endPos);
                foundMatch = true;
            } 
            else
            {
                // Get a list of all artists and try to match, if we're not on an overview page
                HtmlNodeCollection artistNodes = htmlDoc.DocumentNode.SelectNodes("//tr[@id='trlink']//a");
                foreach (HtmlNode artistNode in artistNodes)
                {
                    Artist artistItem = new Artist();
                    artistItem.Name = artistNode.InnerText;
                    if (Management.NamesMatch(artistItem.Name, artist))
                    {
                        foundArtist.AMGOverviewUrl = HttpUtility.HtmlDecode(artistNode.Attributes["href"].Value);

                        int startPos = foundArtist.AMGOverviewUrl.IndexOf(":") + 1;
                        foundArtist.AMGId = foundArtist.AMGOverviewUrl.Substring(startPos);
                        foundArtist.AMGOverviewUrl = AMG_BASE_URL + foundArtist.AMGOverviewUrl;
                        foundArtist.Name = artistItem.Name;
                        foundMatch = true;
                        break;
                    }
                }
            }

            //retrieve Genre, etc. info for artist
            if (foundMatch)
            {
                HtmlNodeCollection flavorBlocks = htmlDoc.DocumentNode.SelectNodes("//td[@class='styles_moods']//div[@id='left-sidebar-list']");

                if (flavorBlocks != null)
                {

                    HtmlNodeCollection genreNodes = flavorBlocks[0].SelectNodes("ul//a");
                    if (genreNodes != null)
                    {
                        foreach (HtmlNode genre in genreNodes)
                        {
                            foundArtist.Genres.Add(genre.InnerText);
                        }
                    }

                    if (flavorBlocks.Count > 1)
                    {
                        HtmlNodeCollection styleNodes = flavorBlocks[1].SelectNodes("ul//a");
                        if (styleNodes != null)
                        {
                            foreach (HtmlNode style in styleNodes)
                            {
                                foundArtist.Styles.Add(style.InnerText);
                            }
                        }
                    }

                    if (flavorBlocks.Count > 2)
                    {
                        HtmlNodeCollection moodNodes = flavorBlocks[2].SelectNodes("ul//a");
                        if (moodNodes != null)
                        {
                            foreach (HtmlNode mood in moodNodes)
                            {
                                foundArtist.Moods.Add(mood.InnerText);
                            }
                        }
                    }

                    if (flavorBlocks.Count > 3)
                    {
                        HtmlNodeCollection instrumentNodes = flavorBlocks[3].SelectNodes("ul//a");
                        if (instrumentNodes != null)
                        {
                            foreach (HtmlNode instrument in instrumentNodes)
                            {
                                foundArtist.Instruments.Add(instrument.InnerText);
                            }
                        }
                    }
                }
            }

            if (!foundMatch)
            {
                return null;
            }
            else
            {
                foundArtist.AMGAllSongsUrl = foundArtist.AMGOverviewUrl + "~T" + Convert.ToInt32(AmgSongType.AllSongs).ToString();
                artistCache.Add(foundArtist);
                return foundArtist;
            }
        }

        private string AddPageNumToUrl(string url, int pageNum)
        {
            string newUrl;

            if (url.Contains("~"))
            {
                newUrl = url.Replace("~", "~" + pageNum.ToString() + "~");
            }
            else
            {
                newUrl = url;
            }

            return newUrl;
        }

        private Artist GetArtistFromCache(string artist)
        {
            Artist cachedArtist = artistCache.Find(
                delegate(Artist a) 
                { 
                    return Management.NamesMatch(a.Name, artist); 
                }
            );
            
            return cachedArtist;
        }

        private Album GetAlbumFromCache(string album)
        {
            Album cachedAlbum = albumCache.Find(
                delegate(Album a)
                {
                    return Management.NamesMatch(a.Name, album);
                }
            );

            return cachedAlbum;
        }
    }
}
