using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
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
using Tag.AMG;
using Tag.Utility;

namespace Tag.mp3
{
    public class MP3File
    {
        TagLib.File _tagLibFile;
        private string _filePath;
        private Artist _amgArtist = new Artist();
        private ClassicalArtist _amgClassicalArtist = new ClassicalArtist();
        private Song _amgSong = new Song();
        private ClassicalSong _amgClassicalSong = new ClassicalSong();
        private Album _amgAlbum = new Album();
        private id3 _id3 = new id3();
        private Guid _uniqueId = Guid.NewGuid();
        private bool _classical;

        public bool Classical
        {
            get { return _classical; }
            set { _classical = value; }
        }

        public id3 ID3
        {
            get { return _id3; }
        }

        public Guid UniqueID
        {
            get { return _uniqueId; }
        }

        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }

        public string FileName
        {
            get
            {
                return Path.GetFileName(_filePath);
            }
        }

        public Artist AMGArtist
        {
            get { return _amgArtist; }
        }

        public Song AMGSong
        {
            get { return _amgSong; }
        }

        public ClassicalArtist AMGClassicalArtist
        {
            get { return _amgClassicalArtist; }
        }

        public ClassicalSong AMGClassicalSong
        {
            get { return _amgClassicalSong; }
        }

        public Album AMGAlbum
        {
            get { return _amgAlbum; }
        }

        public MP3File(string fileName)
        {
            _tagLibFile = TagLib.File.Create(fileName);
            _tagLibFile.GetTag(TagLib.TagTypes.Id3v1);

            _filePath = fileName;
        }

        public void LoadArtistData(Artist artist)
        {
            _amgArtist.AMGAllSongsUrl = artist.AMGAllSongsUrl;
            _amgArtist.AMGId = artist.AMGId;
            _amgArtist.AMGOverviewUrl = artist.AMGOverviewUrl;
            _amgArtist.Name = artist.Name;
            _amgArtist.Genre = artist.Genre;

            foreach (string mood in artist.Moods)
            {
                _amgArtist.Moods.Add(mood);
            }

            foreach (string style in artist.Styles)
            {
                _amgArtist.Styles.Add(style);
            }

            foreach (string instrument in artist.Instruments)
            {
                _amgArtist.Instruments.Add(instrument);
            }
        }

        public void LoadArtistData(ClassicalArtist artist)
        {
            _amgClassicalArtist.AMGAllSongsUrl = artist.AMGAllSongsUrl;
            _amgClassicalArtist.AMGId = artist.AMGId;
            _amgClassicalArtist.AMGOverviewUrl = artist.AMGOverviewUrl;
            _amgClassicalArtist.Name = artist.Name;
            _amgClassicalArtist.Period = artist.Period;
            _amgClassicalArtist.Country = artist.Country;
            _amgClassicalArtist.ArtistImage = artist.ArtistImage;
            _amgClassicalArtist.ArtistImageMimeType = artist.ArtistImageMimeType;
        }

        public void LoadSongData(ClassicalSong song)
        {
            _amgClassicalSong.AMGAlbumId = song.AMGAlbumId;
            _amgClassicalSong.AMGArtistId = song.AMGArtistId;
            _amgClassicalSong.AMGId = song.AMGId;
            _amgClassicalSong.AMGUrl = song.AMGUrl;
            _amgClassicalSong.Title = song.Title;
            _amgClassicalSong.AlbumTrackNum = song.AlbumTrackNum;
            _amgClassicalSong.Lyrics = song.Lyrics;
            _amgClassicalSong.Composer = song.Composer;
            _amgClassicalSong.Performer = song.Performer;
            _amgClassicalSong.Description = song.Description;
            _amgClassicalSong.Genre = song.Genre;
            _amgClassicalSong.Movement = song.Movement;
            _amgClassicalSong.MovementName = song.MovementName;
            _amgClassicalSong.PublicationYear = song.PublicationYear;

            foreach (string work in song.WorkTypes)
            {
                _amgClassicalSong.WorkTypes.Add(work);
            }
        }

        public void LoadSongData(Song song)
        {
            _amgSong.AMGAlbumId = song.AMGAlbumId;
            _amgSong.AMGArtistId = song.AMGArtistId;
            _amgSong.AMGId = song.AMGId;
            _amgSong.AMGUrl = song.AMGUrl;
            _amgSong.Title = song.Title;
            _amgSong.AlbumTrackNum = song.AlbumTrackNum;
            _amgSong.Lyrics = song.Lyrics;
            _amgSong.Composer = song.Composer;
            _amgSong.Performer = song.Performer;
        }

