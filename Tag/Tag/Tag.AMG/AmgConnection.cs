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
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using HtmlAgilityPack;
using Tag.Utility;

namespace Tag.AMG
{
 
    public enum AmgSearchType 
    { 
        NONE = 0,
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

    public enum AmgClassicalTabType
    {
        Overview = 0,
        Biography = 1,
        Works = 2,
        Credits = 3
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

    public class AmgConnection
    {
        const string AMG_URL = @"http://www.allmusic.com/cg/amg.dll?";
        const string AMG_BASE_URL = @"http://www.allmusic.com";
        const string ENCODING = "iso-8859-1";

        const int MaxArtistCache = 50;
        const int MaxAlbumCache = 50;
        Dictionary<int, string> romanNumerals = new Dictionary<int, string>();
        List<string> movementDelimeters = new List<string>();
        List<Artist> artistCache = new List<Artist>();
        List<Album> albumCache = new List<Album>();

        public AmgConnection()
        {
            romanNumerals.Add(9, "IX");
            romanNumerals.Add(10, "X");
            romanNumerals.Add(8, "VIII");
            romanNumerals.Add(7, "VII");
            romanNumerals.Add(6, "VI");
            romanNumerals.Add(4, "IV");
            romanNumerals.Add(5, "V");
            romanNumerals.Add(3, "III");
            romanNumerals.Add(2, "II");
            romanNumerals.Add(1, "I");

            movementDelimeters.Add(".");
            movementDelimeters.Add("-");
            movementDelimeters.Add(",");
            movementDelimeters.Add(" -");
        }

        private string GetQueryString(AmgSearchType searchType, string searchText)
        {
            StringBuilder sb = new StringBuilder();
            
            if (searchType != AmgSearchType.NONE)
            {
                sb.Append("OPT1=" + Convert.ToInt32(searchType).ToString());
                sb.Append("&");
            }
            
            sb.Append("P=amg");
            sb.Append("&");
            sb.Append("sql=" + searchText.TrimEnd('&')).Replace("+", "%20");

            if (searchType != AmgSearchType.NONE)
            {
                sb.Append("&");
                sb.Append("submit=Go");
            }
            
            return  sb.ToString();
        }

        public ClassicalSong GetClassicalSong(string song, string url)
        {
            ClassicalSong foundSong = new ClassicalSong();

            foundSong = ScanPagesForClassicalSongMatch(song, song, url, ComparisonType.Exact);

            if (foundSong == null)
            {
                foundSong = ScanPagesForClassicalSongMatch(song, song, url, ComparisonType.CatalogNumber);
            }
            
            if (foundSong == null)
            {
                foundSong = ScanPagesForClassicalSongMatch(song, song, url, ComparisonType.StartsWith);
            }

            if (foundSong == null)
            {
                foundSong = ScanPagesForClassicalSongMatch(song, song, url, ComparisonType.EndsWith);
            }

            if (foundSong == null)
            {
                foundSong = ScanPagesForClassicalSongMatch(song, song, url, ComparisonType.Contains);
            }

            if (foundSong == null)
            {
                //try to do partial phrase matches
                string[] splitSong = song.Split(' ');

                if (splitSong.Length >= 3)
                {
                    string matchToTry = splitSong[0] + " " + splitSong[1] + " " + splitSong[2];
                    foundSong = ScanPagesForClassicalSongMatch(song, matchToTry, url, ComparisonType.Contains);

                    if (foundSong == null)
                    {
                        matchToTry = splitSong[0] + " " + splitSong[1];
                        foundSong = ScanPagesForClassicalSongMatch(song, matchToTry, url, ComparisonType.Contains);
                    }

                    if (foundSong == null)
                    {
                        matchToTry = splitSong[1] + " " + splitSong[2];
                        foundSong = ScanPagesForClassicalSongMatch(song, matchToTry, url, ComparisonType.Contains);
                    }
                }
            }

            if (foundSong == null)
            {
                foundSong = ScanPagesForClassicalSongMatch(song, song, url, ComparisonType.Majority);
            }

            if (foundSong == null)
            {
                foundSong = ScanPagesForClassicalMovementMatch(song, url, ComparisonType.Majority);
            }

            return foundSong;
        }
        
        public Song GetSong(string song, string url)
        {
            Song foundSong = new Song();

            // build list of all songs (could be multi-page)
            foundSong = ScanPagesForSongMatch(song, url, ComparisonType.Exact);
            if (foundSong == null)
            {
                foundSong = ScanPagesForSongMatch(song, url, ComparisonType.StartsWith);
            }

            if (foundSong == null)
            {
                foundSong = ScanPagesForSongMatch(song, url, ComparisonType.EndsWith);
            }

            if (foundSong == null)
            {
                foundSong = ScanPagesForSongMatch(song, url, ComparisonType.Contains);
            }

            return foundSong;
        }

