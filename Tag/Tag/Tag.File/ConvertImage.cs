using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Tag.Utility
{
    public class ConvertImage
    {
        public static TagLib.IPicture ToIPicture(Image image, System.Drawing.Imaging.ImageFormat format) 
        {
            MemoryStream ms = new MemoryStream();
            image.Save(ms, format);
            TagLib.ByteVector bv = new TagLib.ByteVector(ms.GetBuffer(), Convert.ToInt32(ms.Length));

            TagLib.Picture pic = new TagLib.Picture();

            pic.Data = bv;
            pic.Type = TagLib.PictureType.FrontCover;

            return pic;
        }

        public static Image ToImage(TagLib.IPicture pic)
        {
            MemoryStream ms = new MemoryStream(pic.Data.Data);
            //ms.Write(pic.Data.Data, 78, pic.Data.Data.Length - 78);
            Image image = new Bitmap(ms);

            return image;
        }
    }
}
