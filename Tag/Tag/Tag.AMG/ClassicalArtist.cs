using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Tag.AMG
{
    public class ClassicalArtist : Artist
    {
        private string _country = string.Empty;
        private Image _artistImage;
        private string _artistImageMimeType;
        private string _period = string.Empty;


        public string Country
        {
            get { return _country; }
            set { _country = value; }
        }

        public string Period
        {
            get { return _period; }
            set { _period = value; }
        }
        
        public Image ArtistImage
        {
            get { return _artistImage; }
            set { _artistImage = value; }
        }

        public string ArtistImageMimeType
        {
            get { return _artistImageMimeType; }
            set { _artistImageMimeType = value; }
        }
    }
}
