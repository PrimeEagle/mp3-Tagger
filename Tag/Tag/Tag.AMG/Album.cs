using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Tag.AMG
{
    public class Album : IComparable<Album>
    {
        private string _amgId = string.Empty;
        private string _genre;
        private List<string> _styles = new List<string>();
        private List<string> _moods = new List<string>();
        private List<string> _themes = new List<string>();
        private List<Track> _tracks = new List<Track>();
        private string _amgUrl = string.Empty;
        private string _name = string.Empty;
        private int _releaseYear;
        private Image _albumCoverImage;
        private string _albumCoverMimeType;
        private string _publisher;
        private string _albumArtist;
        private bool _compilation;

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

        public Image AlbumCoverImage
        {
            get { return _albumCoverImage; }
            set { _albumCoverImage = value; }
        }

        public int TrackCount
        {
            get { return _tracks.Count; }
        }

        public string AlbumCoverMimeType
        {
            get { return _albumCoverMimeType; }
            set { _albumCoverMimeType = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Genre
        {
            get { return _genre; }
            set { _genre = value; }
        }

        public List<string> Styles
        {
            get { return _styles; }
        }

        public List<string> Moods
        {
            get { return _moods; }
        }

        public List<string> Themes
        {
            get { return _themes; }
        }

        public List<Track> Tracks
        {
            get { return _tracks; }
        }

        public string AMGUrl
        {
            get { return _amgUrl; }
            set { _amgUrl = value; }
        }

        public int ReleaseYear
        {
            get { return _releaseYear; }
            set { _releaseYear = value; }
        }

        public string AMGId
        {
            get { return _amgId; }
            set { _amgId = value; }
        }

        public bool Equals(Album other)
        {
            if (this.ReleaseYear == other.ReleaseYear)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // static method to get a Comparer object
        public static AlbumComparer GetComparer()
        {
            return new Album.AlbumComparer();
        }

        public int CompareTo(Album b)
        {
            return this.ReleaseYear.CompareTo(b.ReleaseYear);
        }

        public class AlbumComparer : IComparer<Album>
        {

            public bool Equals(Album a, Album b)
            {
                return this.Compare(a, b) == 0;
            }

            public int GetHashCode(Album e)
            {
                return e.GetHashCode();
            }

            public int Compare(Album a, Album b)
            {
                return a.CompareTo(b);
            }
        }
    }
}
