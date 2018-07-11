using System;
using System.Data;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace Tag.mp3
{
    public class id3
    {
        private string _artist;
        private string _album;
        private int _trackNumber;
        private int _year;
        private string _genre;
        private string _lyrics;
        private string _songTitle;
        private string _comment;
        private string _composer;
        private string _performer;
        private System.Drawing.Image _image;
        private List<string> _moods = new List<string>();
        private List<string> _styles = new List<string>();
        private List<string> _themes = new List<string>();
        private List<string> _instruments = new List<string>();
        private int _trackCount;
        private string _publisher;
        private bool _compilation;
        private string _albumArtist;
        private string _grouping;

        public string Grouping
        {
            get { return _grouping; }
            set { _grouping = value; }
        }

        public bool Compilation
        {
            get { return _compilation; }
            set { _compilation = value; }
        }

        public string Publisher
        {
            get { return _publisher; }
            set { _publisher = value; }
        }

        public string AlbumArtist
        {
            get { return _albumArtist; }
            set { _albumArtist = value; }
        }

        public int TrackCount
        {
            get { return _trackCount; }
            set { _trackCount = value; }
        }

        public string Artist
        {
            get { return _artist; }
            set { _artist = value; }
        }

        public string Album
        {
            get { return _album; }
            set { _album = value; }
        }

        public string Composer
        {
            get { return _composer; }
            set { _composer = value; }
        }

        public string Performer
        {
            get { return _performer; }
            set { _performer = value; }
        }

        public int TrackNumber
        {
            get { return _trackNumber; }
            set { _trackNumber = value; }
        }

        public int Year
        {
            get { return _year; }
            set { _year = value; }
        }

        public string SongTitle
        {
            get { return _songTitle; }
            set { _songTitle = value; }
        }

        public string Lyrics
        {
            get { return _lyrics; }
            set { _lyrics = value; }
        }

        public System.Drawing.Image Image
        {
            get { return _image; }
            set { _image = value; }
        }

        public string Comment
        {
            get { return _comment; }
            set { _comment = value; }
        }

        public string Genre
        {
            get { return _genre; }
            set { _genre = value; }
        }

        public List<string> Moods
        {
            get { return _moods; }
        }

        public List<string> Styles
        {
            get { return _styles; }
        }

        public List<string> Themes
        {
            get { return _themes; }
        }

        public List<string> Instruments
        {
            get { return _instruments; }
        }
    }
}
