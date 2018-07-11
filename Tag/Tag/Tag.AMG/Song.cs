using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tag.AMG
{
    public class Song
    {
        private string _title = string.Empty;
        private string _amgId = string.Empty;
        private string _amgUrl = string.Empty;
        private string _amgAlbumId = string.Empty;
        private string _amgArtistId = string.Empty;
        private int _albumTrackNum;
        private string _lyrics = string.Empty;
        private string _composer = string.Empty;
        private string _performer = string.Empty;

        public int AlbumTrackNum
        {
            get { return _albumTrackNum; }
            set { _albumTrackNum = value; }
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

        public string AMGUrl
        {
            get { return _amgUrl; }
            set { _amgUrl = value; }
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public string Lyrics
        {
            get { return _lyrics; }
            set { _lyrics = value; }
        }

        public string AMGId
        {
            get { return _amgId; }
            set { _amgId = value; }
        }

        public string AMGAlbumId
        {
            get { return _amgAlbumId; }
            set { _amgAlbumId = value; }
        }

        public string AMGArtistId
        {
            get { return _amgArtistId; }
            set { _amgArtistId = value; }
        }
    }
}
