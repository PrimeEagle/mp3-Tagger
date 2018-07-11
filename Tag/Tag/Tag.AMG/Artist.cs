using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tag.AMG
{
    public class Artist
    {
        private string _name = string.Empty;
        private string _amgId = string.Empty;
        private string _amgOverviewUrl = string.Empty;
        private string _amgAllSongsUrl = string.Empty;
        private string _genre;
        private List<string> _styles = new List<string>();
        private List<string> _moods = new List<string>();
        private List<string> _instruments = new List<string>();

        public string AMGOverviewUrl
        {
            get { return _amgOverviewUrl; }
            set { _amgOverviewUrl = value; }
        }

        public string AMGAllSongsUrl
        {
            get { return _amgAllSongsUrl; }
            set { _amgAllSongsUrl = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string AMGId
        {
            get { return _amgId; }
            set { _amgId = value; }
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

        public List<string> Instruments
        {
            get { return _instruments; }
        }
    }
}
