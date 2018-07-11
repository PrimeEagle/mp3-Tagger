using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tag.AMG
{
    public class Track
    {
        private string _title;
        private int _number;
        private string _performer;
        private string _composer;

        public int Number
        {
            get { return _number; }
            set { _number = value; }
        }

        public string Performer
        {
            get { return _performer; }
            set { _performer = value; }
        }

        public string Composer
        {
            get { return _composer; }
            set { _composer = value; }
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }
    
    }
}