        public void LoadAlbumData(Album album)
        {
            _amgAlbum.AMGId = album.AMGId;
            _amgAlbum.AMGUrl = album.AMGUrl;
            _amgAlbum.Name = album.Name;
            _amgAlbum.AlbumArtist = album.AlbumArtist;
            _amgAlbum.ReleaseYear = album.ReleaseYear;
            _amgAlbum.AlbumCoverImage = album.AlbumCoverImage;
            _amgAlbum.AlbumCoverMimeType = album.AlbumCoverMimeType;
            _amgAlbum.Genre = album.Genre;
            _amgAlbum.Publisher = album.Publisher;

            foreach (string mood in album.Moods)
            {
                _amgAlbum.Moods.Add(mood);
            }

            foreach (string style in album.Styles)
            {
                _amgAlbum.Styles.Add(style);
            }

            foreach (string theme in album.Themes)
            {
                _amgAlbum.Themes.Add(theme);
            }

            foreach (Track track in album.Tracks)
            {
                _amgAlbum.Tracks.Add(track);
            }

            _amgAlbum.Compilation = album.Compilation;
        }

        public void PopulateID3()
        {
            _id3 = new id3();

            if (_classical)
            {
                _id3.Album = _amgAlbum.Name;
                _id3.Artist = _amgClassicalArtist.Name;
                _id3.TrackCount = _amgAlbum.TrackCount;
                _id3.AlbumArtist = _amgAlbum.AlbumArtist;

                _id3.Genre = _amgClassicalArtist.Period;
                _id3.Image = _amgClassicalArtist.ArtistImage;

                foreach (string work in _amgClassicalSong.WorkTypes)
                {
                    _id3.Styles.Add(work);
                }

                _id3.TrackNumber = _amgClassicalSong.AlbumTrackNum;
                _id3.Year = _amgClassicalSong.PublicationYear;
                if (_amgClassicalSong.Movement > 0)
                {
                    _id3.SongTitle = _amgClassicalSong.Title + ", " + Management.GetRomanNumeral(_amgClassicalSong.Movement) + ". " + _amgClassicalSong.MovementName;
                }
                else
                {
                    _id3.SongTitle = _amgClassicalSong.Title;
                }
                _id3.Lyrics = _amgClassicalSong.Description;

                string genreStyles = "[" + _id3.Genre + "]" + FormatTagText(_id3.Styles);

                string comments = genreStyles + Environment.NewLine + "Movement: " + _amgClassicalSong.Movement + " (" + _amgClassicalSong.MovementName + ")";

                _id3.Comment = comments;
                _id3.Publisher = _amgAlbum.Publisher;
                _id3.Composer = _amgClassicalSong.Composer;
                _id3.Performer = _amgClassicalSong.Performer;
                _id3.Compilation = _amgAlbum.Compilation;
                _id3.Grouping = _amgClassicalArtist.Country;
            }
            else
            {
                _id3.Album = _amgAlbum.Name;
                _id3.Artist = _amgArtist.Name;
                _id3.TrackCount = _amgAlbum.TrackCount;
                _id3.AlbumArtist = _amgAlbum.AlbumArtist;

                if (_amgAlbum.Genre == string.Empty)
                {
                    _id3.Genre = _amgArtist.Genre;
                }
                else
                {
                    _id3.Genre = _amgAlbum.Genre;
                }
                _id3.Genre = "[" + _id3.Genre + "]" + FormatTagText(_id3.Styles);

                _id3.Image = _amgAlbum.AlbumCoverImage;

                foreach (string instrument in _amgArtist.Instruments)
                {
                    _id3.Instruments.Add(instrument);
                }

                if (_amgAlbum.Styles.Count > 0)
                {
                    foreach (string style in _amgAlbum.Styles)
                    {
                        _id3.Styles.Add(style);
                    }
                }
                else
                {
                    foreach (string style in _amgArtist.Styles)
                    {
                        _id3.Styles.Add(style);
                    }
                }

                if (_amgAlbum.Moods.Count > 0)
                {
                    foreach (string mood in _amgAlbum.Moods)
                    {
                        _id3.Moods.Add(mood);
                    }
                }
                else
                {
                    foreach (string mood in _amgArtist.Moods)
                    {
                        _id3.Moods.Add(mood);
                    }
                }

                _id3.Grouping = FormatTagText(_id3.Moods);

                foreach (string theme in _amgAlbum.Themes)
                {
                    _id3.Themes.Add(theme);
                }

                _id3.TrackNumber = _amgSong.AlbumTrackNum;
                _id3.Year = _amgAlbum.ReleaseYear;
                _id3.SongTitle = _amgSong.Title;
                _id3.Lyrics = _amgSong.Lyrics;

                string genreStyles = "[" + _id3.Genre + "]" + FormatTagText(_id3.Styles);
                string moods = FormatTagText(_id3.Moods);
                string themes = FormatTagText(_id3.Themes);
                string instruments = FormatTagText(_id3.Instruments);

                string comments = themes + Environment.NewLine + instruments + Environment.NewLine + moods + Environment.NewLine + genreStyles;

                _id3.Comment = comments;
                _id3.Publisher = _amgAlbum.Publisher;
                _id3.Composer = _amgSong.Composer;
                _id3.Performer = _amgSong.Performer;
                _id3.Compilation = _amgAlbum.Compilation;
            }
        }