        public Album GetAlbum(string songUrl, string artistUrl, bool useDefaultAlbum, string defaultAlbumName)
        {
            Album foundAlbum = null;

            foundAlbum = SearchForAlbum(songUrl, artistUrl, useDefaultAlbum, defaultAlbumName, ComparisonType.Exact);

            return foundAlbum;
        }

        public Song SearchForSong(string song, string url)
        {
            Song foundSong = new Song();

            // build list of all songs (could be multi-page)
            foundSong = ScanPagesForSongMatch(song, url, ComparisonType.Exact);
            
            if (foundSong == null)
            {
                foundSong = ScanPagesForSongMatch(song, url, ComparisonType.StartsWith);
            }

            if (foundSong == null)
            {
                foundSong = ScanPagesForSongMatch(song, url, ComparisonType.EndsWith);
            }

            if (foundSong == null)
            {
                foundSong = ScanPagesForSongMatch(song, url, ComparisonType.Contains);
            }

            return foundSong;
        }

        private Song ScanPagesForSongMatch(string song, string url, ComparisonType matchType)
        {
            bool lastPage = false;
            int pageNum = 1;
            bool foundMatch = false;
            Song foundSong = new Song();

            while (!lastPage && !foundMatch)
            {
                HtmlDocument htmlDoc = Web.GetResponse(AddPageNumToUrl(url, pageNum), ENCODING);

                HtmlNodeCollection songNodes = htmlDoc.DocumentNode.SelectNodes("//td[@class='sorted-cell']//a");
                if (songNodes != null)
                {
                    foreach (HtmlNode songNode in songNodes)
                    {
                        Song tempSong = new Song();
                        tempSong.Title = songNode.InnerText;
                        if (Management.NamesMatch(tempSong.Title, song, matchType))
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

        private ClassicalSong ScanPagesForClassicalSongMatch(string searchSong, string matchSong, string url, ComparisonType matchType)
        {
            bool lastPage = false;
            int pageNum = 1;
            bool foundMatch = false;
            ClassicalSong foundSong = new ClassicalSong();

            string newSong = matchSong;

            int movement = GetMovementNumber(searchSong);
            if (movement > 0)
            {
                newSong = RemoveMovementInfo(matchSong);
            }

            if (matchSong.Contains("132"))
            {
                int aa = 1;
            }

            while (!lastPage && !foundMatch)
            {
                HtmlDocument htmlDoc = Web.GetResponse(AddPageNumToUrl(url, pageNum), ENCODING);

                HtmlNodeCollection songNodes = htmlDoc.DocumentNode.SelectNodes("//td[@class='sorted-cell']");
                if (songNodes != null)
                {
                    foreach (HtmlNode songNode in songNodes)
                    {
                        Song tempSong = new Song();
                        tempSong.Title = songNode.InnerText;
                        if (tempSong.Title.Contains("132"))
                        {
                            int aa = 1;
                        }
                        if (Management.NamesMatch(tempSong.Title, newSong, matchType))
                        {
                            foundSong.Title = songNode.InnerText;
                            try
                            {
                                string partialUrl = songNode.ParentNode.Attributes["onclick"].Value;
                                int startPos = partialUrl.IndexOf("'") + 1;
                                int endPos = partialUrl.IndexOf("'", startPos);
                                partialUrl = partialUrl.Substring(startPos, endPos - startPos);


                                foundSong.AMGId = partialUrl.Substring(partialUrl.IndexOf(":") + 1);
                                foundSong.AMGUrl = AMG_URL + "?" + GetQueryString(AmgSearchType.NONE, partialUrl);

                                //see if we have a description
                                htmlDoc = Web.GetResponse(foundSong.AMGUrl, ENCODING);
                                HtmlNodeCollection descriptionTabs = htmlDoc.DocumentNode.SelectNodes("//td[@class='tab_off']");
              
                                if (descriptionTabs != null)
                                {
                                    bool hasDescription = false;
                                    foreach (HtmlNode descTab in descriptionTabs)
                                    {
                                        if (descTab.InnerText.Contains("Description"))
                                        {
                                            hasDescription = true;
                                            break;
                                        }
                                    }

                                    if (hasDescription)
                                    {
                                        htmlDoc = Web.GetResponse(foundSong.AMGUrl + "~T1", ENCODING);

                                        HtmlNode descriptionNode = htmlDoc.DocumentNode.SelectSingleNode("//td[@colspan='2']/p");
                                        if (descriptionNode != null)
                                        {
                                            foundSong.Description = descriptionNode.InnerText.Replace("\\\"", "\"");
                                        }
                                    }
                                }

                                HtmlNodeCollection songInfoNodes = htmlDoc.DocumentNode.SelectNodes("//td[@class='formed-sub']");
                                if (songInfoNodes != null)
                                {
                                    foundSong.Genre = songInfoNodes[1].InnerText;
                                    string[] workTypes = songInfoNodes[2].InnerText.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                                    foreach (string work in workTypes)
                                    {
                                        foundSong.WorkTypes.Add(work);
                                    }

                                    HtmlNode compositionYearNode = htmlDoc.DocumentNode.SelectSingleNode("//span[contains(.,'Composition Date')]");
                                    if (compositionYearNode == null)
                                    {
                                        // if no composition year, then take the publication year
                                        foundSong.PublicationYear = GetCompositionYear(songInfoNodes[4].InnerText);
                                    }
                                    else
                                    {
                                        foundSong.PublicationYear = GetCompositionYear(songInfoNodes[3].InnerText);
                                    }
                                }

                                if (movement > 0)
                                {
                                    foundSong.Movement = movement;

                                    //see if we have a description
                                    htmlDoc = Web.GetResponse(foundSong.AMGUrl, ENCODING);
                                    HtmlNodeCollection movementTabs = htmlDoc.DocumentNode.SelectNodes("//td[@class='tab_off']");

                                    if (movementTabs != null)
                                    {
                                        bool hasMovements = false;
                                        foreach (HtmlNode movementTab in movementTabs)
                                        {
                                            if (movementTab.InnerText.Contains("Movements"))
                                            {
                                                hasMovements = true;
                                                break;
                                            }
                                        }

                                        if (hasMovements)
                                        {
                                            htmlDoc = Web.GetResponse(foundSong.AMGUrl + "~T2", ENCODING);
                                            HtmlNodeCollection movementNodes = htmlDoc.DocumentNode.SelectNodes("//tr[@class='out']");
                                            if (movementNodes != null)
                                            {
                                                HtmlNodeCollection movementInfoNodes = movementNodes[foundSong.Movement - 1].SelectNodes(".//td");
                                                if (movementInfoNodes != null)
                                                {
                                                    foundSong.MovementName = StripMovementPrefix(movementInfoNodes[1].InnerText);
                                                }

                                            }
                                        }
                                        else
                                        {
                                            //song title indicates movements, but none found in AMG.
                                            //use what's in the title.

                                            foundSong.MovementName = GetMovementName(searchSong);
                                        }
                                    }

                                }

                                foundMatch = true;
                                break;

                            }
                            catch
                            {
                            }
                        }
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


        private ClassicalSong ScanPagesForClassicalMovementMatch(string song, string url, ComparisonType matchType)
        {
            bool lastPage = false;
            int pageNum = 1;
            bool foundMatch = false;
            ClassicalSong foundSong = new ClassicalSong();

            string newSong = song;

            while (!lastPage && !foundMatch)
            {
                HtmlDocument htmlDoc = Web.GetResponse(AddPageNumToUrl(url, pageNum), ENCODING);

                HtmlNodeCollection songNodes = htmlDoc.DocumentNode.SelectNodes("//td[@class='sorted-cell']");
                if (songNodes != null)
                {
                    foreach (HtmlNode songNode in songNodes)
                    {
                        if (foundMatch)
                        {
                            break;
                        }

                        //Song tempSong = new Song();
                        //tempSong.Title = songNode.InnerText;

                        HtmlAttribute hrefAttribute = songNode.ParentNode.Attributes["onclick"];

                        if (hrefAttribute != null)
                        {

                            string partialUrl = hrefAttribute.Value;

                            int startPos = partialUrl.IndexOf("'") + 1;
                            int endPos = partialUrl.IndexOf("'", startPos);
                            partialUrl = partialUrl.Substring(startPos, endPos - startPos);
                            string songUrl = AMG_URL + "?" + GetQueryString(AmgSearchType.NONE, partialUrl);
                            string amgId = partialUrl.Substring(partialUrl.IndexOf(":") + 1);

                            htmlDoc = Web.GetResponse(songUrl + "~T2", ENCODING);
                            HtmlNodeCollection movementNodes = htmlDoc.DocumentNode.SelectNodes("//tr[@class='out']");
                            if (movementNodes != null)
                            {
                                foreach (HtmlNode movementNode in movementNodes)
                                {
                                    HtmlNodeCollection infoNodes = movementNode.SelectNodes(".//td");
                                    if (infoNodes != null)
                                    {
                                        int movementNum = 0;
                                        if (infoNodes != null)
                                        {
                                            string movementName = infoNodes[1].InnerText;
                                            movementNum++;

                                            if (Management.NamesMatch(song, movementName, matchType))
                                            {
                                                foundSong.MovementName = StripMovementPrefix(movementName);
                                                foundSong.Movement = movementNum;
                                                foundSong.AMGUrl = songUrl;
                                                foundSong.AMGId = amgId;
                                                foundSong.Title = songNode.InnerText;

                                                foundMatch = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
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


            if (foundMatch)
            {
                HtmlDocument htmlDoc = Web.GetResponse(foundSong.AMGUrl + "~T1", ENCODING);

                HtmlNode descriptionNode = htmlDoc.DocumentNode.SelectSingleNode("//td[@colspan='2']/p");
                if (descriptionNode != null)
                {
                    foundSong.Description = descriptionNode.InnerText.Replace("\\\"", "\"");
                }

                HtmlNodeCollection songInfoNodes = htmlDoc.DocumentNode.SelectNodes("//td[@class='formed-sub']");
                if (songInfoNodes != null)
                {
                    foundSong.Genre = songInfoNodes[1].InnerText;
                    string[] workTypes = songInfoNodes[2].InnerText.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string work in workTypes)
                    {
                        foundSong.WorkTypes.Add(work);
                    }

                    HtmlNode compositionYearNode = htmlDoc.DocumentNode.SelectSingleNode("//span[contains(.,'Composition Date')]");
                    if (compositionYearNode == null)
                    {
                        // if no composition year, then take the publication year
                        foundSong.PublicationYear = GetCompositionYear(songInfoNodes[4].InnerText);
                    }
                    else
                    {
                        foundSong.PublicationYear = GetCompositionYear(songInfoNodes[3].InnerText);
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


        public Album SearchForAlbum(string songUrl, string artistUrl, bool useDefaultAlbum, string defaultAlbumName, ComparisonType matchType)
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

            List<string> mainAlbumAMGIds = new List<string>();

            Album foundAlbum = new Album();

            bool foundMatch = false;

            BuildAlbumIds(mainAlbumAMGIds, artistUrl, AmgDiscographyType.MainAlbums);

            HtmlDocument htmlDoc = Web.GetResponse(songUrl, ENCODING);

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
                        tempAlbum.AMGUrl = HttpUtility.HtmlDecode(AMG_BASE_URL + partialUrl);
                        int startPos = partialUrl.IndexOf(":") + 1;
                        tempAlbum.AMGId = partialUrl.Substring(startPos);

                        tempAlbumList.Add(tempAlbum);
                        if (useDefaultAlbum && Management.NamesMatch(tempAlbum.Name, defaultAlbumName, matchType))
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

                foreach(Album tempAlbum in tempAlbumList)
                {
                    if (mainAlbumAMGIds.Contains(tempAlbum.AMGId) && tempAlbum.ReleaseYear > 0)
                    {
                        foundMatch = true;
                    }
                    else if (Management.NamesMatch(GetAlbumGenre(tempAlbum.AMGUrl), "Soundtrack", ComparisonType.Exact))
                    {
                        foundMatch = true;
                    }

                    if(foundMatch) 
                    {
                        foundAlbum.AMGId = tempAlbum.AMGId;
                        foundAlbum.AMGUrl = HttpUtility.HtmlDecode(tempAlbum.AMGUrl);
                        string cleanName = tempAlbum.Name;
                        int startPos = cleanName.IndexOf("[");
                        if (startPos >= 0)
                        {
                            int endPos = cleanName.IndexOf("]", startPos);
                            if (endPos > startPos)
                            {
                                cleanName = cleanName.Remove(startPos, endPos - startPos + 1);
                            }
                        }
                        cleanName = cleanName.Replace("  ", " ").Replace("  ", " ").Trim();
                        foundAlbum.Name = cleanName;
                        foundAlbum.ReleaseYear = tempAlbum.ReleaseYear;

                        Album cacheAlbum = GetAlbumFromCache(foundAlbum.Name);
                        if (cacheAlbum != null)
                        {
                            return cacheAlbum;
                        }
                        break;
                    }
                }
            }

            if (foundMatch)
            {
                htmlDoc = Web.GetResponse(foundAlbum.AMGUrl, ENCODING);

                HtmlNodeCollection titleBlocks = htmlDoc.DocumentNode.SelectNodes("//div[@id='left-sidebar-title']//span");
                if (titleBlocks != null)
                {
                    foreach (HtmlNode titleBlock in titleBlocks)
                    {
                        if (Management.NamesMatch(titleBlock.InnerText, "Label", ComparisonType.Exact))
                        {
                            string label = string.Empty;

                            try
                            {
                                label = titleBlock.ParentNode.ParentNode.ParentNode.NextSibling.NextSibling.NextSibling.SelectSingleNode("./td[@class='sub-text']").InnerText;
                            }
                            catch
                            {
                            }

                            foundAlbum.Publisher = label;
                            break;
                        }
                    }
                }

                HtmlNodeCollection flavorBlocks = htmlDoc.DocumentNode.SelectNodes("//div[@id='left-sidebar-list']");

                if (flavorBlocks != null)
                {

                    HtmlNodeCollection genreNodes = flavorBlocks[0].SelectNodes("ul//a");
                    if (genreNodes != null)
                    {
                            foundAlbum.Genre = genreNodes[0].InnerText;
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


                string trackType = string.Empty;
                HtmlNodeCollection trackHeaders = htmlDoc.DocumentNode.SelectNodes("//td[@class='passive']");
                if (trackHeaders != null)
                {
                    foreach (HtmlNode trackHeader in trackHeaders)
                    {
                        if(Management.NamesMatch(trackHeader.InnerText, "Performer", ComparisonType.Exact))
                        {
                            trackType = "performer";
                            foundAlbum.Compilation = true;
                        }

                        if (Management.NamesMatch(trackHeader.InnerText, "Composer", ComparisonType.Exact))
                        {
                            trackType = "composer";
                            foundAlbum.Compilation = false;
                        }                    
                    }
                }
                
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
                                Track t = new Track();
                                t.Number = Convert.ToInt32(trackInfoNodes[2].InnerText);
                                t.Title = trackInfoNodes[4].SelectSingleNode("a").InnerText;

                                if(trackType == "performer")
                                    t.Performer = trackInfoNodes[5].InnerText;

                                if(trackType == "composer")
                                    t.Composer = trackInfoNodes[5].InnerText;

                                foundAlbum.Tracks.Add(t);
                            }
                        }
                    }
                }

                //get image
                HtmlNode imageNode = htmlDoc.DocumentNode.SelectSingleNode("//td[@class='left-sidebar']//img");
                if (imageNode != null)
                {
                    string imageUrl = imageNode.Attributes["src"].Value;

                    if (!imageUrl.StartsWith("http"))
                    {
                        imageUrl = AMG_BASE_URL + HttpUtility.HtmlDecode(imageUrl);
                    }

                    foundAlbum.AlbumCoverImage = Web.GetImage(imageUrl);
                    foundAlbum.AlbumCoverMimeType = Management.GetImageMIMEType(imageUrl);
                }
            }

            if (!foundMatch)
            {
                return null;
            }
            else
            {
                AddToAlbumCache(foundAlbum);
                return foundAlbum;
            }
        }

        public Artist GetArtist(string artist)
        {
            Artist a = null;
            a = SearchForArtist(artist, ComparisonType.Exact);

            if (a == null)
            {
                a = SearchForArtist(artist, ComparisonType.EndsWith);
            }

            if (a == null)
            {
                a = SearchForArtist(artist, ComparisonType.StartsWith);
            }

            if (a == null)
            {
                a = SearchForArtist(artist, ComparisonType.Exact);
            }

            return a;
        }

        public ClassicalArtist GetClassicalArtist(string artist)
        {
            ClassicalArtist a = null;
            a = SearchForClassicalArtist(artist, artist, ComparisonType.Exact);
            
            if (a == null)
            {
                a = SearchForClassicalArtist(artist, artist, ComparisonType.EndsWith);
            }

            if (a == null)
            {
                a = SearchForClassicalArtist(artist, artist, ComparisonType.StartsWith);
            }

            if (a == null)
            {
                a = SearchForClassicalArtist(artist, artist, ComparisonType.Contains);
            }

            // check for last name only
            
            if (a == null)
            {
                string[] artistNames = artist.Split(' ');
                a = SearchForClassicalArtist(artist, artistNames[artistNames.Length - 1], ComparisonType.EndsWith);
            }

            return a;
        }

        private ClassicalArtist SearchForClassicalArtist(string searchArtist, string matchArtist, ComparisonType matchType)
        {
            // check the cache
            Artist cacheArtist = GetArtistFromCache(searchArtist);
            if (cacheArtist != null)
            {
                return (ClassicalArtist)cacheArtist;
            }

            ClassicalArtist foundArtist = new ClassicalArtist();
            bool foundMatch = false;

            string url = AMG_URL + GetQueryString(AmgSearchType.ArtistGroup, searchArtist);

            HtmlDocument htmlDoc = Web.GetResponse(url, ENCODING);

            // check to see if we got to an artist page directly
            HtmlNode artistDivNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='cartistpage']");
            if (artistDivNode != null)
            {
                HtmlNode titleNode = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='title']");
                foundArtist.Name = titleNode.InnerText;

                HtmlNodeCollection UrlNodes = htmlDoc.DocumentNode.SelectNodes("//table[@class='tab']//td//a");
                string urlPath = UrlNodes[0].Attributes["href"].Value;
                int startPos = urlPath.IndexOf(":") + 1;
                int endPos = urlPath.IndexOf("~", startPos);
                foundArtist.AMGId = urlPath.Substring(startPos, endPos - startPos);
                foundArtist.AMGOverviewUrl = AMG_BASE_URL + HttpUtility.HtmlDecode(urlPath.Substring(0, endPos));
                foundMatch = true;
            }
            else
            {
                // see if we got a non-classical artist directly
                HtmlNode popularArtistDivNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='artistpage']");
                if (popularArtistDivNode != null)
                {
                    HtmlNode moreMatches = htmlDoc.DocumentNode.SelectSingleNode("//a[contains(., 'wrong person?')]");
                    if (moreMatches != null)
                    {
                        string artistMatchesUrl = moreMatches.Attributes["href"].Value;
                        artistMatchesUrl = AMG_BASE_URL + HttpUtility.HtmlDecode(artistMatchesUrl.Replace("~C", "~T2C"));

                        htmlDoc = Web.GetResponse(artistMatchesUrl, ENCODING);

                        HtmlNodeCollection artistNodes = htmlDoc.DocumentNode.SelectNodes("//tr[@id='trlink']//strong//a");
                        if (artistNodes != null)
                        {
                            foreach (HtmlNode artistNode in artistNodes)
                            {
                                ClassicalArtist artistItem = new ClassicalArtist();
                                artistItem.Name = artistNode.InnerText;
                                if (!artistItem.Name.Contains("[") && Management.NamesMatch(artistItem.Name, matchArtist, matchType))
                                {
                                    foundArtist.AMGOverviewUrl = HttpUtility.HtmlDecode(artistNode.Attributes["href"].Value);

                                    int startPos = foundArtist.AMGOverviewUrl.IndexOf(":") + 1;
                                    foundArtist.AMGId = foundArtist.AMGOverviewUrl.Substring(startPos);
                                    foundArtist.AMGOverviewUrl = AMG_BASE_URL + HttpUtility.HtmlDecode(foundArtist.AMGOverviewUrl);
                                    foundArtist.Name = artistItem.Name;
                                    foundMatch = true;

                                    htmlDoc = Web.GetResponse(foundArtist.AMGOverviewUrl, ENCODING);
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    // Get a list of all artists and try to match, if we're not on an overview page
                    HtmlNodeCollection artistNodes = htmlDoc.DocumentNode.SelectNodes("//tr[@id='trlink']//strong//a");
                    if (artistNodes != null)
                    {
                        foreach (HtmlNode artistNode in artistNodes)
                        {
                            ClassicalArtist artistItem = new ClassicalArtist();
                            artistItem.Name = artistNode.InnerText;
                            if (!artistItem.Name.Contains("[") && Management.NamesMatch(artistItem.Name, matchArtist, matchType))
                            {
                                foundArtist.AMGOverviewUrl = HttpUtility.HtmlDecode(artistNode.Attributes["href"].Value);

                                int startPos = foundArtist.AMGOverviewUrl.IndexOf(":") + 1;
                                foundArtist.AMGId = foundArtist.AMGOverviewUrl.Substring(startPos);
                                foundArtist.AMGOverviewUrl = AMG_BASE_URL + HttpUtility.HtmlDecode(foundArtist.AMGOverviewUrl);
                                foundArtist.Name = artistItem.Name;
                                foundMatch = true;

                                htmlDoc = Web.GetResponse(foundArtist.AMGOverviewUrl, ENCODING);
                                break;
                            }
                        }
                    }
                }
            }

            //retrieve Genre, etc. info for artist
            if (foundMatch)
            {
                HtmlNodeCollection flavorBlocks = htmlDoc.DocumentNode.SelectNodes("//td[@class='styles_moods']//table//tr");

                if (flavorBlocks != null)
                {
                    HtmlNodeCollection styleNodes = flavorBlocks[1].SelectNodes(".//ul//a");
                    if (styleNodes != null)
                    {
                        foreach(HtmlNode styleNode in styleNodes)
                        {
                            foundArtist.Styles.Add(styleNode.InnerText);
                        }
                    }

                    foundArtist.Country = flavorBlocks[1].SelectNodes(".//td")[1].InnerText;
                    foundArtist.Period = flavorBlocks[3].SelectNodes(".//td")[0].InnerText;
                }

                //get image
                HtmlNode imageNode = htmlDoc.DocumentNode.SelectSingleNode("//td[@class='left-sidebar']//img");
                if (imageNode != null)
                {
                    string imageUrl = imageNode.Attributes["src"].Value;

                    if (!imageUrl.StartsWith("http"))
                    {
                        imageUrl = AMG_BASE_URL + HttpUtility.HtmlDecode(imageUrl);
                    }

                    foundArtist.ArtistImage = Web.GetImage(imageUrl);
                    foundArtist.ArtistImageMimeType = Management.GetImageMIMEType(imageUrl);
                }

            }

            if (!foundMatch)
            {
                return null;
            }
            else
            {
                foundArtist.AMGAllSongsUrl = foundArtist.AMGOverviewUrl + "~T2B";
                AddToArtistCache(foundArtist);
                return foundArtist;
            }
        }

        private Artist SearchForArtist(string artist, ComparisonType matchType) 
        {
            // check the cache
            Artist cacheArtist = GetArtistFromCache(artist);
            if (cacheArtist != null)
            {
                return cacheArtist;
            }

            Artist foundArtist = new Artist();
            bool foundMatch = false;

            string url = AMG_URL + GetQueryString(AmgSearchType.ArtistGroup, artist);

            HtmlDocument htmlDoc = Web.GetResponse(url, ENCODING);

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
                foundArtist.AMGOverviewUrl = AMG_BASE_URL + HttpUtility.HtmlDecode(urlPath.Substring(0, endPos));
                foundMatch = true;
            } 
            else
            {
                // Get a list of all artists and try to match, if we're not on an overview page
                HtmlNodeCollection artistNodes = htmlDoc.DocumentNode.SelectNodes("//tr[@id='trlink']//strong//a");
                foreach (HtmlNode artistNode in artistNodes)
                {
                    Artist artistItem = new Artist();
                    artistItem.Name = artistNode.InnerText;
                    if (Management.NamesMatch(artistItem.Name, artist, matchType))
                    {
                        foundArtist.AMGOverviewUrl = HttpUtility.HtmlDecode(artistNode.Attributes["href"].Value);

                        int startPos = foundArtist.AMGOverviewUrl.IndexOf(":") + 1;
                        foundArtist.AMGId = foundArtist.AMGOverviewUrl.Substring(startPos);
                        foundArtist.AMGOverviewUrl = AMG_BASE_URL + HttpUtility.HtmlDecode(foundArtist.AMGOverviewUrl);
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
                        foundArtist.Genre = genreNodes[0].InnerText;
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
                AddToArtistCache(foundArtist);
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
                    return Management.NamesMatch(a.Name, artist, ComparisonType.Exact); 
                }
            );
            
            return cachedArtist;
        }

        private Album GetAlbumFromCache(string album)
        {
            Album cachedAlbum = albumCache.Find(
                delegate(Album a)
                {
                    return Management.NamesMatch(a.Name, album, ComparisonType.Exact);
                }
            );

            return cachedAlbum;
        }

        private void BuildAlbumIds(List<string> mainAlbumAMGIds, string artistUrl, AmgDiscographyType discType)
        {
            string mainAlbumUrl = artistUrl + "~T" + Convert.ToInt32(discType).ToString();
            HtmlDocument htmlDoc = Web.GetResponse(mainAlbumUrl, ENCODING);
            HtmlNodeCollection mainAlbumBlocks = htmlDoc.DocumentNode.SelectNodes("//table[@id='ExpansionTable1']//tr[@id='trlink' and @class='visible']");
            foreach (HtmlNode mainAlbumBlock in mainAlbumBlocks)
            {
                Album mainAlbum = new Album();

                HtmlNodeCollection mainAlbumData = mainAlbumBlock.SelectNodes("td");
                HtmlNode mainAlbumLinkNode = mainAlbumData[4].SelectSingleNode("a");
                string id = mainAlbumLinkNode.Attributes["href"].Value;
                id = id.Substring(id.IndexOf(":") + 1);

                mainAlbumAMGIds.Add(id);
            }
        }

        private string GetAlbumGenre(string albumUrl)
        {
            string genre = string.Empty;

            HtmlDocument htmlDoc = Web.GetResponse(albumUrl, ENCODING);

            HtmlNodeCollection flavorBlocks = htmlDoc.DocumentNode.SelectNodes("//div[@id='left-sidebar-list']");

            if (flavorBlocks != null)
            {
                HtmlNodeCollection genreNodes = flavorBlocks[0].SelectNodes("ul//a");
                if (genreNodes != null)
                {
                    genre = genreNodes[0].InnerText;
                }
            }

            return genre;
        }

        private void AddToAlbumCache(Album album)
        {
            if (albumCache.Count >= MaxAlbumCache)
            {
                albumCache.RemoveAt(0);
            }

            albumCache.Add(album);
        }

        private void AddToArtistCache(Artist artist)
        {
            if (artistCache.Count >= MaxAlbumCache)
            {
                artistCache.RemoveAt(0);
            }

            artistCache.Add(artist);
        }

        private int GetMovementNumber(string song)
        {
            int movement = 0;
            bool movementFound = false;

            foreach(KeyValuePair<int, string> kvp in romanNumerals)
            {
                foreach(string delimeter in movementDelimeters)
                {
                    string pattern = kvp.Value + delimeter;

                    if (song.Contains(pattern))
                    {
                        movement = kvp.Key;
                        movementFound = true;
                        break;
                    }
                }

                if (movementFound)
                    break;
            }
            return movement;
        }

        private string GetMovementName(string song)
        {
            string movementName = string.Empty;
            bool movementFound = false;

            foreach (KeyValuePair<int, string> kvp in romanNumerals)
            {
                foreach (string delimeter in movementDelimeters)
                {
                    string pattern = kvp.Value + delimeter;

                    if (song.Contains(pattern))
                    {
                        int startPos = song.IndexOf(pattern) + pattern.Length;
                        movementName = song.Substring(startPos).Trim();
                        movementFound = true;
                        break;
                    }
                }

                if (movementFound)
                    break;
            }

            int startIdx = 0;

            for (int i = 0; i < movementName.Length; i++)
            {
                if (Char.IsLetter(movementName[i]))
                {
                    startIdx = i;
                    break;
                }
            }
            
            return movementName.Substring(startIdx);
        }

        private string RemoveMovementInfo(string song)
        {
            string newSong = song;

            bool movementFound = false;

            foreach (KeyValuePair<int, string> kvp in romanNumerals)
            {
                foreach (string delimeter in movementDelimeters)
                {
                    string pattern = kvp.Value + delimeter;

                    if (song.Contains(pattern))
                    {
                        int startPos = song.IndexOf(pattern);
                        newSong = song.Substring(0, startPos);

                        movementFound = true;
                        break;
                    }
                }

                if (movementFound)
                    break;
            }

            return newSong;
        }

        private int GetCompositionYear(string formattedYear)
        {
            List<int> years = new List<int>();

            string[] yearArray = formattedYear.Split(new char[] { '-' });
            foreach (string year in yearArray)
            {
                years.Add( Convert.ToInt32(year.ToUpper().Replace("CA", "").Replace(".", "").Trim()) );
            }

            years.Sort();

            return years[0];
        }

        private string StripMovementPrefix(string movementName)
        {
            string newName = movementName;

            List<string> prefixTags = new List<string>();
            prefixTags.Add("No.");
            prefixTags.Add("1.");
            prefixTags.Add("2.");
            prefixTags.Add("3.");
            prefixTags.Add("4.");
            prefixTags.Add("5.");
            prefixTags.Add("6.");
            prefixTags.Add("7.");
            prefixTags.Add("8.");
            prefixTags.Add("9.");
            prefixTags.Add("10.");

            foreach (string prefix in prefixTags)
            {
                if ( movementName.ToUpper().StartsWith(prefix.ToUpper()) )
                {
                    int pos = prefix.Length;
                    for (int i = pos; i < newName.Length; i++)
                    {
                        if(Char.IsLetter(newName[i]))
                        {
                            newName = newName.Substring(i);
                            break;
                        }
                    }
                }
            }

            return newName;
        }
    
    }
}
