using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Xml;
using Tag.mp3;
using Tag.AMG;
using Tag.Utility;

namespace Tag
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            List<string> files = new List<string>();
            FileManagement.GetFiles(@"C:\mp3", true, files);

            List<MP3File> mp3Files = new List<MP3File>();

            foreach (string file in files)
            {
                if (file.ToUpper().EndsWith(".MP3"))
                {
                    MP3File mp3 = new MP3File();
                    mp3.FilePath = file;
                    mp3Files.Add(mp3);
                }
            }
            
            foreach(MP3File mp3File in mp3Files) 
            {
                MP3File tempMp3File = ShellID3TagReader.ReadID3Tags(mp3File.FilePath);
                CopyID3Info(tempMp3File, mp3File);

                Connection amgConn = new Connection();

                Artist currentArtist = amgConn.SearchForArtist(mp3File.ID3Artist);
                CopyArtistInfo(currentArtist, mp3File);

                Song currentSong = amgConn.SearchForSong(mp3File.ID3SongTitle, currentArtist.AMGAllSongsUrl);
                
                Album currentAlbum = amgConn.SearchForAlbum(currentSong.AMGUrl);

                currentSong.AMGAlbumId = currentAlbum.AMGId;
                currentSong.AMGArtistId = currentArtist.AMGId;

                // save data into ID3 tags
                break;
            }
        }

        private static void CopyArtistInfo(Tag.AMG.Artist artist, Tag.mp3.MP3File file)
        {
            file.AMGArtist.AMGAllSongsUrl = artist.AMGAllSongsUrl;
            file.AMGArtist.AMGId = artist.AMGId;
            file.AMGArtist.AMGOverviewUrl = artist.AMGOverviewUrl;
            file.AMGArtist.Name = artist.Name;
            
            foreach (string genre in artist.Genres)
            {
                file.AMGArtist.Genres.Add(genre);
            }

            foreach (string mood in artist.Moods)
            {
                file.AMGArtist.Moods.Add(mood);
            }

            foreach (string style in artist.Styles)
            {
                file.AMGArtist.Styles.Add(style);
            }

            foreach (string instrument in artist.Instruments)
            {
                file.AMGArtist.Instruments.Add(instrument);
            }
        }

        private static void CopyID3Info(Tag.mp3.MP3File source, Tag.mp3.MP3File target)
        {
            target.ID3Album = source.ID3Album;
            target.ID3Artist = source.ID3Artist;
            target.ID3SongTitle = source.ID3SongTitle;
            target.ID3TrackNumber = source.ID3TrackNumber;
        }
    }
}