        public void InitializeID3Tags()
        {
            if (_classical)
            {
                ReadClassicalID3Tags();
            }
            else
            {
                ReadID3Tags();
            }
        }

        private void ReadClassicalID3Tags()
        {
            _id3.Album = _tagLibFile.Tag.Album;
            _id3.Artist = _tagLibFile.Tag.Artists[0];
            _id3.Comment = _tagLibFile.Tag.Comment;
            _id3.SongTitle = _tagLibFile.Tag.Title;
            _id3.TrackNumber = Convert.ToInt32(_tagLibFile.Tag.Track);
            _id3.Lyrics = _tagLibFile.Tag.Lyrics;
            _id3.Year = Convert.ToInt32(_tagLibFile.Tag.Year);
            _id3.TrackCount = Convert.ToInt32(_tagLibFile.Tag.TrackCount);
            if (_tagLibFile.Tag.AlbumArtists != null && _tagLibFile.Tag.AlbumArtists.Length > 0)
                _id3.AlbumArtist = _tagLibFile.Tag.AlbumArtists[0];

            try
            {
                _id3.Publisher = GetCustomText("TPUB")[0];
            }
            catch
            {
                _id3.Publisher = string.Empty;
            }


            if (_tagLibFile.Tag.Genres.Length > 0)
                _id3.Genre = _tagLibFile.Tag.Genres[0];

            if (_tagLibFile.Tag.Pictures.Length > 0)
            {
                try
                {
                    _id3.Image = ConvertImage.ToImage(_tagLibFile.Tag.Pictures[0]);
                }
                catch
                {
                    _id3.Image = null;
                    _tagLibFile.Tag.Pictures = null;
                }
            }

            if (_tagLibFile.Tag.Performers != null && _tagLibFile.Tag.Performers.Length > 0)
                _id3.Performer = _tagLibFile.Tag.Performers[0];

            if (_tagLibFile.Tag.Composers != null && _tagLibFile.Tag.Composers.Length > 0)
                _id3.Composer = _tagLibFile.Tag.Composers[0];

            _id3.TrackCount = Convert.ToInt32(_tagLibFile.Tag.TrackCount);

            TagLib.Id3v2.Tag compTag = (TagLib.Id3v2.Tag)_tagLibFile.GetTag(TagLib.TagTypes.Id3v2, false);
            if (compTag != null)
            {
                _id3.Compilation = compTag.IsCompilation;
            }
        }

        private void ReadID3Tags()
        {
            _id3.Album = _tagLibFile.Tag.Album;
            _id3.Artist = _tagLibFile.Tag.Artists[0];
            _id3.Comment = _tagLibFile.Tag.Comment;
            _id3.SongTitle = _tagLibFile.Tag.Title;
            _id3.TrackNumber = Convert.ToInt32(_tagLibFile.Tag.Track);
            _id3.Lyrics = _tagLibFile.Tag.Lyrics;
            _id3.Year = Convert.ToInt32(_tagLibFile.Tag.Year);
            _id3.TrackCount = Convert.ToInt32(_tagLibFile.Tag.TrackCount);
            if (_tagLibFile.Tag.AlbumArtists != null && _tagLibFile.Tag.AlbumArtists.Length > 0)
                _id3.AlbumArtist = _tagLibFile.Tag.AlbumArtists[0];

            try
            {
                _id3.Publisher = GetCustomText("TPUB")[0];
            }
            catch
            {
                _id3.Publisher = string.Empty;
            }


            if (_tagLibFile.Tag.Genres.Length > 0)
                _id3.Genre = _tagLibFile.Tag.Genres[0];

            if (_tagLibFile.Tag.Pictures.Length > 0)
            {
                try
                {
                    _id3.Image = ConvertImage.ToImage(_tagLibFile.Tag.Pictures[0]);
                }
                catch
                {
                    _id3.Image = null;
                    _tagLibFile.Tag.Pictures = null;
                }
            }
            string[] moods = GetCustomText("TMOO");

            if (moods != null)
            {
                foreach (string mood in moods)
                {
                    _id3.Moods.Add(mood);
                }
            }

            if (_tagLibFile.Tag.Performers != null && _tagLibFile.Tag.Performers.Length > 0)
                _id3.Performer = _tagLibFile.Tag.Performers[0];

            if (_tagLibFile.Tag.Composers != null && _tagLibFile.Tag.Composers.Length > 0)
                _id3.Composer = _tagLibFile.Tag.Composers[0];

            _id3.TrackCount = Convert.ToInt32(_tagLibFile.Tag.TrackCount);

            TagLib.Id3v2.Tag compTag = (TagLib.Id3v2.Tag)_tagLibFile.GetTag(TagLib.TagTypes.Id3v2, false);
            if (compTag != null)
            {
                _id3.Compilation = compTag.IsCompilation;
            }
        }

