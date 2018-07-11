using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Tag.Utility
{
    public enum ComparisonType
    {
        Exact = 0,
        StartsWith = 1,
        EndsWith = 2,
        Contains = 3,
        Majority = 4,
        CatalogNumber = 5
    }

    public class Management
    {
        public static bool NamesMatchTest(string name1, string name2)
        {
            return NamesMatch(name1, name2, ComparisonType.Majority);
        }

        
        public static bool NamesMatch(string name1, string name2)
        {
            return NamesMatch(name1, name2, ComparisonType.Exact);
        }

        public static bool NamesMatch(string name1, string name2, ComparisonType matchType)
        {
            bool matched = false;

            switch (matchType)
            {
                case ComparisonType.Exact:
                case ComparisonType.StartsWith:
                case ComparisonType.EndsWith:
                case ComparisonType.Contains:
                    matched = PhraseMatch(name1, name2, matchType);
                    break;
                
                case ComparisonType.Majority:
                    string[] splitName1 = name1.Split(' ');
                    string[] splitName2 = name2.Split(' ');

                    int maxMatches = splitName1.Length < splitName2.Length ? splitName1.Length : splitName2.Length;
                    int actualMatches = 0;

                    List<string> sourceList = new List<string>();
                    Dictionary<string, bool> targetList = new Dictionary<string, bool>();

                    if (splitName1.Length < splitName2.Length)
                    {
                        foreach (string n1 in splitName1)
                        {
                            sourceList.Add(n1);
                        }
                        foreach (string n2 in splitName2)
                        {
                            targetList.Add(n2, false);
                            
                        }
                    }
                    else
                    {
                        foreach (string n1 in splitName1)
                        {
                            sourceList.Add(n1);
                        }
                        foreach (string n2 in splitName2)
                        {
                            targetList.Add(n2, false);
                        }
                    }


                    foreach (string sourceStr in sourceList)
                    {
                        string matchedKey = string.Empty;

                        foreach (KeyValuePair<string, bool> targetKvp in targetList)
                        {
                            if (targetKvp.Value == false)
                            {
                                if (PhraseMatch(sourceStr, targetKvp.Key, ComparisonType.Exact))
                                {
                                    matchedKey = targetKvp.Key;
                                    actualMatches++;
                                    break;
                                }
                            }
                        }

                        if (matchedKey != string.Empty)
                        {
                            targetList[matchedKey] = true;
                        }
                    }



                    if ( (actualMatches * 2) >= maxMatches )
                    {
                        matched = true;
                    }
                    break;
                    
                case ComparisonType.CatalogNumber:
                    List<string> catalogNames = new List<string>();
                    catalogNames.Add("BWV Anh. ");
                    catalogNames.Add("BWV ");
                    catalogNames.Add("K. ");
                    catalogNames.Add("Op. ");
                    catalogNames.Add("WAB ");
                    catalogNames.Add("WoO ");
                    catalogNames.Add("Hess ");
                    catalogNames.Add("JB 1:");

                    foreach (string catalog in catalogNames)
                    {
                        if (name1.ToUpper().Contains(catalog.ToUpper()) &&
                           name2.ToUpper().Contains(catalog.ToUpper()))
                        {
                            StringBuilder catNum1 = new StringBuilder();
                            int startPos1 = name1.IndexOf(catalog) + catalog.Length;

                            for (int i = startPos1; i < name1.Length; i++)
                            {
                                if (!Char.IsDigit(name1[i]))
                                {
                                    break;
                                }
                                else
                                {
                                    catNum1.Append(name1[i]);
                                }
                            }


                            StringBuilder catNum2 = new StringBuilder();
                            int startPos2 = name2.IndexOf(catalog) + catalog.Length;

                            for (int i = startPos2; i < name2.Length; i++)
                            {
                                if (!Char.IsDigit(name2[i]))
                                {
                                    break;
                                }
                                else
                                {
                                    catNum2.Append(name2[i]);
                                }
                            }

                            try
                            {
                                int num1 = Convert.ToInt32(catNum1.ToString());
                                int num2 = Convert.ToInt32(catNum2.ToString());

                                if (num1 == num2)
                                {
                                    matched = true;
                                    break;
                                }
                            }
                            catch 
                            {
                            }
                        }
                    }

                    break;
                
                default:
                    break;
            }

            return matched;
        }


        private static bool PhraseMatch(string name1, string name2, ComparisonType matchType)
        {
            bool match = false;
            List<string> removableWords = new List<string>();

            StringBuilder tempName1 = new StringBuilder(name1.ToUpper());
            StringBuilder tempName2 = new StringBuilder(name2.ToUpper());

            removableWords.Add(" THE ");
            removableWords.Add(" OF ");
            removableWords.Add(" A ");
            removableWords.Add(" AN ");
            removableWords.Add("A ");

            foreach (string word in removableWords)
            {
                tempName1.Replace(word, "");
                tempName2.Replace(word, "");
            }


            //// remove anything in parentheses
            int startPos = tempName1.ToString().IndexOf("(");
            int endPos = tempName1.ToString().IndexOf(")");

            startPos = tempName2.ToString().IndexOf("(");
            endPos = tempName2.ToString().IndexOf(")");
            if (startPos >= 0 && endPos >= 0)
            {
                tempName2 = tempName2.Remove(startPos, endPos - startPos + 1);
            }

            // remove anything in brackets
            startPos = tempName1.ToString().IndexOf("[");
            endPos = tempName1.ToString().IndexOf("]");
            if (startPos >= 0 && endPos >= 0)
            {
                tempName1 = tempName1.Remove(startPos, endPos - startPos + 1);
            }

            startPos = tempName2.ToString().IndexOf("[");
            endPos = tempName2.ToString().IndexOf("]");
            if (startPos >= 0 && endPos >= 0)
            {
                tempName2 = tempName2.Remove(startPos, endPos - startPos + 1);
            }

            tempName1 = tempName1.Replace(" ", "");
            tempName2 = tempName2.Replace(" ", "");

            tempName1 = new StringBuilder(Regex.Replace(tempName1.ToString().ToUpper().Trim(), @"\W*", ""));
            tempName2 = new StringBuilder(Regex.Replace(tempName2.ToString().ToUpper().Trim(), @"\W*", ""));

            if (tempName1.Length > 0 && tempName2.Length > 0)
            {
                switch (matchType)
                {
                    case ComparisonType.Exact:
                        if (tempName1.ToString() == tempName2.ToString())
                        {
                            match = true;
                        }
                        break;

                    case ComparisonType.StartsWith:
                        if (tempName1.ToString() == tempName2.ToString() ||
                            tempName1.ToString().StartsWith(tempName2.ToString()) ||
                            tempName2.ToString().StartsWith(tempName1.ToString()))
                        {
                            match = true;
                        }
                        break;

                    case ComparisonType.EndsWith:
                        if (tempName1.ToString() == tempName2.ToString() ||
                            tempName1.ToString().EndsWith(tempName2.ToString()) ||
                            tempName2.ToString().EndsWith(tempName1.ToString()))
                        {
                            match = true;
                        }
                        break;

                    case ComparisonType.Contains:
                        if (tempName1.ToString() == tempName2.ToString() ||
                            tempName1.ToString().Contains(tempName2.ToString()) ||
                            tempName2.ToString().Contains(tempName1.ToString()))
                        {
                            match = true;
                        }
                        break;

                    default:
                        break;
                }
            }

            return match;
        }

        public static string GetImageMIMEType(string imageUrl)
        {
            string extension = Path.GetExtension(imageUrl).ToUpper();
            string mimeType = string.Empty;

            switch (extension)
            {
                case "JPG":
                case "JPEG":
                    mimeType = "image/jpeg";
                    break;

                case "PNG":
                    mimeType = "image/png";
                    break;

                default:
                    break;
            }

            return mimeType;
        }

        public static string GetRomanNumeral(int num)
        {
            string roman = string.Empty;

            switch(num)
            {
                case 1:
                    roman = "I";
                    break;
                case 2:
                    roman = "II";
                    break;
                case 3:
                    roman = "III";
                    break;
                case 4:
                    roman = "IV";
                    break;
                case 5:
                    roman = "V";
                    break;
                case 6:
                    roman = "VI";
                    break;
                case 7:
                    roman = "VII";
                    break;
                case 8:
                    roman = "VIII";
                    break;
                case 9:
                    roman = "IX";
                    break;
                case 10:
                    roman = "X";
                    break;
                default:
                    break;
            }

            return roman;
        }
    }
}
