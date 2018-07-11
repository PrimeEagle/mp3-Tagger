using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tag.Utility
{
    public class FileManagement
    {
        static public void GetFiles(string dir, bool recursive, List<string> fileList)
        {
            string[] files = Directory.GetFiles(dir);
            foreach (string file in files)
            {
                fileList.Add(file);
            }

            if (recursive)
            {
                foreach (string dirName in Directory.GetDirectories(dir))
                {
                    GetFiles(dirName, recursive, fileList);
                }
            }
        }

    }
}