        public void Save()
        {
            _tagLibFile.RemoveTags(TagLib.TagTypes.Id3v1);
            _tagLibFile.RemoveTags(TagLib.TagTypes.Id3v2);

            _tagLibFile.GetTag(TagLib.TagTypes.Id3v2, true);
            _tagLibFile.GetTag(TagLib.TagTypes.Id3v1, true);
            TagLib.Id3v2.Tag.DefaultVersion = 3;
            TagLib.Id3v2.Tag.ForceDefaultVersion = true;

            _tagLibFile.Tag.Album = _id3.Album;
            _tagLibFile.Tag.Performers = new string[] { _id3.Artist };
            _tagLibFile.Tag.AlbumArtists = new string[] { _id3.Artist };
            _tagLibFile.Tag.Artists = new string[] { _id3.Artist };
            _tagLibFile.Tag.Comment = _id3.Comment;
            _tagLibFile.Tag.Title = _id3.SongTitle;
            _tagLibFile.Tag.Track = Convert.ToUInt32(_id3.TrackNumber);
            _tagLibFile.Tag.Lyrics = _id3.Lyrics;
            _tagLibFile.Tag.Year = Convert.ToUInt32(_id3.Year);
            _tagLibFile.Tag.TrackCount = Convert.ToUInt32(_id3.TrackCount);
            _tagLibFile.Tag.AlbumArtists = new string[] { _id3.AlbumArtist };

            if (_id3.Image != null)
            {
                TagLib.Picture pix = (TagLib.Picture)ConvertImage.ToIPicture(_id3.Image, System.Drawing.Imaging.ImageFormat.Jpeg);
                
                //create Id3v2 Picture Frame
                TagLib.Id3v2.AttachedPictureFrame albumCoverPictFrame = new TagLib.Id3v2.AttachedPictureFrame(pix);
                albumCoverPictFrame.MimeType = System.Net.Mime.MediaTypeNames.Image.Jpeg;
                //encoding must be Latin1 to work with iTunes
                albumCoverPictFrame.TextEncoding = TagLib.StringType.Latin1;

                if (_classical)
                {
                    albumCoverPictFrame.Type = TagLib.PictureType.Artist;
                }
                else
                {
                    albumCoverPictFrame.Type = TagLib.PictureType.FrontCover;
                }

                //Id3v2 allows more than one type of image, just one needed
                TagLib.IPicture[] pixs = { albumCoverPictFrame };

                _tagLibFile.Tag.Pictures = pixs;

                if (_classical)
                {
                    _tagLibFile.Tag.Pictures[0].Type = TagLib.PictureType.Artist;
                }
                else
                {
                    _tagLibFile.Tag.Pictures[0].Type = TagLib.PictureType.FrontCover;
                }
            }

            SetCustomText("TMOO", _id3.Moods.ToArray());
            SetCustomText("TPUB", new string[] { _id3.Publisher });

            _tagLibFile.Tag.Genres = new string[] { _id3.Genre };
            _tagLibFile.Tag.Grouping = FormatTagText(_id3.Moods);
            _tagLibFile.Tag.Comment = _id3.Comment;

            _tagLibFile.Tag.Composers = new string[] { _id3.Composer };
            _tagLibFile.Tag.Performers = new string[] { _id3.Performer };
            _tagLibFile.Tag.TrackCount = Convert.ToUInt32(_id3.TrackCount);


            TagLib.Id3v2.Tag compTag = (TagLib.Id3v2.Tag)_tagLibFile.GetTag(TagLib.TagTypes.Id3v2, true);
            if (compTag != null)
            {
                compTag.IsCompilation = _id3.Compilation;
            }
            
            TagLib.Id3v2.Tag idTag = (TagLib.Id3v2.Tag)_tagLibFile.GetTag(TagLib.TagTypes.Id3v2, true);

            if (_classical)
            {
                TagLib.Id3v2.UserTextInformationFrame.Get(idTag, "AMG Album ID", true).Text = new string[] { _amgAlbum.AMGId };
                TagLib.Id3v2.UserTextInformationFrame.Get(idTag, "AMG Album URL", true).Text = new string[] { _amgAlbum.AMGUrl };
                TagLib.Id3v2.UserTextInformationFrame.Get(idTag, "AMG Artist ID", true).Text = new string[] { _amgClassicalArtist.AMGId };
                TagLib.Id3v2.UserTextInformationFrame.Get(idTag, "AMG Artist URL", true).Text = new string[] { _amgClassicalArtist.AMGOverviewUrl };
                TagLib.Id3v2.UserTextInformationFrame.Get(idTag, "AMG Artist Songs URL", true).Text = new string[] { _amgClassicalArtist.AMGAllSongsUrl };
                TagLib.Id3v2.UserTextInformationFrame.Get(idTag, "AMG Song ID", true).Text = new string[] { _amgClassicalSong.AMGId };
                TagLib.Id3v2.UserTextInformationFrame.Get(idTag, "AMG Song URL", true).Text = new string[] { _amgClassicalSong.AMGUrl };
            }
            else
            {
                TagLib.Id3v2.UserTextInformationFrame.Get(idTag, "AMG Album ID", true).Text = new string[] { _amgAlbum.AMGId };
                TagLib.Id3v2.UserTextInformationFrame.Get(idTag, "AMG Album URL", true).Text = new string[] { _amgAlbum.AMGUrl };
                TagLib.Id3v2.UserTextInformationFrame.Get(idTag, "AMG Artist ID", true).Text = new string[] { _amgArtist.AMGId };
                TagLib.Id3v2.UserTextInformationFrame.Get(idTag, "AMG Artist URL", true).Text = new string[] { _amgArtist.AMGOverviewUrl };
                TagLib.Id3v2.UserTextInformationFrame.Get(idTag, "AMG Artist Songs URL", true).Text = new string[] { _amgArtist.AMGAllSongsUrl };
                TagLib.Id3v2.UserTextInformationFrame.Get(idTag, "AMG Song ID", true).Text = new string[] { _amgSong.AMGId };
                TagLib.Id3v2.UserTextInformationFrame.Get(idTag, "AMG Song URL", true).Text = new string[] { _amgSong.AMGUrl };
            }

            //foreach (TagLib.Id3v2.Frame frame in _tagLibFile.GetTag(TagLib.TagTypes.Id3v2) as TagLib.Id3v2.Tag)
            //{
            //    System.Diagnostics.Debug.WriteLine(String.Format("ID: {0}\nToString: {1}", frame.FrameId, frame));
            //}

            _tagLibFile.Save();
        }


