using System;
using System.Collections.Generic;

namespace FrigidBlackwaters.Core
{
    public static class TextFormatting
    {
        public static string ManualWrap(string original, int maxNumCharsPerLine)
        {
            string[] words = original.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

            List<string> results = new List<string>();
            string wrapped = "";
            if (words.Length > 0)
            {
                string currLine = words[0];
                for (int i = 1; i < words.Length; i++)
                {
                    string word = words[i];
                    if (currLine.Length + word.Length + 1 <= maxNumCharsPerLine)
                    {
                        currLine = currLine + ' ' + word;
                    }
                    else
                    {
                        results.Add(currLine);
                        currLine = word;
                    }
                }
                results.Add(currLine);

                for (int i = 0; i < results.Count - 1; i++)
                {
                    wrapped += results[i];
                    wrapped += '\n';
                }
                wrapped += results[results.Count - 1];
            }
            return wrapped;
        }
    }
}
