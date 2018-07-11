using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tag.AMG
{
   public class ClassicalSong : Song
    {
        private string _genre = string.Empty;
        private List<string> _workTypes = new List<string>();
        private int _publicationYear;
        private string _description = string.Empty;
        private int _movement;
        private string _movementName;

        public string MovementName
        {
            get { return _movementName; }
            set { _movementName = value; }
        }

        public int Movement
        {
            get { return _movement; }
            set { _movement = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public int PublicationYear
        {
            get { return _publicationYear; }
            set { _publicationYear = value; }
        }

        public List<string> WorkTypes
        {
            get { return _workTypes; }
            set { _workTypes = value; }
        }

        public string Genre
        {
            get { return _genre; }
            set { _genre = value; }
        }

    }
}