        private string FormatTagText(List<string> values)
        {
            StringBuilder text = new StringBuilder();

            foreach (string val in values)
            {
                text.Append(val + "/");
            }

            if (text.Length > 0)
            {
                text = text.Remove(text.Length - 1, 1);
            }

            return text.ToString();
        }

        private string FormatTagXml(string rootTag, string elementTag, List<string> tagValues)
        {
            StringBuilder xml = new StringBuilder();

            if (tagValues.Count > 0)
            {
                xml.Append("<" + rootTag + ">");
                foreach (string val in tagValues)
                {
                    xml.Append("<" + elementTag + ">" + val + "</" + elementTag + ">");
                }
                xml.Append("</" + rootTag + ">");
            }

            return xml.ToString();
        }

        private void SetCustomText(string tag, string[] values)
        {
            TagLib.Id3v2.Tag id3v2 = (TagLib.Id3v2.Tag)_tagLibFile.GetTag(TagLib.TagTypes.Id3v2, true);

            if (id3v2 != null)
                id3v2.SetTextFrame(tag, values);
        }

        private string[] GetCustomText(string tag)
        {
            TagLib.Id3v2.Tag id3v2 = (TagLib.Id3v2.Tag)_tagLibFile.GetTag(TagLib.TagTypes.Id3v2);

            if (id3v2 != null)
                foreach (TagLib.Id3v2.TextInformationFrame f in id3v2.GetFrames(tag))
                    return f.Text;

            return new string[] { };
        }
    }
}
